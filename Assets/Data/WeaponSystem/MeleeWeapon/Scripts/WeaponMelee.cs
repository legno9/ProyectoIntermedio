using UnityEngine;

public class WeaponMelee : WeaponBase
{
    [SerializeField] protected float damage = 1f;
    protected HitCollider[] hitColliders;
    public AnimationClip[] lightAttacks;
    public AnimationClip[] heavyAttacks;

    //[Header("Sound Settings")]
    //[SerializeField] protected AudioClipList attackSounds = new AudioClipList();

    public override void Init()
    {
        base.Init();
        hitColliders = GetComponentsInChildren<HitCollider>(true);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SetDamage(damage);
            hitCollider.gameObject.SetActive(false);
        }
        
    }

    public override void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        animator.runtimeAnimatorController = null;
        Attacking(false);
    }

    public override bool PerformAttack()
    {
        return true;
    }

    public void Attacking(bool value)
    {
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.gameObject.SetActive(value);
        }
    }

    public int LightComboCount()
    {
        return lightAttacks.Length;
    }

    public int HeavyComboCount()
    {
        return heavyAttacks.Length;
    }
}
