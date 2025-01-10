using UnityEngine;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    [SerializeField] private EntityWeaponManager weaponManager;
    private Image cooldownImage;

    private void Awake()
    {
        cooldownImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        cooldownImage.fillAmount = 1 - weaponManager.GetCurrentWeaponCooldown();
    }
}
