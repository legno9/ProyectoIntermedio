using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForwarder : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnMeleeAttackEvent;
    [HideInInspector] public UnityEvent OnAttackFinishedEvent;
    [HideInInspector] public UnityEvent OnReloadFinishedEvent;
    [HideInInspector] public UnityEvent<int> OnMeleeAttackIndexEvent;
    [HideInInspector] public UnityEvent<int> OnAttackFinishedIndexEvent;

    public void MeleeWeaponAttack()
    {
        OnMeleeAttackEvent?.Invoke();
    }

    public void AttackFinished()
    {
        OnAttackFinishedEvent?.Invoke();
    }

    public void ReloadingFinished()
    {
        OnReloadFinishedEvent?.Invoke();
    }

    public void MeleeWeaponAttackWithIndex(int index)
    {
        OnMeleeAttackIndexEvent?.Invoke(index);
    }

    public void AttackFinishedIndexEvent(int index)
    {
        OnAttackFinishedIndexEvent?.Invoke(index);
    }
}
