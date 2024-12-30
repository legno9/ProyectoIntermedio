using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class EntityWeaponManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform weaponsParent;
    [SerializeField] private int startingWeaponIndex = -1;

    [Header("IK")]
    [SerializeField] private List<Rig> aimRigs = new();


    private Animator animator;
    private RuntimeAnimatorController originalAnimatorController;
    private int currentWeapon = -1;
    private WeaponBase[] weapons;

    private bool canReload = true;
    public UnityEvent<WeaponBase> OnWeaponSwitched;

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon != -1 ? weapons[currentWeapon] : null;
    }

    public bool GetCurrentWeaponIsRanged()
    {
        return currentWeapon != -1 ? weapons[currentWeapon] is WeaponRanged : false;
    }

    public float GetCurrentWeaponCooldown()
    {
        return currentWeapon != -1 ? weapons[currentWeapon].GetCurrentCooldown() : 1f;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        originalAnimatorController = animator.runtimeAnimatorController;
        weapons = weaponsParent.GetComponentsInChildren<WeaponBase>(true);

        foreach (WeaponBase weaponBase in weapons)
        {
            weaponBase.Init();
        }

        SelectWeapon(startingWeaponIndex);
    }

    private void OnEnable()
    {
        foreach (AnimationEventForwarder var in GetComponentsInChildren<AnimationEventForwarder>())
        {
            var.OnMeleeAttackEvent.AddListener(OnAttackEvent);
            var.OnReloadFinishedEvent.AddListener(OnReloadEvent);
        }
    }

    private void OnDisable()
    {
        foreach (AnimationEventForwarder var in GetComponentsInChildren<AnimationEventForwarder>())
        {
            var.OnMeleeAttackEvent.RemoveListener(OnAttackEvent);
        }
    }

    private void OnAttackEvent()
    {
        weaponsParent.GetComponentInChildren<WeaponMelee>().ActivateHitCollider();
    }

    private void OnReloadEvent()
    {
        if (currentWeapon != -1 && weapons[currentWeapon] is WeaponRanged)
        {
            ((WeaponRanged)weapons[currentWeapon]).Reload();
            canReload = true;
        }
    }

    public void RecoverWeaponAmmo(WeaponType weaponType, int amount)
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon is WeaponRanged && ((WeaponRanged)weapon).GetWeaponType() == weaponType)
            {
                ((WeaponRanged)weapon).RecoverAmmo(amount);
                break;
            }
        }
    }

    public bool PerformAttack()
    {
        if (currentWeapon != -1)
        {
            if (weapons[currentWeapon].PerformAttack())
            {
                animator.SetTrigger("Attack");
                canReload = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool PerformReload()
    {
        if (currentWeapon != -1)
        {
            if (weapons[currentWeapon] is WeaponRanged)
            {
                if (((WeaponRanged)weapons[currentWeapon]).CanReload())
                {
                    ((WeaponRanged)weapons[currentWeapon]).SetIsReloading(true);
                    animator.SetTrigger("Reload");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void PerformChangeToNextOrPrevWeapon(bool performNext)
    {
        int weaponToSet = currentWeapon;

        if (performNext)
        {
            weaponToSet++;

            if (weaponToSet >= weapons.Length)
            {
                weaponToSet = -1;
            }
        }
        else
        {
            weaponToSet--;

            if (weaponToSet < -1)
            {
                weaponToSet = weapons.Length - 1;
            }
        }

        if (weaponToSet != currentWeapon)
        {
            canReload = true;
            SelectWeapon(weaponToSet);
        }
    }

    private void SelectWeapon(int weaponToSet)
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Deselect(animator);

            if (weapons[currentWeapon] is WeaponRanged)
            {
                ((WeaponRanged)weapons[currentWeapon]).SetIsReloading(false);
            }
        }

        currentWeapon = weaponToSet;

        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Select(animator);

            if (weapons[currentWeapon] is WeaponRanged)
            {
                float attackSpeedMultiplier = ((WeaponRanged)weapons[currentWeapon]).GetAttacksPerSecond() * ((WeaponRanged)weapons[currentWeapon]).GetAttackAnimationLength();
                animator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier);
                animator.SetBool("HasRifleEquipped", true);
                animator.SetBool("HasKnifeEquipped", false);
            }
            else if (weapons[currentWeapon] is WeaponMelee)
            {
                animator.SetBool("HasRifleEquipped", false);
                animator.SetBool("HasKnifeEquipped", true);
            }

            OnWeaponSwitched?.Invoke(weapons[currentWeapon]);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
            animator.SetBool("HasRifleEquipped", false);
            animator.SetBool("HasKnifeEquipped", false);

            OnWeaponSwitched?.Invoke(null);
        }
    }
}
