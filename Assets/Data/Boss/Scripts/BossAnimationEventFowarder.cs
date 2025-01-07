using UnityEngine;
using UnityEngine.Events;

public class BossAnimationEventFowarder : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAttackStarted;
    [HideInInspector] public UnityEvent OnAttackFinished;

    public void AttackStarted()
    {
        OnAttackStarted?.Invoke();
    }

    public void AttackFinished()
    {
        OnAttackFinished?.Invoke();
    }
}
