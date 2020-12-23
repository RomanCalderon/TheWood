using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings
{
    public void SetMasterVolume(AudioMixer mixer, float value)
    {
        mixer.SetFloat("MasterVolume", ConvertToDecibel(value));
    }

    public void SetMusicVolume(AudioMixer mixer, float value)
    {
        mixer.SetFloat("MusicVolume", ConvertToDecibel(value));
    }

    public void SetSoundVolume(AudioMixer mixer, float value)
    {
        mixer.SetFloat("SoundVolume", ConvertToDecibel(value));
    }
    
    private float ConvertToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
}
