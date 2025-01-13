using System.Threading;
using UnityEngine;

public class ParryManager : MonoBehaviour
{
    [SerializeField] private HurtCollider parryCollider;
    [SerializeField] private HurtCollider playerCollider;
    [SerializeField] private float blockDamageMultiplier = 0.5f;
    [SerializeField] private float parryTime = 0.5f;

    private bool perfectParry;
    private float currentParryTime = 0f;

    private void OnEnable()
    {
        parryCollider.OnHitWithDamage.AddListener(OnParry);
        currentParryTime = parryTime;
        perfectParry = true;
    }

    private void OnDisable()
    {
        parryCollider.OnHitWithDamage.RemoveListener(OnParry);
    }

    private void Update()
    {
        if (currentParryTime > 0f)
        {
            currentParryTime -= Time.deltaTime;
        }
        else
        {
            perfectParry = false;
        }
    }

    private void OnParry(float damage)
    {
        if (!perfectParry)
        {
            playerCollider.NotifyDamage(parryCollider.GetHitter(), damage *= blockDamageMultiplier);
        }
    }
}
