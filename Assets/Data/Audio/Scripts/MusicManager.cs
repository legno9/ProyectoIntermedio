using UnityEngine;
using UnityEngine.Playables;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;
    private AudioSource musicSource;

    [SerializeField] private EntityHealth player;
    //[SerializeField] private VictoryZone victoryZone;
    [SerializeField] private PlayableDirector defeatDirector;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        player.OnDeath.AddListener(PlayDefeatMusic);
        //victoryZone.OnPlayerClearedLevel.AddListener(PlayVictoryMusic);
    }

    private void OnDisable()
    {
        player.OnDeath.RemoveListener(PlayDefeatMusic);
        //victoryZone.OnPlayerClearedLevel.RemoveListener(PlayVictoryMusic);
    }

    private void PlayVictoryMusic()
    {
        musicSource.Stop();
        musicSource.clip = victoryMusic;
        musicSource.Play();
    }

    private void PlayDefeatMusic()
    {
        musicSource.Stop();
        musicSource.clip = defeatMusic;
        musicSource.Play();

        defeatDirector.Play();
    }
}
