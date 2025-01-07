using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class PlayerAmmoUI : MonoBehaviour
{
    [SerializeField] private EntityWeaponManager _entityWeaponManager;
    [SerializeField] private TextMeshProUGUI _text;

    private void Update()
    {
        (float currentAmmo, float maxAmmo, float ammoInReserve) = _entityWeaponManager.GetCurrentWeaponAmmo();
        _text.text = currentAmmo.ToString() + "/" + maxAmmo.ToString() + " | " + ammoInReserve.ToString();
    }
}
