using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Game.Helpers.Audio.Impl
{
    [CreateAssetMenu]
    public class AudioClipsDataObject : ScriptableObject, IAudioClipsProvider
    {
        [SerializeField] private ClipData[] _clips;

        public IClipData GetClipData(int no) => _clips[no];

        [Serializable]
        private class ClipData : IClipData
        {
            public AudioClip[] AudioClips;
            public float Volume = 1f;
            public AudioMixerGroup AudioMixerGroup;

            public AudioClip Clip => AudioClips[Random.Range(0, AudioClips.Length)];
            public float DefaultVolume => Volume;
            public AudioMixerGroup Group => AudioMixerGroup;
        }
    }
}