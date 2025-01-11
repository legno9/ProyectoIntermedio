using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;
    private SoundEmitter soundEmitter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        soundEmitter = GetComponent<SoundEmitter>();
    }

    public void PlaySoundFX(AudioClip audioClip, Vector3 spawnPosition, float volume, float pitch, bool isFromPlayer, SoundRadius soundRadius)
    {
        if (isFromPlayer)
        {
            soundEmitter.EmitSound(spawnPosition, true, soundRadius);
        }

        float clampedVolume = Mathf.Clamp01(volume);
        float clampedPitch = Mathf.Clamp(pitch, -3, 3);

        AudioSource audioSource = Instantiate(soundFXObject, spawnPosition, Quaternion.identity, gameObject.transform);
        audioSource.clip = audioClip;
        audioSource.volume = clampedVolume;
        audioSource.pitch = clampedPitch;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource, clipLength);
    }
}

//[Serializable]
//public class AudioClipPlus
//{
//    public AudioClip audioClip;
//    [Range(0,1)] public float volume = 1;
//    [Range(-3,3)] public float pitch = 1;

//    public void PlayAtPoint(Vector3 position)
//    {
//        SoundFXManager.Instance.PlaySoundFX(audioClip, position, volume, pitch);
//    }
//}

[System.Serializable]
public class AudioClipList
{
    [Range(0, 1)] public float volume = 1;
    [Range(-3, 3)] public float pitch = 1;
    [FormerlySerializedAs("isFromPlayer")]
    [Tooltip("If true, the sound will be heard by AI listeners")]
    [SerializeField] private bool canAlertEnemies = false;
    [SerializeField] private SoundRadius soundRadius = SoundRadius.small;
    public List<AudioClip> audioClipList = new();
    // anadir manager sonidos y meterle el sound emitter
    public void PlayAtPointRandom(Vector3 position)
    {
        if (audioClipList.Count == 0) return;
        SoundFXManager.Instance.PlaySoundFX(audioClipList[Random.Range(0, audioClipList.Count)], position, volume, pitch, canAlertEnemies, soundRadius);
    }

    public void PlayAtPoint(int audioClipIndex, Vector3 position)
    {
        int clampedIndex = Mathf.Clamp(audioClipIndex, 0, audioClipList.Count - 1);
        SoundFXManager.Instance.PlaySoundFX(audioClipList[audioClipIndex], position, volume, pitch, canAlertEnemies, soundRadius);
    }
}
