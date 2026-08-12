using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer and Groups")]
    public AudioMixer audioMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup voiceGroup;

    [Header("Music (OST)")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public float musicFadeTime = 0.8f;
    [Range(0f, 1f)] public float defaultMusicVolume = 1f;

    [Header("SFX Pool")]
    public Transform sfxPoolParent;
    public int poolSize = 12;
    public bool expandPoolIfNeeded = true;

    [Header("Clips (assign in Inspector)")]
    public List<AudioClipEntry> audioClips = new List<AudioClipEntry>();

    [Header("Prefs Keys")]
    public string musicPrefKey = "AudioManager_MusicVolume";
    public string sfxPrefKey = "AudioManager_SFXVolume";
    public string voicePrefKey = "AudioManager_VoiceVolume";

    // Internal
    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
    private bool isMusicAActive = true;
    private Coroutine musicFadeCoroutine;

    [System.Serializable]
    public class AudioClipEntry
    {
        public string id;
        public AudioClip clip;
    }

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Validate music sources
        if (musicSourceA == null)
        {
            GameObject go = new GameObject("MusicSourceA");
            go.transform.SetParent(transform);
            musicSourceA = go.AddComponent<AudioSource>();
            musicSourceA.playOnAwake = false;
            musicSourceA.outputAudioMixerGroup = musicGroup;
        }
        if (musicSourceB == null)
        {
            GameObject go = new GameObject("MusicSourceB");
            go.transform.SetParent(transform);
            musicSourceB = go.AddComponent<AudioSource>();
            musicSourceB.playOnAwake = false;
            musicSourceB.outputAudioMixerGroup = musicGroup;
        }

        // Initialize pool parent
        if (sfxPoolParent == null)
        {
            GameObject go = new GameObject("SFXPool");
            go.transform.SetParent(transform);
            sfxPoolParent = go.transform;
        }

        // Load clips into dictionary
        LoadClips();

        // Initialize pool
        InitializePool();

        // Load saved volumes
        LoadVolumePrefs();
    }

    void LoadClips()
    {
        clips.Clear();
        foreach (var e in audioClips)
        {
            if (e == null || string.IsNullOrEmpty(e.id) || e.clip == null) continue;
            if (!clips.ContainsKey(e.id))
                clips.Add(e.id, e.clip);
            else
                Debug.LogWarning($"AudioManager: duplicate clip id '{e.id}'");
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreatePooledSource();
        }
    }

    AudioSource CreatePooledSource()
    {
        GameObject go = new GameObject("PooledSFX");
        go.transform.SetParent(sfxPoolParent);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.outputAudioMixerGroup = sfxGroup;
        sfxPool.Enqueue(src);
        return src;
    }

    // -----------------------
    // Music control
    // -----------------------
    public void PlayMusic(string id, bool loop = true, float crossfadeTime = -1f)
    {
        if (!clips.ContainsKey(id))
        {
            Debug.LogWarning($"AudioManager: PlayMusic - clip id '{id}' not found");
            return;
        }

        AudioClip newClip = clips[id];
        if (crossfadeTime < 0f) crossfadeTime = musicFadeTime;

        AudioSource active = isMusicAActive ? musicSourceA : musicSourceB;
        AudioSource inactive = isMusicAActive ? musicSourceB : musicSourceA;

        // If same clip already playing on active and playing, just ensure loop
        if (active.clip == newClip && active.isPlaying)
        {
            active.loop = loop;
            return;
        }

        // Start crossfade
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(CrossfadeMusic(inactive, active, newClip, loop, crossfadeTime));
        isMusicAActive = !isMusicAActive;
    }

    IEnumerator CrossfadeMusic(AudioSource toSource, AudioSource fromSource, AudioClip newClip, bool loop, float duration)
    {
        toSource.clip = newClip;
        toSource.loop = loop;
        toSource.volume = 0f;
        toSource.Play();

        float time = 0f;
        float fromStartVol = fromSource.isPlaying ? fromSource.volume : 0f;
        float toTargetVol = defaultMusicVolume;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            toSource.volume = Mathf.Lerp(0f, toTargetVol, t);
            if (fromSource.isPlaying)
                fromSource.volume = Mathf.Lerp(fromStartVol, 0f, t);
            yield return null;
        }

        toSource.volume = toTargetVol;
        if (fromSource.isPlaying) fromSource.Stop();
        musicFadeCoroutine = null;
    }

    public void StopMusic(float fadeTime = 0.5f)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeOutMusic(fadeTime));
    }

    IEnumerator FadeOutMusic(float duration)
    {
        AudioSource active = isMusicAActive ? musicSourceA : musicSourceB;
        float startVol = active.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            active.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }
        active.Stop();
        active.volume = defaultMusicVolume;
        musicFadeCoroutine = null;
    }

    // -----------------------
    // SFX playback (pool)
    // -----------------------
    public void PlayOneShot(string id, float volume = 1f)
    {
        if (!clips.ContainsKey(id))
        {
            Debug.LogWarning($"AudioManager: PlayOneShot - clip id '{id}' not found");
            return;
        }
        AudioSource src = GetPooledSource();
        if (src == null) return;
        src.clip = clips[id];
        src.spatialBlend = 0f;
        src.volume = volume;
        src.loop = false;
        src.Play();
        StartCoroutine(ReturnToPoolWhenFinished(src));
    }

    public void PlaySFXAtPosition(string id, Vector3 position, float volume = 1f, float spatialBlend = 1f)
    {
        if (!clips.ContainsKey(id))
        {
            Debug.LogWarning($"AudioManager: PlaySFXAtPosition - clip id '{id}' not found");
            return;
        }
        AudioSource src = GetPooledSource();
        if (src == null) return;
        src.transform.position = position;
        src.clip = clips[id];
        src.spatialBlend = spatialBlend;
        src.volume = volume;
        src.loop = false;
        src.Play();
        StartCoroutine(ReturnToPoolWhenFinished(src));
    }

    public AudioSource PlaySFXOnObject(string id, GameObject target, bool loop = false, float volume = 1f, float spatialBlend = 1f)
    {
        if (!clips.ContainsKey(id))
        {
            Debug.LogWarning($"AudioManager: PlaySFXOnObject - clip id '{id}' not found");
            return null;
        }
        AudioSource src = target.GetComponent<AudioSource>();
        if (src == null)
        {
            src = target.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxGroup;
            src.playOnAwake = false;
        }
        src.clip = clips[id];
        src.loop = loop;
        src.volume = volume;
        src.spatialBlend = spatialBlend;
        src.Play();
        return src;
    }

    AudioSource GetPooledSource()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource src = sfxPool.Dequeue();
            src.transform.SetParent(null);
            return src;
        }
        else if (expandPoolIfNeeded)
        {
            AudioSource src = CreatePooledSource();
            sfxPool.Dequeue(); // created and enqueued, remove to return
            src.transform.SetParent(null);
            return src;
        }
        else
        {
            Debug.LogWarning("AudioManager: SFX pool exhausted");
            return null;
        }
    }

    IEnumerator ReturnToPoolWhenFinished(AudioSource src)
    {
        yield return new WaitWhile(() => src.isPlaying);
        src.clip = null;
        src.transform.SetParent(sfxPoolParent);
        src.transform.localPosition = Vector3.zero;
        src.spatialBlend = 0f;
        sfxPool.Enqueue(src);
    }

    // -----------------------
    // Volume control
    // -----------------------
    public void SetVolume(string exposedParam, float linear)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioManager: SetVolume called but no AudioMixer assigned");
            return;
        }
        float clamped = Mathf.Clamp01(linear);
        float dB = (clamped <= 0f) ? -80f : Mathf.Log10(clamped) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
    }

    public void SaveVolumePrefs()
    {
        float music = 1f, sfx = 1f, voice = 1f;
        if (audioMixer != null)
        {
            audioMixer.GetFloat("MusicVolume", out float mdb);
            audioMixer.GetFloat("SFXVolume", out float sdb);
            audioMixer.GetFloat("VoiceVolume", out float vdb);
            music = DbToLinear(mdb);
            sfx = DbToLinear(sdb);
            voice = DbToLinear(vdb);
        }
        PlayerPrefs.SetFloat(musicPrefKey, music);
        PlayerPrefs.SetFloat(sfxPrefKey, sfx);
        PlayerPrefs.SetFloat(voicePrefKey, voice);
        PlayerPrefs.Save();
    }

    public void LoadVolumePrefs()
    {
        if (audioMixer == null) return;
        float music = PlayerPrefs.GetFloat(musicPrefKey, 1f);
        float sfx = PlayerPrefs.GetFloat(sfxPrefKey, 1f);
        float voice = PlayerPrefs.GetFloat(voicePrefKey, 1f);
        SetVolumeSafe("MusicVolume", music);
        SetVolumeSafe("SFXVolume", sfx);
        SetVolumeSafe("VoiceVolume", voice);
    }

    void SetVolumeSafe(string exposedParam, float linear)
    {
        if (audioMixer == null) return;
        float clamped = Mathf.Clamp01(linear);
        float dB = (clamped <= 0f) ? -80f : Mathf.Log10(clamped) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
    }

    float DbToLinear(float db)
    {
        if (db <= -80f) return 0f;
        return Mathf.Pow(10f, db / 20f);
    }

    // -----------------------
    // Utility
    // -----------------------
    public bool HasClip(string id)
    {
        return clips.ContainsKey(id);
    }

    // Optional: call this if you change audioClips at runtime in editor
    public void RefreshClips()
    {
        LoadClips();
    }
}
