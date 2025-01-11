using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutscenesManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector introCutscene;
    [SerializeField] private PlayableDirector victoryCutscene;
    [SerializeField] private PlayableDirector defeatCutscene;

    [SerializeField] private PlayerDeath playerDeath;
    [SerializeField] private BossController bossController;
    [SerializeField] private LoadLevel loadLevel;

    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private PlayerInput playerInput;

    private void OnEnable()
    {
        playerDeath.OnPlayerDeath.AddListener(defeatCutscene.Play);

        if (loadLevel != null)
        {
            loadLevel.OnLevelCleared.AddListener(victoryCutscene.Play);
        }
        else
        {
            bossController.OnKilled.AddListener(victoryCutscene.Play);
        }
    }

    private void OnDisable()
    {
        playerDeath.OnPlayerDeath.RemoveListener(defeatCutscene.Play);

        if (loadLevel != null)
        {
            loadLevel.OnLevelCleared.RemoveListener(victoryCutscene.Play);
        }
        else
        {
            bossController.OnKilled.RemoveListener(victoryCutscene.Play);
        }
    }

    public void DisablePlayerInput()
    {
        inputActionAsset.Disable();
        playerInput.enabled = false;
    }

    public void EnablePlayerInput()
    {
        inputActionAsset.Enable();
        playerInput.enabled = true;
    }

    public void SwitchToNextLevel()
    {
        string nextLevel = LevelManager.Instance.GetNextLevel();

        if (nextLevel == null)
        {
            StartCoroutine(LoadSceneAsync("VictoryScene"));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(nextLevel));
        }
    }

    public void GoToDefeatScene()
    {
        StartCoroutine(LoadSceneAsync("DefeatScene"));
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
    }
}
