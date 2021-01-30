using UnityEngine;
using UnityEngine.Audio;

namespace Game.Helpers.Audio
{
    public interface IAudioClipsProvider
    {
        IClipData GetClipData(int no);
    }

    public interface IClipData
    {
        AudioClip Clip { get; }
        float DefaultVolume { get; }
        AudioMixerGroup Group { get; }
    }
}