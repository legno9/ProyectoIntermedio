using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public string PreviousScene { get; private set; }
    public string CurrentScene { get; private set; }
    public string[] Levels; // List of level scene names in order
    private int currentLevelIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.Confined;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentScene(string sceneName)
    {
        PreviousScene = CurrentScene;
        CurrentScene = sceneName;

        // Update level index if it's in the level sequence
        currentLevelIndex = System.Array.IndexOf(Levels, CurrentScene);
    }

    public string GetNextLevel()
    {
        if (currentLevelIndex + 1 < Levels.Length)
        {
            return Levels[currentLevelIndex + 1];
        }

        return null; // No more levels
    }
}
