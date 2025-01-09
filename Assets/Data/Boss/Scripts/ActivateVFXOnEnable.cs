using UnityEngine;

public class ActivateVFXOnEnable : MonoBehaviour
{
    [SerializeField] private HitColliderSelfDeactivation hitCollider;
    private ParticleSystem vfx;

    private void Awake()
    {
        vfx = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        hitCollider.OnActivate.AddListener(Activate);
    }

    private void OnDisable()
    {
        hitCollider.OnActivate.RemoveListener(Activate);
    }

    private void Activate()
    {
        vfx.Play();
    }
}
