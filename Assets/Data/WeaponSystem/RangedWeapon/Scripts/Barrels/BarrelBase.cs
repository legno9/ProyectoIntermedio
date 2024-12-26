using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugShoot;

    [Header("Barrel Properties")]
    [SerializeField] protected float spread;

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
