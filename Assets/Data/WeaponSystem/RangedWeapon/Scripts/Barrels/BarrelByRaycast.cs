using UnityEngine;

public class BarrelByRaycast : BarrelBase, IHitter
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private GameObject bulletTrailPrefab;

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public override void Shoot()
    {
        // Calculate start and end positions before checking for targets
        Vector3 bulletStartPosition = transform.position;
        Quaternion spreadToApply = Quaternion.Euler(0, Random.Range(-(spread / 2), (spread / 2)), 0);
        Vector3 normalizedBulletDirection = (spreadToApply * transform.forward).normalized;
        Vector3 bulletEndPosition = transform.position + normalizedBulletDirection * range;

        // Check for targets that overlap the firing point
        Collider[] colliders = Physics.OverlapSphere(bulletStartPosition, 0.0001f, layerMask);

        if (colliders.Length == 0) // No overlaping targets, we can safely raycast
        {
            if (Physics.Raycast(transform.position, normalizedBulletDirection, out RaycastHit hitInfo, range, layerMask))
            {
                bulletEndPosition = hitInfo.point;
                HurtCollider hurtCollider = hitInfo.collider.GetComponent<HurtCollider>();

                if (hurtCollider != null)
                {
                    hurtCollider.NotifyTrigger(this, bulletEndPosition, hitInfo.normal);
                }
            }
        }
        else // Overlaping targets, we need to loop through all colliders and find the one closest to the position
        {
            Collider closestCollider = null;
            float closestDistance = Mathf.Infinity;

            foreach (var collider in colliders)
            {
                float distance = Vector3.Distance(bulletStartPosition, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }

            bulletEndPosition = bulletStartPosition;
            Vector3 normal = (transform.position - closestCollider.transform.position).normalized;
            closestCollider.GetComponent<HurtCollider>()?.NotifyTrigger(this, bulletEndPosition, normal);
        }

        Instantiate(bulletTrailPrefab)?.GetComponent<BulletTrail>()?.Init(bulletStartPosition, bulletEndPosition);
    }
}
