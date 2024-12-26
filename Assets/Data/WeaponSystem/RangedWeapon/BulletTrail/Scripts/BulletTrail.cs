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

    private void Update()
    {
        if (lineRenderer.widthMultiplier == 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        Vector3[] positions = new Vector3[10];

        for (int i = 0; i < N_POSITIONS; i++)
        {
            float t = (float)i / (float)N_POSITIONS;
            positions[i] = Vector3.Lerp(startPos, endPos, t);
        }

        lineRenderer.SetPositions(positions);

        DOTween.To(
            () => lineRenderer.widthMultiplier,
            (x) => lineRenderer.widthMultiplier = x,
            0f,
            trailDuration
        );
    }
}
