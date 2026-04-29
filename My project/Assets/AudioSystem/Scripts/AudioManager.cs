using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioDataSO> playOnWake = new List<AudioDataSO>();
    public static AudioManager Instance { get; private set; }
    Dictionary<string, AudioDataSO> audioLookup = new Dictionary<string, AudioDataSO>();

    public static Action<Vector3, AudioDataSO, bool> AlertEnemyEvent;

    private AudioSource oneShotSource;
    private Transform enemyTrans;

    [SerializeField] private int maxAudioSources = 20;
    private List<AudioSource> pooledSources = new List<AudioSource>();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (oneShotSource == null)
        {
            oneShotSource = this.AddComponent<AudioSource>();
        }

        LoadAudio();
        CreateAudioPool();

        enemyTrans = FindFirstObjectByType<NPCStateManager>().gameObject.transform;

        foreach (var clip in playOnWake) {
            PlaySound(clip.id, null, null);
        }
    }
    void CreateAudioPool()
    {
        for (int i = 0; i < maxAudioSources; i++)
        {
            GameObject obj = new GameObject($"AudioSource_{i}");
            obj.transform.parent = transform;

            AudioSource source = obj.AddComponent<AudioSource>();
            pooledSources.Add(source);
        }
    }
    AudioSource GetAvailableSource()
    {
        foreach (var source in pooledSources)
        {
            if (!source.isPlaying)
            {
                AudioFollowTarget follow = source.GetComponent<AudioFollowTarget>();
                if (follow != null)
                {
                    Destroy(follow);
                }

                return source;
            }
        }

        return pooledSources[0]; // if all busy, get oldest
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
            sequential = 0;
        }

        AudioClip clip = data.playRandom ? data.GetRandomClip() : data.audioClips[sequential];

        if (clip == null) return;

        data.lastPlayed = sequential;
        float pitch = UnityEngine.Random.Range(data.pitchMin, data.pitchMax);

        if (pos == null && !loop)
        {
            oneShotSource.pitch = pitch;
            oneShotSource.volume = data.volumeMulti;
            oneShotSource.PlayOneShot(clip);
            return;
        }

        //GameObject audioObj = new GameObject($"Audio_{id}");
        //audioObj.transform.parent = transform;

        //if (pos != null) audioObj.transform.position = pos.Value;

        //AudioSource source = audioObj.AddComponent<AudioSource>();

        AudioSource source = GetAvailableSource();

        if (data.followEnemy)
        {
            AudioFollowTarget aft = source.GetComponent<AudioFollowTarget>();

            if (aft == null)
            {
                aft = source.gameObject.AddComponent<AudioFollowTarget>();
            }

            aft.target = enemyTrans;
        }

        source.Stop();

        source.clip = clip;
        source.loop = loop;
        source.pitch = pitch;
        source.volume = data.volumeMulti;
        source.spatialBlend = pos != null ? 1f : 0f;

        source.transform.position = pos ?? transform.position;

        source.Play();

        if (pos != null && data.alertEnemyOnPlay)
        {
            AlertEnemyEvent?.Invoke(pos.Value, data, data.alertWhenHunting);
        }
    }
}
