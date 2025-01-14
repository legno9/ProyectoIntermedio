using System.Threading;
using UnityEngine;

public class ParryManager : MonoBehaviour
{
    [SerializeField] private EntityHealth entityHealth;
    [SerializeField] private float blockDamageMultiplier = 0.5f;
    [SerializeField] private float parryTime = 0.5f;

    [SerializeField] private GameObject parryVFX;
    [SerializeField] private AudioClipList parrySounds;
    private HurtCollider hurtCollider;

    private float currentParryTime = 0f;

    private void Awake()
    {
        hurtCollider = GetComponent<HurtCollider>();
    }

    private void OnEnable()
    {
        currentParryTime = parryTime;
        gameObject.tag = "Parry";
        hurtCollider.OnHitWithTrigger.AddListener(AttackParried);
    }

    private void AttackParried(float damage, Vector3 triggerPos, Vector3 normal)
    {
        Instantiate(parryVFX, triggerPos, Quaternion.identity);
        parrySounds.PlayAtPointRandom(triggerPos);
    }

    private void OnDisable()
    {
        entityHealth.blockDamageMultiplier = 1f;
        hurtCollider.OnHitWithTrigger.RemoveListener(AttackParried);
    }

    private void Update()
    {
        if (currentParryTime > 0f)
        {
            currentParryTime -= Time.deltaTime;
        }
        else
        {
            gameObject.tag = "Untagged";
            entityHealth.blockDamageMultiplier = blockDamageMultiplier;
        }
    }
}
