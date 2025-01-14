using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChosenUI : MonoBehaviour
{
    [SerializeField] private EntityWeaponManager weaponManager;
    [SerializeField] private UDictionary<WeaponBase, Sprite> weaponIcons;
    [SerializeField] private Image weaponIcon;

    private void OnEnable()
    {
        weaponManager.OnWeaponSwitched.AddListener(OnWeaponSwitched);
    }

    private void OnDisable()
    {
        weaponManager.OnWeaponSwitched.RemoveListener(OnWeaponSwitched);
    }

    private void OnWeaponSwitched(WeaponBase weapon)
    {
        if (weapon == null)
        {
            weaponIcon.sprite = null;
            weaponIcon.gameObject.SetActive(false);
        }
        else
        {
            weaponIcon.sprite = weaponIcons[weapon];
            weaponIcon.gameObject.SetActive(true);
        }
    }
}
