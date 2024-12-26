using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    //[SerializeField] private AudioClipList cantAttackSounds;
    //[SerializeField] private AudioClipList switchWeaponsSounds;
    private bool canPerformWrongSound = true;
    private EntityWeaponManager weaponManager;
    private bool attacking = false;

    private void Awake()
    {
        weaponManager = GetComponent<EntityWeaponManager>();
    }

    private void Update()
    {
        if (attacking)
        {
            if (!weaponManager.PerformAttack() && canPerformWrongSound)
            {
                canPerformWrongSound = false;
                //cantAttackSounds.PlayAtPointRandom(transform.position);
            }
            else
            {
                canPerformWrongSound = false;
            }
        }
    }

    private void OnAttack(InputValue value)
    {
        if (attacking == false && value.Get<float>() == 1f)
        {
            canPerformWrongSound = true;
        }

        if (value.Get<float>() == 1f)
        {
            attacking = true;
        }
        else
        {
            attacking = false;
        }
    }

    private void OnNextPrevWeapon(InputValue value)
    {
        Vector2 readValue = value.Get<Vector2>();
        bool mustSelectNextWeapon = readValue.y > 0;

        //switchWeaponsSounds.PlayAtPointRandom(transform.position);
        weaponManager.PerformChangeToNextOrPrevWeapon(mustSelectNextWeapon);
    }
}
