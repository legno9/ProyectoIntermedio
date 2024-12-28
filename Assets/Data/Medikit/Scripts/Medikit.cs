using UnityEngine;

public class Medikit : MonoBehaviour
{
    [SerializeField] private int amount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<EntityHealth>().RecoverHealth(amount);
            Destroy(gameObject);
        }
    }
}
