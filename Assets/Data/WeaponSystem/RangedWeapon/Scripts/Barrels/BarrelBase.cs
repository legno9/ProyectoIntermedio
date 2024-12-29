using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugShoot;

    [Header("Barrel Properties")]
    [SerializeField] protected float spread;
    [SerializeField] protected Transform aimTarget;

    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            Shoot();
        }
    }

    public abstract void Shoot();
}
