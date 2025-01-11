using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchingButton : MonoBehaviour
{
    private bool isSwitching = false;
    private TextMeshProUGUI buttonText;

    private void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (SceneManager.GetActiveScene().name == "TittleScene")
        {
            buttonText.text = "Start Game";
            return;
        }

        if (SceneManager.GetActiveScene().name == "DefeatScene")
        {
            buttonText.text = "Try Again";
            return;
        }

        if (SceneManager.GetActiveScene().name == "VictoryScene")
        {
            buttonText.text = "Main Menu";
        }
    }

    public void SwitchToNextLevel()
    {
        if (isSwitching) return;

        isSwitching = true;
        string nextLevel = LevelManager.Instance.GetNextLevel();

        if (nextLevel == null)
        {
            StartCoroutine(LoadSceneAsync("TittleScene"));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(nextLevel));
        }
    }

    public void RetryLevel()
    {
        if (isSwitching) return;

        isSwitching = true;
        StartCoroutine(LoadSceneAsync(LevelManager.Instance.CurrentScene));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Start loading the scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Prevent the scene from switching automatically
        asyncOperation.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        // Allow the scene to activate
        asyncOperation.allowSceneActivation = true;

        // Optional: Wait for activation to finish
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        isSwitching = false;
    }
}
