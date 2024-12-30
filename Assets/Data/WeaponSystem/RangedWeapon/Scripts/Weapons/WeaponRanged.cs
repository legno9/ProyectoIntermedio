using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : WeaponBase
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClipList shootSounds = new AudioClipList();
    [SerializeField] private AudioClipList reloadSounds = new AudioClipList();
    [SerializeField] protected GameObject muzzleFlash;

    //[Header("Debug")]
    //[SerializeField] private bool debugShoot;

    //[SerializeField] private bool debugStartShooting;
    //[SerializeField] private bool debugStopShooting;

    //[SerializeField] private bool debugShootBurst;
    //[SerializeField] private bool debugCancelBurst;

    private BarrelBase[] barrels;
    [SerializeField] private bool infiniteAmmo = false;
    [SerializeField] private int maxAmmo = 25;
    private int currentAmmo;
    [SerializeField] private int maxReserveAmmo = 100;
    private int currentReserveAmmo = 0;
    [SerializeField] private WeaponType weaponType = WeaponType.Rifle;
    private bool isReloading = false;
    public void SetIsReloading(bool value) { isReloading = value; }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public bool CanReload()
    {
        return currentReserveAmmo > 0 && currentAmmo < maxAmmo && !isReloading;
    }

    private void OnValidate()
    {
        //if (debugShoot)
        //{
        //    debugShoot = false;
        //    Shoot();
        //}
    }

    public override void Init()
    {
        base.Init();
        barrels = GetComponentsInChildren<BarrelBase>();
        currentAmmo = maxAmmo;
        currentReserveAmmo = maxReserveAmmo;
        lastAttackTime = Time.time - 1f / attacksPerSecond;
    }

    private void Shoot()
    {
        Instantiate(muzzleFlash, barrels[0].transform.position, barrels[0].transform.rotation, barrels[0].transform);

        foreach (BarrelBase barrel in barrels)
        {
            barrel.Shoot();
        }

        if (!infiniteAmmo){ currentAmmo--; }
        shootSounds.PlayAtPointRandom(transform.position);
    }

    public override bool PerformAttack()
    {
        if (Time.time - lastAttackTime > 1f / attacksPerSecond && currentAmmo > 0)
        {
            lastAttackTime = Time.time;
            Shoot();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reload()
    {
        int ammoRecovered = Mathf.Min(maxAmmo - currentAmmo, currentReserveAmmo);
        currentReserveAmmo -= ammoRecovered;
        currentAmmo += ammoRecovered;
        reloadSounds.PlayAtPointRandom(transform.position);
        isReloading = false;
    }

    public void RecoverAmmo(int ammo)
    {
        currentReserveAmmo = Mathf.Min(currentReserveAmmo + ammo, maxReserveAmmo);
    }
}

public enum WeaponType
{
    Rifle,
    Shotgun,
    GrenadeLauncher
}
