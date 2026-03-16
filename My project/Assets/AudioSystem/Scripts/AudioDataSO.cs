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
    public bool alertEnemyOnPlay = true;
    public float alertRadius = 16f;

    [HideInInspector]
    public int lastPlayed = -1;
    public float GetRandomPitch()
    {
        return Random.Range(pitchMin, pitchMax);
    }
    public AudioClip GetRandomClip()
    {
        if (audioClips == null || audioClips.Length == 0)
            return null;

        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
