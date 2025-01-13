using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerMeleeAnimation : MonoBehaviour
{
    [SerializeField, Range(1f, 10f)] private float layerTransitionSpeed = 5f;
    [SerializeField] private float transitionBetweenAttacks = 0.25f;
    [HideInInspector] public UnityEvent OnAttackAnimationComplete = new ();
    [HideInInspector] public UnityEvent OnCanBuffer = new ();
    private Animator animator;
    private AnimatorOverrideController newAttacksAOC;
    private PlayerMovement playerMovement;
    private const int movementLayer = 0;
    private const int attackLayer = 1;
    private float attackWeight = 0f;
    private bool canBuffer = false;
    
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
        playerMovement = GetComponent<PlayerMovement>();
        OnCanBuffer.AddListener(() => canBuffer = true);
    }
    
    private void Update()
    {
        if (CanMove())
        {
            playerMovement.canMove = true;
            if (animator.GetLayerWeight(attackLayer) > 0f)
            {
                TransitionToMovementLayer();
            }
        }
        else 
        { 
            playerMovement.canMove = false;
            attackWeight = 0f;
        }
    }

    public void WeaponChanged(AnimatorOverrideController animatorBase)
    {
        newAttacksAOC = new(animatorBase){name = "NewAttacksAOC"};

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new();
        animatorBase.GetOverrides(overrides);

        foreach (var pair in overrides)
        {
            newAttacksAOC[pair.Key] = pair.Value;
        }

        animator.runtimeAnimatorController = newAttacksAOC;
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

    public void StartAttackAnimation(WeaponMelee weapon, int attackIndex, bool isPrimaryAttack)
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
        
        AnimationClip targetAnimation = isPrimaryAttack ? 
            weapon.primaryAttacks[attackIndex] : weapon.secondaryAttacks[attackIndex];
        
        string attackAnimName = AttackAnims[attackIndex];
                
        if (!ReferenceEquals(newAttacksAOC[attackAnimName], targetAnimation))
        {
            newAttacksAOC[attackAnimName] = targetAnimation;
            
        }

        StartCoroutine(PlayAfterOneFrame(attackAnimName));
    }

    IEnumerator PlayAfterOneFrame(string clip)
    {
        yield return null;
        animator.CrossFade(clip, transitionBetweenAttacks);
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
