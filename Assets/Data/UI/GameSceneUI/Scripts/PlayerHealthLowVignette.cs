using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthLowVignette : MonoBehaviour
{
    [SerializeField] float maxIntensity = 0.5f;
    [SerializeField] float vignetteThreshold = 0.5f;
    [SerializeField] private EntityHealth player;
    private Vignette vignette;

    private void Awake()
    {
        if (FindFirstObjectByType<Volume>().profile.TryGet(out vignette))
        {
            vignette.intensity.Override(0f);
        }
    }

    private void Update()
    {
        float vignetteIntensity = Mathf.Max(0, (-vignetteThreshold + (1 - player.GetCurrentHealthPercentage()))) * (2 * maxIntensity);
        vignette.intensity.Override(vignetteIntensity);
    }
}
