using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AudioClipList switchWeaponsSounds;
    [SerializeField] private AudioClipList cantReloadSounds;
    private EntityWeaponManager weaponManager;
    private bool attacking = false;
    private bool isPrimaryAttack = false;

    private void Awake()
    {
        weaponManager = GetComponent<EntityWeaponManager>();
    }

    private void Update()
    {
        if (attacking)
        {
            weaponManager.PerformAttack(isPrimaryAttack);
        }
    }

    private void OnAttack(InputValue value)
    {
        if (value.Get<float>() == 1f)
        {
            if (CheckMeleeAttack())
            {
                isPrimaryAttack = true;
                weaponManager.PerformAttack(isPrimaryAttack);
                return;
            }
            
            attacking = true;
        }
        else
        {
            attacking = false;
        }
    }

    private void OnSecondaryAttack(InputValue value)
    {
        if (CheckMeleeAttack())
        {
            isPrimaryAttack = false;
            weaponManager.PerformAttack(isPrimaryAttack);
            return;
        }
    }

    private bool CheckMeleeAttack()
    {
        return !weaponManager.GetCurrentWeaponIsRanged();
    }

    private void OnReload(InputValue value)
    {
        if (value.Get<float>() == 1f)
        {
            if (!CheckMeleeAttack())
            {
                if (!weaponManager.PerformReload())
                {
                    cantReloadSounds.PlayAtPointRandom(transform.position);
                }
            }
        }
    }

    private void OnNextPrevWeapon(InputValue value)
    {
        if (!attacking && weaponManager.CanChangeWeapon())
        {
            Vector2 readValue = value.Get<Vector2>();
            bool mustSelectNextWeapon = readValue.y > 0;

            switchWeaponsSounds.PlayAtPointRandom(transform.position);
            weaponManager.PerformChangeToNextOrPrevWeapon(mustSelectNextWeapon);
        }
    }
}
