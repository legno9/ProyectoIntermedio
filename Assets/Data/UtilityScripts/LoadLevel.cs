using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out var characterController))
        {
            GameManager.Instance.LoadScene(levelIndex);
        }
    }
}
