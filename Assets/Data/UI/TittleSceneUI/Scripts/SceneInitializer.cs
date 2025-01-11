using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance?.SetCurrentScene(SceneManager.GetActiveScene().name);
    }
}