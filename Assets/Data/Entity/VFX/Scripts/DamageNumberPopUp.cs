using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumberPopUp : MonoBehaviour
{
    private Canvas canvas;
    private TextMeshProUGUI textMesh;
    private Camera mainCamera;

    [SerializeField] private float initialVerticalVelocity = 1f;
    private float verticalVelocity = 0f;
    [SerializeField] private float deceleration = 0.1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    private float currentLifetime = 0f;
    private Tween fadeOutTween;


    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        mainCamera = Camera.main;
        verticalVelocity = initialVerticalVelocity;
    }

    public void Initialize(float damage)
    {
        textMesh.text = damage.ToString();
    }

    private void Update()
    {
        // Movement
        verticalVelocity -= deceleration * Time.deltaTime;
        transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);

        // Rotation
        canvas.transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        
        // Scale
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        float scaleFactor = distanceToCamera;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Lifetime
        currentLifetime += Time.deltaTime;

        if (currentLifetime > lifetime && fadeOutTween == null)
        {
            fadeOutTween = textMesh.DOFade(0f, fadeOutDuration)
            .OnComplete(() =>
            {
                textMesh.gameObject.SetActive(false);
            });
        }
    }
}
