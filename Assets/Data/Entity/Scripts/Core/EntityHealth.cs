using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;
    //[SerializeField] private AudioClipList hurtSounds;
    //[SerializeField] private AudioClipList deathSounds;

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
}
