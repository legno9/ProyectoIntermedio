using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int amount = 50;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<EntityWeaponManager>().RecoverWeaponAmmo(weaponType, amount);
            Destroy(gameObject);
        }
    }
}
