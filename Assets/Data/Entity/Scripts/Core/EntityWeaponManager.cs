using DG.Tweening;
using System.Collections;
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

    [Header("Melee")]
    [Tooltip("If true, the player will reset the attack combo when switching between light and heavy attacks")]
    [SerializeField]bool resetAttackCombo = false;

    private struct AttackData
    {
        public WeaponMelee weapon;
        public bool isLightAttack;
        public int maxCombo;
    }

    private PlayerMeleeAnimation playerMeleeAnimation;
    private Coroutine resetCombo;
    private bool isAttacking = false;
    private bool attackBuffered = false;
    private int comboCount = 0;
    private bool currentAttackTypeIsLight;
    private AttackData currentAttack = new();
    private AttackData bufferedAttack = new();
    private Animator animator;
    private RuntimeAnimatorController originalAnimatorController;
    private int currentWeapon = -1;
    private WeaponBase[] weapons;
    // private bool canReload = true;
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

    public (float, float, float) GetCurrentWeaponAmmo()
    {
        (float currentAmmo, float maxAmmo, float ammoInReserve) = (0f, 0f, 0f);

        if (currentWeapon != -1 && weapons[currentWeapon] is WeaponRanged)
        {
            currentAmmo = ((WeaponRanged)weapons[currentWeapon]).GetCurrentAmmo();
            maxAmmo = ((WeaponRanged)weapons[currentWeapon]).GetMaxAmmo();
            ammoInReserve = ((WeaponRanged)weapons[currentWeapon]).GetCurrentReserveAmmo();
        }

        return (currentAmmo, maxAmmo, ammoInReserve);
    }

    private void Awake()
    {
        playerMeleeAnimation = GetComponent<PlayerMeleeAnimation>();
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
        playerMeleeAnimation?.OnAttackAnimationComplete.AddListener(OnAttackEnd);
        foreach (AnimationEventForwarder var in GetComponentsInChildren<AnimationEventForwarder>())
        {
            // var.OnMeleeAttackEvent.AddListener(OnAttackEvent);
            var.OnReloadFinishedEvent.AddListener(OnReloadEvent);
        }
    }

    private void OnDisable()
    {
        playerMeleeAnimation?.OnAttackAnimationComplete.AddListener(OnAttackEnd);

        foreach (AnimationEventForwarder var in GetComponentsInChildren<AnimationEventForwarder>())
        {
            // var.OnMeleeAttackEvent.RemoveListener(OnAttackEvent);
            var.OnReloadFinishedEvent.AddListener(OnReloadEvent);
        }
    }

    // private void OnAttackEvent()
    // {
    //     weaponsParent.GetComponentInChildren<WeaponMelee>().ActivateHitCollider();
    // }

    private void OnReloadEvent()
    {
        if (currentWeapon != -1 && weapons[currentWeapon] is WeaponRanged)
        {
            ((WeaponRanged)weapons[currentWeapon]).Reload();
            // canReload = true;
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

    public bool PerformAttack(bool isLightAttack)
    {
        if (currentWeapon != -1)
        {
            if (weapons[currentWeapon] is WeaponMelee)
            {
                if (!isAttacking)
                {
                    CreateAttack(ref currentAttack, isLightAttack);
                    currentAttackTypeIsLight = isLightAttack;
                    ExecuteAttack();
                }
                else if (!attackBuffered && playerMeleeAnimation.CanBufferAttack())
                {
                    CreateAttack(ref bufferedAttack, isLightAttack);
                    attackBuffered = true;
                }

                return true;
            }
            if (weapons[currentWeapon].PerformAttack() && weapons[currentWeapon] is WeaponRanged)
            {
                animator.SetTrigger("Attack");
                // canReload = false;
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
            // canReload = true;
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
                if (DoesParameterExist(animator, "HasKatanaEquipped"))
                {
                    animator.SetBool("HasKatanaEquipped", false);
                }
            }
            else if (weapons[currentWeapon] is WeaponMelee)
            {
                playerMeleeAnimation.WeaponChanged(weapons[currentWeapon].GetOverrideController());
                animator.SetBool("HasRifleEquipped", false);
                if (DoesParameterExist(animator, "HasKatanaEquipped"))
                {
                    animator.SetBool("HasKatanaEquipped", true);
                }
            }

            OnWeaponSwitched?.Invoke(weapons[currentWeapon]);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
            animator.SetBool("HasRifleEquipped", false);
            if (DoesParameterExist(animator, "HasKatanaEquipped"))
            {
                animator.SetBool("HasKatanaEquipped", false);
            }

            OnWeaponSwitched?.Invoke(null);
        }
    }

    private bool DoesParameterExist(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }

#region MeleeCombat    
    private void CreateAttack(ref AttackData attack, bool isLightAttack)
    {
        WeaponMelee weapon = (WeaponMelee)weapons[currentWeapon];
        attack.weapon = weapon;
        attack.isLightAttack = isLightAttack;
        attack.maxCombo = isLightAttack ? weapon.LightComboCount() : weapon.HeavyComboCount();
    }

    private void ExecuteAttack()
    {   
        if (currentAttack.maxCombo == 0){return;}

        isAttacking = true;

        if (resetAttackCombo && currentAttackTypeIsLight != currentAttack.isLightAttack)
        {
            comboCount = 0;
        }
        else
        {
            comboCount = comboCount >= currentAttack.maxCombo ? 0 : comboCount;
        }
        
        playerMeleeAnimation.StartAttackAnimation(currentAttack.weapon, comboCount, currentAttack.isLightAttack);
        currentAttack.weapon.Attacking(isAttacking);

        currentAttackTypeIsLight = currentAttack.isLightAttack;
        comboCount++;
        
        if (resetCombo != null)
        {
            StopCoroutine(resetCombo);
            resetCombo = null;
        }
    }

    private void OnAttackEnd()
    {   
        isAttacking = false;
        currentAttack.weapon.Attacking(isAttacking);

        
        if (attackBuffered)
        {
            currentAttack = bufferedAttack;
            attackBuffered = false;
            ExecuteAttack();
            return;
        }

        resetCombo = StartCoroutine(ResetComboAfterDelay());
        playerMeleeAnimation.AttackSequenceEnded();
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        comboCount = 0;
    }

#endregion MeleeCombat
}
