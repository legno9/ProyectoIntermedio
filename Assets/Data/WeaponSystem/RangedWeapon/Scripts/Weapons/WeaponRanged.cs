using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged : WeaponBase
{
    [Header("Sound Settings")]
    //[SerializeField] private AudioClipList shootSounds = new AudioClipList();
    [SerializeField] protected GameObject muzzleFlash;

    //[Header("Debug")]
    //[SerializeField] private bool debugShoot;

    //[SerializeField] private bool debugStartShooting;
    //[SerializeField] private bool debugStopShooting;

    //[SerializeField] private bool debugShootBurst;
    //[SerializeField] private bool debugCancelBurst;

    private BarrelBase[] barrels;

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
        lastAttackTime = Time.time - 1f / attacksPerSecond;
    }

    private void Shoot()
    {
        Instantiate(muzzleFlash, barrels[0].transform.position, barrels[0].transform.rotation, barrels[0].transform);

        foreach (BarrelBase barrel in barrels)
        {
            barrel.Shoot();
        }

        //shootSounds.PlayAtPointRandom(transform.position);
    }

    public override bool PerformAttack()
    {
        if (Time.time - lastAttackTime > 1f / attacksPerSecond)
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
}
