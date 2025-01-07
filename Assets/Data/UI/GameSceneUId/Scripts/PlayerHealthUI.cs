using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private EntityHealth entityLife;
    private Image healthImage;

    private void Awake()
    {
        healthImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        healthImage.fillAmount = entityLife.GetCurrentHealthPercentage();
    }
}
