using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class PlayerAmmoUI : MonoBehaviour
{
    [SerializeField] private EntityWeaponManager entityWeaponManager;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI reserveAmmoText;

    private void Update()
    {
        (float currentAmmo, float maxAmmo, float ammoInReserve) = entityWeaponManager.GetCurrentWeaponAmmo();
        currentAmmoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
        reserveAmmoText.text = ammoInReserve.ToString();
    }
}
