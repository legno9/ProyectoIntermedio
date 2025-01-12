using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private AudioClipList switchWeaponsSounds;
    [SerializeField] private AudioClipList cantReloadSounds;
    private EntityWeaponManager weaponManager;
    private bool attacking = false;
    private bool isLigtAttack = false;

    private void Awake()
    {
        weaponManager = GetComponent<EntityWeaponManager>();
    }

    private void Update()
    {
        if (attacking)
        {
            weaponManager.PerformAttack(isLigtAttack);
        }
    }

    private void OnAttack(InputValue value)
    {
        if (value.Get<float>() == 1f)
        {
            if (CheckMeleeAttack())
            {
                isLigtAttack = true;
                weaponManager.PerformAttack(isLigtAttack);
                return;
            }
            
            attacking = true;
        }
        else
        {
            attacking = false;
        }
    }

    private void OnHeavyAttack(InputValue value)
    {
        if (CheckMeleeAttack())
        {
            isLigtAttack = false;
            weaponManager.PerformAttack(isLigtAttack);
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
            if (!weaponManager.PerformReload())
            {
                cantReloadSounds.PlayAtPointRandom(transform.position);
            }
        }
    }

    private void OnNextPrevWeapon(InputValue value)
    {
        Vector2 readValue = value.Get<Vector2>();
        bool mustSelectNextWeapon = readValue.y > 0;

        switchWeaponsSounds.PlayAtPointRandom(transform.position);
        weaponManager.PerformChangeToNextOrPrevWeapon(mustSelectNextWeapon);
    }
}
