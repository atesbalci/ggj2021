using System.Collections.Generic;
using UnityEngine;

namespace Game.Helpers.Audio.Impl
{
    public class AudioManager : IAudioManager
    {
        private const int PoolSize = 20;

        private readonly IAudioClipsProvider _audioClipsProvider;
        private readonly IList<AudioSource> _audioSources;
        
        private int _currentIndex = -1;

        public AudioManager(IAudioClipsProvider audioClipsProvider)
        {
            _audioClipsProvider = audioClipsProvider;
            _audioSources = new AudioSource[PoolSize];
            var gameObject = new GameObject("AudioManager");
            for (int i = 0; i < PoolSize; i++)
            {
                _audioSources[i] = gameObject.AddComponent<AudioSource>();
                _audioSources[i].playOnAwake = false;
            }
        }

        public IAudioPlaybackInstance Play(int clip, bool loops = false)
        {
            AudioSource currentSource;
            do
            {
                _currentIndex = (_currentIndex + 1) % PoolSize;
                currentSource = _audioSources[_currentIndex];
            } while (currentSource.loop && currentSource.isPlaying);

            var clipData = _audioClipsProvider.GetClipData(clip);
            currentSource.clip = clipData.Clip;
            currentSource.outputAudioMixerGroup = clipData.Group;
            currentSource.volume = clipData.DefaultVolume;
            currentSource.loop = loops;
            currentSource.pitch = 1f;
            currentSource.Play();

            return new Playback(currentSource);
        }

        public void StopAndClearAll()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = null;
            }
        }

        private class Playback : IAudioPlaybackInstance
        {
            private readonly AudioSource _audioSource;

            public Playback(AudioSource audioSource)
            {
                _audioSource = audioSource;
            }
            
            public void Stop()
            {
                _audioSource.Stop();
            }

            public float Volume
            {
                get => _audioSource.volume;
                set => _audioSource.volume = value;
            }

            public bool Looping
            {
                get => _audioSource.loop;
                set => _audioSource.loop = value;
            }

            public bool IsPlaying => _audioSource.isPlaying;

            public float Pitch
            {
                get => _audioSource.pitch;
                set => _audioSource.pitch = value;
            }
        }
    }
}