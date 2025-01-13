using System.Threading;
using UnityEngine;

public class ParryManager : MonoBehaviour
{
    [SerializeField] private EntityHealth entityHealth;
    [SerializeField] private float blockDamageMultiplier = 0.5f;
    [SerializeField] private float parryTime = 0.5f;

    private float currentParryTime = 0f;

    private void OnEnable()
    {
        currentParryTime = parryTime;
        gameObject.tag = "Parry";
    }

    private void OnDisable()
    {
        entityHealth.blockDamageMultiplier = 1f;
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
