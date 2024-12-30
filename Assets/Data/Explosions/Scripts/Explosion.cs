using UnityEngine;

public class Explosion : MonoBehaviour, IHitter
{
    [SerializeField] float force = 200f;
    [SerializeField] float damage = 10f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask occluderLayerMask = Physics.DefaultRaycastLayers;

    [SerializeField] private GameObject visualExplosionPrefab;
    [SerializeField] private AudioClipList explosionSounds;

    private void Start()
    {
        foreach (Collider c in Physics.OverlapSphere(transform.position, radius, targetLayerMask))
        {
            if (!Physics.Linecast(transform.position, c.transform.position, out RaycastHit hit, occluderLayerMask) ||
                (hit.collider == c))
            {
                c.GetComponent<HurtCollider>()?.NotifyTrigger(this, hit.point, hit.normal);
            }

            c.attachedRigidbody?.AddExplosionForce(force, transform.position, radius);
        }

        explosionSounds.PlayAtPointRandom(transform.position);
        Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
