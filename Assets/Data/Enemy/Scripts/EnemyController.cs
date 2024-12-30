using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(EntityHealth))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IMovingAnimatable
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private TargetFollower targetFollower;

    private const float MIN_ROTATION_FOR_MOVEMENT = 45f;
    private Animator animator;
    private EntityHealth entityLife;
    private NavMeshAgent agent;
    private EnemyWeaponManager weaponManager;
    private Transform target;
    

    private void Awake()
    {
        weaponManager = GetComponent<EnemyWeaponManager>();
        animator = GetComponentInChildren<Animator>();
        entityLife = GetComponent<EntityHealth>();
        agent = GetComponent<NavMeshAgent>();
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
    }

    private void OnDisable()
    {
        entityLife.OnDeath.RemoveListener(OnDeath);
    }

    public Transform GetCurrentWeaponShootPoint()
    {
        return weaponManager.GetCurrentWeapon().GetComponentInChildren<BarrelByRaycast>().transform;
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
        animator.enabled = false;
        // GetComponentInChildren<HitCollider>(true).gameObject.SetActive(false);
        // GetComponentInChildren<HurtCollider>(true).gameObject.SetActive(false);
        // GetComponentInChildren<Ragdollizer>().Ragdollize();

        // Instantiate(despawnEffect, transform.position, Quaternion.identity).GetComponent<VFXResizer>().ChangeSize(transform.localScale.y);

        // DOVirtual.DelayedCall(5f, () => Destroy(gameObject));
        Destroy(gameObject);
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
