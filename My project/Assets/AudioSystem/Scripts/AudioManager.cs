using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    Dictionary<string, AudioDataSO> audioLookup = new Dictionary<string, AudioDataSO>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAudio();
    }

    void LoadAudio()
    {
        AudioDataSO[] audioData = Resources.LoadAll<AudioDataSO>("AudioSOs");

        foreach (var data in audioData)
        {
            audioLookup[data.id] = data;
        }
    }

    public AudioDataSO GetAudioData(string id)
    {
        if (audioLookup.TryGetValue(id, out var audioData))
        {
            return audioData;
        }
        else
        {
            Debug.LogWarning($"Audio ID '{id}' not found!");
            return null;
        }
    }

    public void PlaySound(string id, Vector3? pos, bool? loopOverride)
    {
        if (!audioLookup.TryGetValue(id, out AudioDataSO data))
        {
            Debug.LogWarning($"Audio {id} not found");
            return;
        }

        bool loop = loopOverride ?? data.loopByDefault;

        if (data.audioClips == null || data.audioClips.Length == 0)
        {
            Debug.LogWarning($"Audio {id} has no clips assigned");
            return;
        }

        int sequential = data.lastPlayed + 1;
        if (sequential >= data.audioClips.Length)
        {
            data.lastPlayed = -1;
            sequential = 0;
        }

        AudioClip clip = data.playRandom ? data.GetRandomClip() : data.audioClips[sequential];

        if (clip == null)
            return;

        if (pos == null)
        {
            GameObject audioObj = new GameObject($"Audio_{id}");
            audioObj.transform.parent = transform;

            AudioSource source = audioObj.AddComponent<AudioSource>();

            source.clip = clip;
            source.loop = loop;
            source.pitch = Random.Range(data.pitchMin, data.pitchMax);
            source.volume = data.volumeMulti;

            source.Play();

            if (!loop)
            {
                Destroy(audioObj, clip.length / source.pitch);
            }
        }
        else
        {
            Vector3 position = pos.Value;
            //loop
            //pitch
            AudioSource.PlayClipAtPoint(clip, position, data.volumeMulti);
        }
    }

    public void PlayWorldSound(string id, bool? loopOverride)
    {
    }
}
