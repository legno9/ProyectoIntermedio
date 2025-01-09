using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Events;

public class BossController : MonoBehaviour, IMovingAnimatable
{
    [SerializeField] private List<BossAttack> attacks;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private HitColliderSelfDeactivation axeCollider;
    [SerializeField] private List<HitColliderSelfDeactivation> specialAttackColliders;
    [SerializeField] private ParticleSystem axeTrailVFX;
    [SerializeField] private float maxAngleDiff = 5f;
    [SerializeField] private float angularVelocity = 180f;
    private (float x, float y) cooldownRange = (0.5f, 1.5f);
    private float currentCooldown = 0f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private HurtCollider hurtCollider;
    private EntityHealth entityHealth;

    public UnityEvent OnAttack;

    // Flinch Meter
    [SerializeField] private float flinchMeterMax = 100f;
    [SerializeField] private float strongAttackThreshold = 10f;
    [SerializeField] private float parryDamage = 25f;
    [SerializeField] private float knockedDownDuration = 5f;
    private float flinchMeter = 0f;
    private bool isAttacking = false;
    private bool isKnockedDown = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        hurtCollider = GetComponent<HurtCollider>();
        entityHealth = GetComponent<EntityHealth>();

        var emission = axeTrailVFX.emission;
        emission.enabled = false;

        EnableMovement();
    }

    private void OnEnable()
    {
        hurtCollider.OnHitWithDamage.AddListener(AddFlinchAmount);
        entityHealth.OnDeath.AddListener(OnDeath);

        foreach (var hitCollider in GetComponentsInChildren<HitCollider>(true))
        {
            hitCollider.OnHitWithTag.AddListener(CheckParry);
        }
    }

    private void OnDisable()
    {
        hurtCollider.OnHitWithDamage.RemoveListener(AddFlinchAmount);
        entityHealth.OnDeath.RemoveListener(OnDeath);

        foreach (var hitCollider in GetComponentsInChildren<HitCollider>(true))
        {
            hitCollider.OnHitWithTag.RemoveListener(CheckParry);
        }
    }

    private void Update()
    {
        currentCooldown -= Time.deltaTime;
        foreach (BossAttack attack in attacks)
        {
            attack.ReduceCooldown(Time.deltaTime);
        }

        Vector3 playerDir = Vector3.ProjectOnPlane(playerTransform.position - transform.position, Vector3.up).normalized;
        float angleDiff = Vector3.SignedAngle(transform.forward, playerDir, Vector3.up);
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool ableToAttack = Mathf.Abs(angleDiff) < maxAngleDiff && currentCooldown <= 0f && !isAttacking && !isKnockedDown;

        if (ableToAttack)
        {
            var usableAttacks = attacks.Where(a => a.IsReady && distance < a.MinDistance);

            if (usableAttacks.Any())
            {
                Debug.Log(usableAttacks.Count());
                int index = UnityEngine.Random.Range(0, usableAttacks.Count());
                BossAttack attackToUse = usableAttacks.ElementAt(index);
                StartAttack(distance, attackToUse);
            }
        }
        else if (!isKnockedDown && !isAttacking)
        {
            MoveAndRotate(angleDiff);
        }
        
    }

    private void StartAttack(float distance, BossAttack attackToUse)
    {
        OnAttack?.Invoke();
        int minComboLength = Mathf.FloorToInt((distance / attackToUse.MinDistance) * attackToUse.MaxComboLength) + 1;
        int comboLength = UnityEngine.Random.Range(minComboLength, 4);
        animator.SetInteger("ComboLength", comboLength);
        animator.SetTrigger(attackToUse.TriggerName);
        attackToUse.ResetCooldown();
        isAttacking = true;

        var emission = axeTrailVFX.emission;
        emission.enabled = true;

        DisableMovement();
    }

    public void OnAttackFinished()
    {
        currentCooldown = UnityEngine.Random.Range(cooldownRange.x, cooldownRange.y);

        isAttacking = false;

        var emission = axeTrailVFX.emission;
        emission.enabled = false;

        EnableMovement();
    }

    private void MoveAndRotate(float angleDiff)
    {
        navMeshAgent.SetDestination(playerTransform.position);

        float angleToApply = angularVelocity * Time.deltaTime;
        angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angleDiff));

        Quaternion rotationToApply =
            Quaternion.AngleAxis(
                angleToApply * Mathf.Sign(angleDiff),
                Vector3.up);
        transform.rotation *= rotationToApply;
    }

    public void ActivateAxeHitbox(float duration)
    {
        axeCollider.Activate(duration);
    }

    public void ActivateSpecialHitbox(int index)
    {
        specialAttackColliders[index].Activate();
    }

    private void EnableMovement()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
    }

    private void DisableMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.updatePosition = false;
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
        Vector3 localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
        Vector3 normalizedLocalVelocity = localVelocity / navMeshAgent.speed;
        return normalizedLocalVelocity;
    }

    private void AddFlinchAmount(float damage)
    {
        float flichAmount = damage >= strongAttackThreshold ? damage * 2 : damage;
        flinchMeter = Mathf.Clamp(flinchMeter + damage, 0f, flinchMeterMax);

        if (flinchMeter == flinchMeterMax && damage >= strongAttackThreshold && !isKnockedDown)
        {
            animator.SetTrigger("KnockedDown");
            flinchMeter = 0f;
            isKnockedDown = true;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            OnAttackFinished();
            DisableMovement();
            Invoke("GetUp", knockedDownDuration);
        }
    }

    private void GetUp()
    {
        animator.SetTrigger("GetUp");
    }

    private void CheckParry(string tag)
    {
        if (tag == "Parry" && !isKnockedDown)
        {
            OnParried();
        }
    }

    private void OnParried()
    {
        AddFlinchAmount(parryDamage);

        if (!isKnockedDown)
        {
            animator.SetTrigger("Parried");
            OnAttackFinished();
            DisableMovement();
        }
    }

    public void OnGetUp()
    {
        isKnockedDown = false;
        EnableMovement();
    }

    private void OnDeath()
    {
        animator.SetTrigger("Die");
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        OnAttackFinished();
        DisableMovement();
        entityHealth.enabled = false;
        hurtCollider.enabled = false;
        Invoke("DestroyAfterDeath", 5f);
    }

    private void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}

[Serializable]
public class BossAttack
{
    [field: SerializeField] public string TriggerName { get; set; }
    [field: SerializeField] public int MaxComboLength { get; set; }
    [field: SerializeField] public float MinDistance { get; set; }
    [field: SerializeField] public float Cooldown { get; set; }

    private float currentCooldown = 0f;
    public bool IsReady { get => currentCooldown <= 0; }
    public void ResetCooldown() => currentCooldown = Cooldown;
    public void ReduceCooldown(float amount) => currentCooldown -= amount;
}
