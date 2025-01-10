using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private EntityHealth entityLife;
    [SerializeField] private Image greenHealthImage;
    [SerializeField] private Image redHealthImage;
    [SerializeField] private float redHealthDelay = 0.25f;
    [SerializeField] private float redHealthDuration = 0.5f;
    private Tween redHealthTween;

    private void OnEnable()
    {
        entityLife.OnHealthChanged.AddListener(UpdateHealthUI);
    }

    private void OnDisable()
    {
        entityLife.OnHealthChanged.RemoveListener(UpdateHealthUI);
    }

    private void UpdateHealthUI(float currentHealth, float damage)
    {
        redHealthTween.Kill();

        redHealthImage.fillAmount = greenHealthImage.fillAmount;
        greenHealthImage.fillAmount = entityLife.GetCurrentHealthPercentage();

        redHealthTween = DOTween.To(
            () => redHealthImage.fillAmount,
            (x) => redHealthImage.fillAmount = x,
            greenHealthImage.fillAmount,
            redHealthDuration
        ).SetDelay(redHealthDelay);
    }
}
