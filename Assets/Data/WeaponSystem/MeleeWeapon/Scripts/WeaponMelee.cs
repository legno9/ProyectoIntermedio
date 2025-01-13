using System.Collections;
using UnityEngine;

public class WeaponMelee : WeaponBase
{
    [Header("Melee Settings")]
    [SerializeField] protected AnimationEventForwarder animationEvent;
    [SerializeField] protected HitCollider[] hitColliders;
    [SerializeField] protected ParticleSystem[] AttachedVFX;

    [Header("Attacks Settings")]
    [SerializeField] protected float normalDamage = 1f;
    [SerializeField] protected float finisherDamage = 1f;
    public int maxCombo = 4;
    public float delayResetCombo = 0.5f;
    public AnimationClip[] primaryAttacks;
    public AnimationClip[] secondaryAttacks;

    //[Header("Sound Settings")]
    //[SerializeField] protected AudioClipList attackSounds = new AudioClipList();

    private void OnEnable() 
    {
        animationEvent.OnMeleeAttackEvent.AddListener(() => SetCollidersState(true));
        animationEvent.OnAttackFinishedEvent.AddListener(() => SetCollidersState(false));
        animationEvent.OnMeleeAttackIndexEvent.AddListener((index) => SetCollidersState(true, index));
        animationEvent.OnAttackFinishedIndexEvent.AddListener((index) => SetCollidersState(false, index));
    }

    private void OnDisable() 
    {
        animationEvent.OnMeleeAttackEvent.RemoveAllListeners();
    }

    public override bool PerformAttack()//Unused on melee
    {
        return true;
    }

    public override void Init()
    {
        base.Init();
        SetCollidersState(false);
    }

    public override void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        animator.runtimeAnimatorController = null;
        SetCollidersState(false);
    }

    private void SetCollidersState(bool activated, int index = 0)
    {
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if (i == index)
            {
                hitColliders[i].gameObject.SetActive(activated);
                
                if(activated){AttachedVFX[i].Play();}
                else{AttachedVFX[i].Stop();}
            }
        }
    }

    public void PrepareForDamage(bool isFinisherAttack)
    {
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SetDamage(isFinisherAttack ? finisherDamage : normalDamage);
        }
    }
}
