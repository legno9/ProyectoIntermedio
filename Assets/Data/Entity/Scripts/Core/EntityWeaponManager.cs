using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.Rendering.DebugUI;

public class EntityWeaponManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform weaponsParent;
    [SerializeField] private int startingWeaponIndex = -1;

    [Header("IK")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private float idleOffset = 0f;
    [SerializeField] private float inCombatStanceOffset = 39f;
    [SerializeField] private float meleeAttackOffset = -10f;
    [SerializeField] private float meleeOffsetDelay = 0.75f;

    private Animator animator;

    private RuntimeAnimatorController originalAnimatorController;
    private int currentWeapon = -1;

    WeaponBase[] weapons;

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon != -1 ? weapons[currentWeapon] : null;
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

    public bool PerformAttack()
    {
        if (currentWeapon != -1)
        {
            if (weapons[currentWeapon].PerformAttack())
            {
                animator.SetTrigger("Attack");
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
            SelectWeapon(weaponToSet);
        }
    }

    private void SelectWeapon(int weaponToSet)
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Deselect(animator);
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
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
            animator.SetBool("HasRifleEquipped", false);
            animator.SetBool("HasKnifeEquipped", false);
        }

        AnimateAimRigWeight();
    }

    private void AnimateAimRigWeight()
    {
        float offset = 0;

        if (currentWeapon != -1)
        {
            if (animator.GetBool("IsInCombatStance"))
            {
                offset = inCombatStanceOffset;
            }
            else
            {
                offset = idleOffset;
            }
        }

        DOTween.To(
            () => aimRig.GetComponentInChildren<MultiAimConstraint>().data.offset,
            (x) => aimRig.GetComponentInChildren<MultiAimConstraint>().data.offset = x,
            Vector3.up * offset,
            0.25f
        );

        DOTween.To(
            () => aimRig.weight,
            (x) => aimRig.weight = x,
            (currentWeapon != -1) ? 1f : 0f,
            0.25f
        );
    }
}
