using UnityEngine;

public class Medikit : MonoBehaviour
{
    [SerializeField] private int amount = 10;
    [SerializeField] private AudioClipList pickupSounds = new AudioClipList();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<EntityHealth>().RecoverHealth(amount);
            pickupSounds.PlayAtPointRandom(other.transform.position);
            Destroy(gameObject);
        }
    }
}
