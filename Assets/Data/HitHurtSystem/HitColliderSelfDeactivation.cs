using UnityEngine;
using DG.Tweening;

public class HitColliderSelfDeactivation : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    Tween selfDeactivation = null;
    private void OnEnable()
    {
        selfDeactivation = DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        if (selfDeactivation != null)
        {
            selfDeactivation.Kill();
            selfDeactivation = null;
        }
    }
}
