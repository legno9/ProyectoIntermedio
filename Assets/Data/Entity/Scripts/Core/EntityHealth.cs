using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;
    //[SerializeField] private AudioClipList hurtSounds;
    //[SerializeField] private AudioClipList deathSounds;

    [SerializeField] private float passiveRegenRate = 0f;
    [SerializeField] private float passiveRegenDelay = 10f;
    private float timeSinceLastDamage = 0f;

    [HideInInspector] public UnityEvent<float, float> OnHealthChanged; // current health and health change
    [HideInInspector] public UnityEvent OnDeath;

    #region Debug
    [SerializeField] private float debugLifeToAdd = 0f;
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
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, damage);
        timeSinceLastDamage = 0f;

        if (Mathf.Sign(damage) == 1)
        {
            //hurtSounds.PlayAtPointRandom(transform.position);
        }

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            //deathSounds.PlayAtPointRandom(transform.position);
        }
    }

    private void OnDisable()
    {
        hurtCollider.OnHitWithDamage.RemoveListener(OnHitWithDamage);
    }

    private void Update()
    {
        if (currentHealth < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= passiveRegenDelay)
            {
                RecoverHealth(passiveRegenRate * Time.deltaTime);
            }
        }
    }

    public void RecoverHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, amount);
    }
}
