using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "ScriptableObjects/AudioDataSO", order = 1)]
public class AudioDataSO : ScriptableObject
{
    public enum AudioType
    {
        SFX,
        Music
    }

    [Header("Identification")]
    public string id = "audioClip";

    [Header("Type")]
    public AudioType audioType;

    [Header("Clips")]
    public AudioClip[] audioClips;

    [Header("Playback")]
    public bool loopByDefault;
    public bool playRandom;
    public float volumeMulti = 1f;

    public float pitchMin = 1f;
    public float pitchMax = 1f;

    public AudioMixerGroup overrideMixer;

    [Header("Gameplay")]
    public bool followEnemy = false;
    public bool alertEnemyOnPlay = true;
    public float alertRadius = 16f;
    public bool alertWhenHunting = false;
    [Range(0, 10)]
    public int alertPriority = 1;

    [HideInInspector]
    public int lastPlayed = -1;
    public float GetRandomPitch()
    {
        float min = Mathf.Min(pitchMin, pitchMax);
        float max = Mathf.Max(pitchMin, pitchMax);
        return Random.Range(min, max);
    }
    public AudioClip GetRandomClip()
    {
        if (audioClips == null || audioClips.Length == 0)
            return null;

        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
