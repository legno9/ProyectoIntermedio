using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent<float> OnHitWithDamage;
    public UnityEvent<float, Collision> OnHitWithCollision;
    public UnityEvent<float, Vector3, Vector3> OnHitWithTrigger;

    //[SerializeField] private GameObject damageNumberPopUp;
    private IHitter hitter;

    public void NotifyCollision(IHitter collisionHitter, Collision collision)
    {
        hitter = collisionHitter;
        OnHitWithDamage?.Invoke(hitter.GetDamage());
        OnHitWithCollision?.Invoke(hitter.GetDamage(), collision);
        //Instantiate(damageNumberPopUp, collision.contacts[0].point, Quaternion.identity).GetComponent<DamageNumberPopUp>().Initialize(hitter.GetDamage());
    }

    public void NotifyTrigger(IHitter triggerHitter, Vector3 triggerPos, Vector3 normal)
    {
        hitter = triggerHitter;
        OnHitWithDamage?.Invoke(hitter.GetDamage());
        OnHitWithTrigger?.Invoke(hitter.GetDamage(), triggerPos, normal);
        //Instantiate(damageNumberPopUp, triggerPos, Quaternion.identity).GetComponent<DamageNumberPopUp>().Initialize(hitter.GetDamage());
    }

    public void NotifyDamage(IHitter hitter, float damage)
    {
        OnHitWithDamage?.Invoke(damage);
    }

    public IHitter GetHitter()
    {
        return hitter;
    }
}
