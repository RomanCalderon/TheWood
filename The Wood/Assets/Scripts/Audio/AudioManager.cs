// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audio Manager
//
// A singleton class for controlling game audio by containing references to master, sound, and music mixers.
// In order for the class to work properly, you should create the singleton asset from the Assets/Singletons/AudioManager command
// at the top bar in unity. This will create the scriptable object as a resource in unity and you will only need one. It
// will load itself with the game as long as the yaSingleton (https://assetstore.unity.com/packages/tools/integration/yasingleton-116633)
// library is present in your project.
//
// In addition, you should create mixers for Master, Sound, and Music Values. After that, expose the volume parameter
// of each mixer. The parameters should be titled MasterVolume, SoundVolume, and MusicVolume Respectively.
//
// If you have any confusion, check my channel for tutorials on how to implement it in your game below.
// https://www.youtube.com/watch?v=POM7Ath86pg
// https://www.youtube.com/watch?v=AlbAhrgcPv0
//
// Modified by Christopher Navarre and rereleased under CC-BY 4.0 https://creativecommons.org/licenses/by/4.0/legalcode.
// Please credit Chris' Tutorials @ https://www.youtube.com/c/ChrisTutorialsOnYT
//
// Originally Developed by Daniel Rodríguez (Seth Illgard) in April 2010 http://www.silentkraken.com under MIT License
// /////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using yaSingleton;

namespace ChrisTutorials.Persistent
{
    [CreateAssetMenu(fileName = "Audio Manager", menuName = "Singletons/AudioManager")]
    public class AudioManager : Singleton<AudioManager>
    {
        #region Public Fields

        // public static AudioManager Instance;

        public AudioMixerGroup masterGroup;
        public AudioMixer masterMixer;
        public AudioMixerGroup musicGroup;

        public AudioMixerGroup soundGroup;

        public int lowestDeciblesBeforeMute = -20;

        #endregion Public Fields

        #region Public Enums

        public enum AudioChannel { Master, Sound, Music }

        #endregion Public Enums

        #region Public Methods

        /// <summary>
        /// Plays a sound at the given point in space by creating an empty game object with an
        /// AudioSource in that place and destroys it after it finished playing.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="emitter"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public AudioSource CreatePlaySource(AudioClip clip, Transform emitter, float volume, float pitch, bool music = false)
        {
            GameObject go = new GameObject("Audio: " + clip.name);
            go.transform.position = emitter.position;
            go.transform.parent = emitter;

            //Create the source
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;

            // Output sound through the sound group or music group
            if (music)
                source.outputAudioMixerGroup = musicGroup;
            else
                source.outputAudioMixerGroup = soundGroup;

            source.Play();
            return source;
        }

        public AudioSource Play(AudioClip clip, Transform emitter)
        {
            return Play(clip, emitter, 1f, 1f);
        }

        public AudioSource Play(AudioClip clip, Transform emitter, float volume)
        {
            return Play(clip, emitter, volume, 1f);
        }

        /// <summary>
        /// Plays a sound by creating an empty game object with an AudioSource and attaching it to
        /// the given transform (so it moves with the transform). Destroys it after it finished playing.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="emitter"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch)
        {
            //Create an empty game object
            AudioSource source = CreatePlaySource(clip, emitter, volume, pitch);
            Destroy(source.gameObject, clip.length);
            return source;
        }

        public AudioSource Play(AudioClip clip, Vector3 point)
        {
            return Play(clip, point, 1f, 1f);
        }

        public AudioSource Play(AudioClip clip, Vector3 point, float volume)
        {
            return Play(clip, point, volume, 1f);
        }

        /// <summary>
        /// Plays a sound at the given point in space by creating an empty game object with an
        /// AudioSource in that place and destroys it after it finished playing.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="point"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch)
        {
            AudioSource source = CreatePlaySource(clip, point, volume, pitch);
            source.minDistance = 0.1f;
            source.maxDistance = 15f;
            Destroy(source.gameObject, clip.length);
            return source;
        }

        /// <summary>
        /// Plays the sound effect in a loop. Should destroy the audio source in your script when it
        /// is ready to end.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="point"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public AudioSource PlayLoop(AudioClip clip, Transform emitter, float volume = 1f, float pitch = 1f, bool music = true)
        {
            AudioSource source = CreatePlaySource(clip, emitter, volume, pitch, true);
            source.loop = true;
            return source;
        }

        /// <summary>
        /// Plays the sound effect in a loop. Should destroy the audio source in your script when it
        /// is ready to end.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="point"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public AudioSource PlayLoop(AudioClip clip, Vector3 point, float volume = 1f, float pitch = 1f, bool music = true)
        {
            AudioSource source = CreatePlaySource(clip, point, volume, pitch, true);
            source.loop = true;
            return source;
        }

        /// <summary>
        /// Adjusts the volume on the audio channel in the unity audio mixer
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="volume">From 0 (mute) to 100 (full volume - 0 DB)</param>
        public void SetVolume(AudioChannel channel, int volume)
        {
            // Converts the 0 - 100 input into decibles | volume of 0 will mute, 1 should be ~the lowestDecibles set,
            // and 100 should be 0 DB offset from the base volume on the channel
            float adjustedVolume = lowestDeciblesBeforeMute + (-lowestDeciblesBeforeMute / 5 * volume / 20);

            // Effectively completed muted if volume if 0
            if (volume == 0)
            {
                adjustedVolume = -100;
            }

            switch (channel)
            {
                case AudioChannel.Master:
                    masterMixer.SetFloat("MasterVolume", adjustedVolume);
                    break;

                case AudioChannel.Sound:
                    masterMixer.SetFloat("SoundVolume", adjustedVolume);
                    break;

                case AudioChannel.Music:
                    masterMixer.SetFloat("MusicVolume", adjustedVolume);
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods
        
        private AudioSource CreatePlaySource(AudioClip clip, Vector3 point, float volume, float pitch, bool music = false)
        {
            //Create an empty game object
            GameObject go = new GameObject("Audio: " + clip.name);
            go.transform.position = point;

            //Create the source
            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;

            // Output sound through the sound group or music group
            if (music)
                source.outputAudioMixerGroup = musicGroup;
            else
                source.outputAudioMixerGroup = soundGroup;

            source.Play();
            return source;
        }

        /// <summary>
        /// Set up audio levels
        /// </summary>
        private void Start()
        {
            // Set the audio levels from player preferences
            int masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
            int soundVolume = PlayerPrefs.GetInt("SoundVolume", 100);
            int musicVolume = PlayerPrefs.GetInt("MusicVolume", 100);

            // Update the audio mixer
            SetVolume(AudioChannel.Master, masterVolume);
            SetVolume(AudioChannel.Sound, soundVolume);
            SetVolume(AudioChannel.Music, musicVolume);
        }

        #endregion Private Methods
    }
}