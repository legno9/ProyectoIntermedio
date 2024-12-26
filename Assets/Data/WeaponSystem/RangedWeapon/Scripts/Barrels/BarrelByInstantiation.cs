using UnityEngine;

public class BarrelByInstantiation : BarrelBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileStartSpeed = 5;
    [SerializeField] private float projectileLifetime = 5;
    [SerializeField] private float projectileDamage = 5;

    public override void Shoot()
    {
        Quaternion spreadToApply = Quaternion.Euler(0, Random.Range(-(spread / 2), (spread / 2)), 0);
        GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, transform.rotation * spreadToApply);
        projectileInstance.GetComponent<Projectile>().Init(projectileStartSpeed, projectileLifetime, projectileDamage);
    }
}
