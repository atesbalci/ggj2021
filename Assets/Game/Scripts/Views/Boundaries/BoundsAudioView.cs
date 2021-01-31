using DG.Tweening;
using Game.Behaviours.Boundaries;
using Game.Helpers.Audio;
using UnityEngine;
using Zenject;

namespace Game.Views.Boundaries
{
    [RequireComponent(typeof(BoundsBehaviour))]
    public class BoundsAudioView : MonoBehaviour
    {
        private Tween _tween;
        private IAudioManager _audioManager;
        
        [Inject]
        public void Initialize(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            GetComponent<BoundsBehaviour>().DistanceZoneChange += OnDistanceZoneChange;
        }

        private void OnDistanceZoneChange(BoundDistanceZone zone)
        {
            _tween.Kill();
            float delay;
            int audioClip;
            switch (zone)
            {
                case BoundDistanceZone.Far:
                    delay = Constants.BoundFarClipInterval;
                    audioClip = GameAudio.BoundFar;
                    break;
                case BoundDistanceZone.Mid:
                    delay = Constants.BoundMidClipInterval;
                    audioClip = GameAudio.BoundMid;
                    break;
                case BoundDistanceZone.Close:
                    delay = Constants.BoundCloseClipInterval;
                    audioClip = GameAudio.BoundClose;
                    break;
                default:
                    return;
            }

            _tween = DOVirtual.DelayedCall(delay, () => _audioManager.Play(audioClip)).SetLoops(-1);
        }
    }
}