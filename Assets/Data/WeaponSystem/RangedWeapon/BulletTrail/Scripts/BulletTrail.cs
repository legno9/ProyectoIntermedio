using UnityEngine;
using DG.Tweening;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private float trailDuration = 0.25f;
    private LineRenderer lineRenderer;

    private const int N_POSITIONS = 10;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        // Crear posiciones interpoladas
        Vector3[] positions = new Vector3[N_POSITIONS];

        for (int i = 0; i < N_POSITIONS; i++)
        {
            float t = (float)i / (float)(N_POSITIONS - 1); // Asegurar que t alcance 1.0
            positions[i] = Vector3.Lerp(startPos, endPos, t);
        }

        lineRenderer.SetPositions(positions);

        // Tween para reducir el ancho de la línea
        DOTween.To(
            () => lineRenderer.widthMultiplier,
            (x) => lineRenderer.widthMultiplier = x,
            0f,
            trailDuration
        ).OnComplete(() =>
        {
            // Destruir el objeto cuando el tween termine
            Destroy(gameObject);
        });
    }
}
