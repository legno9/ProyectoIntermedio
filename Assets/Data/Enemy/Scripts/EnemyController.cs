using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

[RequireComponent(typeof(EntityHealth))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IMovingAnimatable
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private TargetFollower targetFollower;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject myTarget;
    [SerializeField] private CapsuleCollider Collider;
    private const float MIN_ROTATION_FOR_MOVEMENT = 45f;
    private Animator animator;
    private EntityHealth entityLife;
    private NavMeshAgent agent;
    private BehaviorGraphAgent behaviourAgent;
    private EnemyWeaponManager weaponManager;
    private Transform target;

    

    private void Awake()
    {
        weaponManager = GetComponent<EnemyWeaponManager>();
        animator = GetComponentInChildren<Animator>();
        entityLife = GetComponent<EntityHealth>();
        agent = GetComponent<NavMeshAgent>();
        behaviourAgent = GetComponent<BehaviorGraphAgent>();
    }

    private void Start()
    {
        Behaviour[] constraints = new Behaviour[]
        {
            weaponManager.GetCurrentWeapon().GetComponent<AimConstraint>(),
            aimRig
        };

        targetFollower.SetConstraints(constraints);
    }

    private void OnEnable()
    {
        entityLife.OnDeath.AddListener(OnDeath);
        entityLife.OnDamaged.AddListener(Damaged);
        entityLife.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        entityLife.OnDeath.RemoveListener(OnDeath);
        entityLife.OnDamaged.RemoveListener(Damaged);
        entityLife.OnHealthChanged.RemoveListener(OnHealthChanged);
    }

    public Transform GetCurrentWeaponShootPoint()
    {
        return weaponManager.GetCurrentWeapon().GetComponentInChildren<BarrelByRaycast>().transform;
    }

    private void Damaged()
    {
        if (behaviourAgent.GetVariable<bool>("TargetDetected", out var targetDetected) && !targetDetected)
        {
            targetDetected.Value = true;
            behaviourAgent.SetVariableValue("TimerToUpdateTarget", 1f);
        }
    }

    private void OnHealthChanged(float currentHealth, float damage)
    {
        hpBar.fillAmount = currentHealth / entityLife.GetMaxHealth();
    }

    public void Shoot(Transform target)
    {
        this.target = target;
        targetFollower.UpdateFollower();
        weaponManager.PerformAttack();
    }

    private void OnDeath()
    {
        enabled = false;
        agent.enabled = false;
        behaviourAgent.enabled = false;
        animator.enabled = false;
        myTarget.SetActive(false);
        Collider.enabled = false;
        
        GetComponentInChildren<Ragdollizer>().Ragdollize();
        Instantiate(weaponManager.GetCurrentWeaponAmmo(), transform.position, Quaternion.identity);
        gameObject.GetComponentInParent<EnemiesGroupManager>()?.RemoveEnemy(gameObject);

        DOVirtual.DelayedCall(5f, () => Destroy(gameObject));
    }

    public float GetNormalizedForwardVelocity()
    {
        return CalcNormalizedLocalVelocity().z;
    }

    public float GetNormalizedHorizontalVelocity()
    {
        return CalcNormalizedLocalVelocity().x;
    }

    private Vector3 CalcNormalizedLocalVelocity()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        Vector3 normalizedLocalVelocity = localVelocity / agent.speed;
        return normalizedLocalVelocity;
    }
}
