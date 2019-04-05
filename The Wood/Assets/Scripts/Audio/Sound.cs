using UnityEngine;
using UnityEngine.Audio;
using ChrisTutorials.Persistent;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    public AudioManager.AudioChannel audioChannel;

    [Range(0f, 1f)]
    public float baseVolume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    [HideInInspector] public AudioSource source;
    public Coroutine playCoroutine;
    public Coroutine stopCoroutine;
    [HideInInspector] public float volume;
    public float fadeDuration;
    public float delay;

    public void Initialize(AudioSource source)
    {
        this.source = source;
        source.clip = clip;
        source.volume = 0;
        source.pitch = pitch;
        source.loop = loop;

        // Set correct audio channel
        AudioMixer masterMixer = Resources.Load("Audio/MasterMixer") as AudioMixer;
        switch (audioChannel)
        {
            case AudioManager.AudioChannel.Sound:
                source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Sound")[0];
                break;
            case AudioManager.AudioChannel.Music:
                source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Music")[0];
                break;
            default:
                break;
        }
    }

    public void Update()
    {
        if (source == null)
            return;

        if (source.isPlaying)
            source.volume = Mathf.Lerp(source.volume, volume, Time.deltaTime / ((fadeDuration == 0f) ? 0.01f : fadeDuration) * DayNightCycle.instance.GetRate());
    }
}
