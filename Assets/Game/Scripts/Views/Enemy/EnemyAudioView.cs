using DG.Tweening;
using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Enemy.AI.States;
using Game.Helpers.Audio;
using Game.Helpers.DistanceZones;
using UnityEngine;
using Zenject;

namespace Game.Views.Enemy
{
    public class EnemyAudioView : MonoBehaviour
    {
        private IAudioManager _audioManager;
        private IAudioPlaybackInstance _chaseMusicPlayback;
        private Transform _playerTransform;
        private DistanceZoneProperty _distanceZone;

        private Tween _audioTickTickTween;
        
        [Inject]
        public void Initialize(IAudioManager audioManager, Transform playerTransform)
        {
            _audioManager = audioManager;
            _playerTransform = playerTransform;
            _distanceZone = new DistanceZoneProperty(
                Constants.EnemyDeathDistance,
                Constants.EnemyCloseDistance,
                Constants.EnemyMidDistance,
                Constants.EnemyFarDistance);
            _distanceZone.ZoneChange += OnDistanceZoneChange;
            AiBehaviour.OnPlayerCatched += OnPlayerCatch;
            ChaseState.OnEnter += ChaseStateOnEnter;
            ChaseState.OnExit += ChaseStateOnExit;
        }

        private void OnDistanceZoneChange(DistanceZone zone)
        {
            _audioTickTickTween.Kill();
            float interval;
            int clip;
            switch (zone)
            {
                case DistanceZone.Far:
                    interval = Constants.EnemyFarClipInterval;
                    clip = GameAudio.BoundFar;
                    break;
                case DistanceZone.Mid:
                    interval = Constants.EnemyMidClipInterval;
                    clip = GameAudio.BoundMid;
                    break;
                case DistanceZone.Close:
                    interval = Constants.EnemyCloseClipInterval;
                    clip = GameAudio.BoundClose;
                    break;
                default:
                    return;
            }

            _audioTickTickTween = DOVirtual.DelayedCall(interval, () => _audioManager.Play(clip)).SetLoops(-1);
        }

        private void OnPlayerCatch()
        {
            _audioManager.Play(GameAudio.Scream);
        }

        private void ChaseStateOnEnter()
        {
            _chaseMusicPlayback?.Stop();
            _chaseMusicPlayback = _audioManager.Play(GameAudio.Chase, true);
        }

        private void ChaseStateOnExit()
        {
            _chaseMusicPlayback?.Stop();
            _chaseMusicPlayback = null;
        }

        private void Update()
        {
            _distanceZone.UpdateDistance(Vector3.Distance(_playerTransform.position, transform.position));
        }
    }
}