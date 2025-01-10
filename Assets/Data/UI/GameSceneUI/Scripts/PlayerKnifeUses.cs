using UnityEngine;

public class PlayerKnifeUses : MonoBehaviour
{
    [SerializeField] private WeaponMelee_WithLimitedUses knife;

    private void Update()
    {
        int uses = 3 - knife.GetCurrentUses();
        for (int i = 0; i < uses; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = uses; i < 3; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
