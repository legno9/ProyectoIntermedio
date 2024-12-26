using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent<float> OnHitWithDamage;
    public UnityEvent<float, Collision> OnHitWithCollision;
    public UnityEvent<float, Vector3, Vector3> OnHitWithTrigger;

    public void NotifyCollision(IHitter hitter, Collision collision)
    {
        OnHitWithDamage?.Invoke(hitter.GetDamage());
        OnHitWithCollision?.Invoke(hitter.GetDamage(), collision);
    }

    public void NotifyTrigger(IHitter hitter, Vector3 triggerPos, Vector3 normal)
    {
        OnHitWithDamage?.Invoke(hitter.GetDamage());
        OnHitWithTrigger?.Invoke(hitter.GetDamage(), triggerPos, normal);
    }
}
