using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private int amount = 50;
    [SerializeField] private AudioClipList pickupSounds = new AudioClipList();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<EntityWeaponManager>().RecoverWeaponAmmo(weaponType, amount);
            pickupSounds.PlayAtPointRandom(other.transform.position);
            Destroy(gameObject);
        }
    }
}
