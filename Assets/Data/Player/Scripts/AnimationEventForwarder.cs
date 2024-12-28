using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForwarder : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnMeleeAttackEvent;
    [HideInInspector] public UnityEvent OnAttackFinishedEvent;
    [HideInInspector] public UnityEvent OnReloadFinishedEvent;

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
}
