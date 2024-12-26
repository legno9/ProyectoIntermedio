using UnityEngine;

public class WeaponMelee : WeaponBase
{
    [SerializeField] protected float damage = 1f;
    protected HitCollider hitCollider;

    //[Header("Sound Settings")]
    //[SerializeField] protected AudioClipList attackSounds = new AudioClipList();

    public override void Init()
    {
        base.Init();
        hitCollider = GetComponentInChildren<HitCollider>(true);
        hitCollider.SetDamage(damage);
        lastAttackTime = Time.time - 1f / attacksPerSecond;
    }

    public override void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        animator.runtimeAnimatorController = null;
        hitCollider.gameObject.SetActive(false);
    }

    public override bool PerformAttack()
    {
        if (Time.time - lastAttackTime > 1f / attacksPerSecond)
        {
            lastAttackTime = Time.time;
            //attackSounds.PlayAtPointRandom(transform.position);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivateHitCollider()
    {
        hitCollider.gameObject.SetActive(true);
    }
}
