using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float soundRadius = 10f;  // Rango de escucha

    public void EmitSound(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, soundRadius);

        foreach (var collider in colliders)
        {
            HeardDetector listener = collider.GetComponent<HeardDetector>();
            listener?.HearSound(position, gameObject);
        }
    }
}
