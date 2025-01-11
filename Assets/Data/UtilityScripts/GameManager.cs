using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }
    public int CurrentScore { get; private set; }
    public float GameTime { get; private set; }

    [SerializeField] private bool pauseOnAwake = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (pauseOnAwake)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

        CurrentScore = 0;
        GameTime = 0f;
    }

    private void Update()
    {
        if (!IsPaused)
        {
            GameTime += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartGame()
    {
        InitializeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
