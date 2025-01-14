using UnityEngine;

public enum SoundRadius
{
    small = 5,
    medium = 7,
    large = 15
}

public class SoundEmitter : MonoBehaviour
{
    public void EmitSound(Vector3 position, bool isFromPlayer, SoundRadius soundRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, (int) soundRadius);

        foreach (var collider in colliders)
        {
            HeardDetector listener = collider.GetComponent<HeardDetector>();
            listener?.HearSound(position, gameObject, isFromPlayer);
        }
    }
}
