using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class HitColliderSelfDeactivation : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    public UnityEvent OnActivate;

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

    public void Activate(float duration)
    {
        selfDeactivation.Kill();
        selfDeactivation = DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
        gameObject.SetActive(true);
        OnActivate?.Invoke();
    }

    public void Activate()
    {
        selfDeactivation.Kill();
        selfDeactivation = DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
        gameObject.SetActive(true);
        OnActivate?.Invoke();
    }
}
