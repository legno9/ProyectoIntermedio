using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float soundRadius = 10f;  // Rango de escucha

    // Este método será llamado para emitir el sonido
    public void EmitSound(Vector3 position)
    {
        // Simula la propagación del sonido
        Collider[] colliders = Physics.OverlapSphere(position, soundRadius);

        foreach (var collider in colliders)
        {
            // Si el collider tiene un componente AIListener, se activa la escucha
            HeardDetector listener = collider.GetComponent<HeardDetector>();
            listener?.HearSound(position, gameObject);
        }
    }
}
