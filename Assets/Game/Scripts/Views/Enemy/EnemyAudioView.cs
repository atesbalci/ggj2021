using Game.Behaviours.Enemy.AI;
using Game.Behaviours.Enemy.AI.States;
using Game.Helpers.Audio;
using UnityEngine;
using Zenject;

namespace Game.Views.Enemy
{
    public class EnemyAudioView : MonoBehaviour
    {
        private IAudioManager _audioManager;
        private IAudioPlaybackInstance _chaseMusicPlayback;
        
        [Inject]
        public void Initialize(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            AiBehaviour.OnPlayerCatched += OnPlayerCatch;
            ChaseState.OnEnter += ChaseStateOnEnter;
            ChaseState.OnExit += ChaseStateOnExit;
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
    }
}