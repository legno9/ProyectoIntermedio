using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private string levelName;
    public UnityEvent OnLevelCleared;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out var characterController))
        {
            OnLevelCleared.Invoke();
            //SceneManager.LoadScene(levelName);
        }
    }
}
