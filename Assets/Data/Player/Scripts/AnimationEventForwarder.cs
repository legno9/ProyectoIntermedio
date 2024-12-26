using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForwarder : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnMeleeAttackEvent;
    [HideInInspector] public UnityEvent OnAttackFinishedEvent;

    public void MeleeWeaponAttack()
    {
        OnMeleeAttackEvent?.Invoke();
    }

    public void AttackFinished()
    {
        OnAttackFinishedEvent?.Invoke();
    }
}
