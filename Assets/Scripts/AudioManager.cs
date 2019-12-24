/* The ol' Brackeys audio manager, slightly tweaked */

using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
    public bool loop = false;
    private AudioSource source;
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.playOnAwake = false;
        source.clip = clip;
        source.loop = loop;
    }
    public void Play()
    {
        if (!source.isPlaying)
        {
            source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
            source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
            source.Play();
        }
    }
    public void PlayOneShot(AudioClip clip)
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.PlayOneShot(clip);
    }
    public void PlayPause()
    {
        if (source.isPlaying)
        {
            source.Pause();
        }
        else
        {
            source.Play();
        }
    }
    public void Stop()
    {
        source.Stop();
    }
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField]
    Sound[] sounds;
    void Awake()
    {
        CheckForAudioManager();
    }
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        StartBGM();
    }
    private void StartBGM()
    {
        PlaySound("BGM", false);
        PlayPauseSound("BGM");
    }
    private void CheckForAudioManager()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void PlaySound(string _name, bool once)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if (once)
                {
                    sounds[i].PlayOneShot(sounds[i].clip);
                    return;
                }
                sounds[i].Play();
                return;
            }
        }
    }
    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }
    public void PlayPauseSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].PlayPause();
                return;
            }
        }
    }
}