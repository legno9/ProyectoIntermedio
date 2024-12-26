using UnityEngine;

public class WeaponMelee_WithLimitedUses : WeaponMelee
{
    [SerializeField] private int maxUses = 3;
    private int currentUses = 0;
    [SerializeField] private float secondsToRecharge = 5;

    public override void Init()
    {
        base.Init();
        currentUses = maxUses;
    }

    private void Update()
    {
        if (Time.time - lastAttackTime >= secondsToRecharge && currentUses < maxUses)
        {
            currentUses = maxUses;
        }
    }

    public override bool PerformAttack()
    {
        if (Time.time - lastAttackTime > 1f / attacksPerSecond)
        {
            if (currentUses <= 0)
            {
                return false;
            }
            else
            {
                currentUses--;
                lastAttackTime = Time.time;
                //attackSounds.PlayAtPointRandom(transform.position);
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public int GetCurrentUses()
    {
        return currentUses;
    }
}
