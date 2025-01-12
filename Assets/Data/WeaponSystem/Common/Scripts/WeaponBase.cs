using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController overrideController;
    [SerializeField] protected float range = 2f;
    [SerializeField] protected float attacksPerSecond = 1f;
    [SerializeField] protected float attackAnimationLength = 3f;
    protected float lastAttackTime;

    public float GetCurrentCooldown()
    {
        float timePerAttack = 1f / attacksPerSecond;
        float timeSinceLastAttack = Time.time - lastAttackTime;
        return timeSinceLastAttack / timePerAttack;
    }

    public AnimatorOverrideController GetOverrideController()
    {
        return overrideController;
    }

    public virtual void Init()
    {
        gameObject.SetActive(false);
    }

    public virtual void Select(Animator animator)
    {
        gameObject.SetActive(true);
        animator.runtimeAnimatorController = overrideController;
    }

    public virtual void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        animator.runtimeAnimatorController = null;
    }

    public abstract bool PerformAttack();

    public virtual float GetRange() => range;

    public float GetAttacksPerSecond()
    {
        return attacksPerSecond;
    }

    public float GetAttackAnimationLength()
    {
        return attackAnimationLength;
    }
}
