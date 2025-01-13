using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;
    [SerializeField] private AudioClipList hurtSounds;
    [SerializeField] private AudioClipList deathSounds;

    [SerializeField] private float passiveRegenRate = 0f;
    [SerializeField] private float passiveRegenDelay = 10f;
    private float timeSinceLastDamage = 0f;

    [SerializeField] private float invulnerabilityTime = 0.5f;
    private float currentInvulnerabilityTime = 0f;
    [SerializeField] private GameObject damageNumberPopUp;

    [HideInInspector] public UnityEvent<float, float> OnHealthChanged; // current health and health change
    [HideInInspector] public UnityEvent OnDeath;
    [HideInInspector] public UnityEvent OnDamaged;

    private List<float> damageToDeal = new();
    private bool canBeDamaged = true;

    #region Debug
    // [SerializeField] private float debugLifeToAdd = 0f;
    [SerializeField] private float debugLifeToSubtract = 0f;
    [SerializeField] private bool debugAplyLifeChange;

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealthPercentage() => currentHealth / maxHealth;

    private void OnValidate()
    {
        if (debugAplyLifeChange)
        {
            debugAplyLifeChange = false;
            OnHitWithDamage(debugLifeToSubtract);
            timeSinceLastDamage = 0f;
        }
    }
    #endregion

    HurtCollider hurtCollider;

    private void Awake()
    {
        currentHealth = maxHealth;
        hurtCollider = GetComponentInChildren<HurtCollider>();
    }

    private void OnEnable()
    {
        hurtCollider.OnHitWithDamage.AddListener(OnHitWithDamage);
    }

    private void OnHitWithDamage(float damage)
    {
        if (currentInvulnerabilityTime > 0f)
        {
            return;
        }

        damageToDeal.Add(damage);
    }

    private void OnDisable()
    {
        hurtCollider.OnHitWithDamage.RemoveListener(OnHitWithDamage);
    }

    private void Update()
    {
        currentInvulnerabilityTime -= Time.deltaTime;

        if (currentHealth < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= passiveRegenDelay)
            {
                RecoverHealth(passiveRegenRate * Time.deltaTime);
            }
        }
    }

    private void LateUpdate()
    {
        if (!canBeDamaged)
        {
            canBeDamaged = true;
            return;
        }

        if (damageToDeal.Count > 0)
        {
            foreach (var damage in damageToDeal)
            {
                currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
                OnHealthChanged?.Invoke(currentHealth, damage);
                OnDamaged?.Invoke();
                Instantiate(damageNumberPopUp, transform.position + Vector3.up * 1.5f * transform.localScale.y, Quaternion.identity).GetComponent<DamageNumberPopUp>().Initialize(damage);
            }

            currentInvulnerabilityTime = invulnerabilityTime;
            timeSinceLastDamage = 0f;

            hurtSounds.PlayAtPointRandom(transform.position);

            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
                deathSounds.PlayAtPointRandom(transform.position);
            }

            damageToDeal.Clear();
        }
    }

    public void HitWasParried()
    {
        canBeDamaged = false;
    }

    public void RecoverHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, amount);
    }
}
