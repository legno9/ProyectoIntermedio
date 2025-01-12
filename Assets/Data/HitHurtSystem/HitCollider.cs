using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour, IHitter
{
    [Header("Config")]
    [SerializeField] private float damage = 10;
    [SerializeField] private bool deactivateOnHit = false;
    [SerializeField] private string[] affectedTags;

    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent<string> OnHitWithTag;

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (affectedTags.Contains(collision.collider.tag) && collision.collider.TryGetComponent<HurtCollider>(out HurtCollider hurtCollider))
        {
            hurtCollider.NotifyCollision(this, collision);
            OnHit.Invoke();
            OnHitWithTag.Invoke(collision.collider.tag);
            
            if (deactivateOnHit){gameObject.SetActive(false);}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (affectedTags.Contains(other.tag) && other.TryGetComponent<HurtCollider>(out HurtCollider hurtCollider))
        {
            Vector3 triggerPoint = other.ClosestPoint(transform.position);
            Vector3 normal = (transform.position - triggerPoint).normalized;

            hurtCollider.NotifyTrigger(this, triggerPoint, normal);
            OnHit.Invoke();
            OnHitWithTag.Invoke(other.tag);

            if (deactivateOnHit){gameObject.SetActive(false);}

        }
    }
}
