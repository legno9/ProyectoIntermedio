using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerMeleeAnimation : MonoBehaviour
{
    [SerializeField, Range(1f, 10f)] public float layerTransitionSpeed = 5f;
    [HideInInspector] public UnityEvent OnAttackAnimationComplete = new ();
    [HideInInspector] public UnityEvent OnCanBuffer = new ();
    private Animator animator;
    private AnimatorOverrideController lightAttacksAOC;
    private AnimatorOverrideController heavyAttacksAOC;
    private AnimatorOverrideController currentAOC;
    private const int movementLayer = 0;
    private const int attackLayer = 1;
    private float attackWeight = 0f;
    private bool canBuffer = false;
    private bool lastAttackWasLight = true;
    
    private readonly string[] AttackAnims = new string[]
    { 
        "Attk1PH", 
        "Attk2PH", 
        "Attk3PH",
        "Attk4PH" 
    };
    
    private void Awake() 
    {
        animator = GetComponentInChildren<Animator>();
        OnCanBuffer.AddListener(() => canBuffer = true);
    }
    
    private void Update()
    {
        if (CanMove())
        {
            if (animator.GetLayerWeight(attackLayer) > 0f)
            {
                TransitionToMovementLayer();
            }
        }
        else { attackWeight = 0f;}
    }

    public void WeaponChanged(AnimatorOverrideController animatorBase)
    {
        lightAttacksAOC = new(animatorBase);
        lightAttacksAOC.name = "LightAttacksAOC";

        heavyAttacksAOC = new(animatorBase);
        heavyAttacksAOC.name = "HeavyAttacksAOC";

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new();
        animatorBase.GetOverrides(overrides);

        foreach (var pair in overrides)
        {
            lightAttacksAOC[pair.Key] = pair.Value;
            heavyAttacksAOC[pair.Key] = pair.Value;
        }
    }

    private void TransitionToMovementLayer()
    {
        attackWeight = Mathf.Lerp(attackWeight, 1f, Time.deltaTime * layerTransitionSpeed);
        animator.SetLayerWeight(attackLayer, 1f - attackWeight);

        if (attackWeight > 0.9f)
        {
            animator.SetLayerWeight(attackLayer, 0f);
            animator.SetLayerWeight(movementLayer, 1f);
        }
    }

    public void StartAttackAnimation(WeaponMelee weapon, int attackIndex, bool isLightAttack = true)
    {
        if (animator.GetBool("IsAttacking") || !animator.GetBool("CanAttack"))
        {
            Debug.LogWarning("Cannot attack; skipping animation");
            OnAttackAnimationComplete.Invoke();
            return;
        }

        animator.SetBool("IsAttacking", true);
        animator.SetLayerWeight(attackLayer, 1);
        canBuffer = false;

        if (attackIndex < 0 || attackIndex >= AttackAnims.Length)
        {
            Debug.LogError($"Attack index {attackIndex} out of range");
            return;
        }
        
        AnimationClip targetAnimation = isLightAttack ? 
            weapon.lightAttacks[attackIndex] : weapon.heavyAttacks[attackIndex];
        
        string attackAnimName = AttackAnims[attackIndex];
        
        currentAOC = isLightAttack ? lightAttacksAOC : heavyAttacksAOC;
        
        if (!ReferenceEquals(currentAOC[attackAnimName], targetAnimation))
        {
            currentAOC[attackAnimName] = targetAnimation;
        }
        
        if (animator.GetCurrentAnimatorClipInfo(1).Length > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(1).IsName(attackAnimName))
            {
                StartCoroutine(ResetAndPlayAnimation(attackAnimName));
                return;
            }
        }

        animator.runtimeAnimatorController = currentAOC;

        if (lastAttackWasLight == isLightAttack)
        {
            animator.CrossFade(attackAnimName, 0.1f);
        }
        else 
        {
            animator.Play(attackAnimName);
            lastAttackWasLight = isLightAttack;
        }
    }

    private IEnumerator ResetAndPlayAnimation(string attackAnimName)
    {
        animator.Play("Idle");
        yield return null;
        animator.runtimeAnimatorController = currentAOC;
        yield return null;
        animator.Play(attackAnimName);
    }

    public bool CanMove()
    {
        return !animator.GetBool("IsAttacking");
    }

    public void AttackSequenceEnded()
    {
        animator.SetBool("IsAttacking", false);
    }

    public bool CanBufferAttack()
    {
        return canBuffer;
    }

}
