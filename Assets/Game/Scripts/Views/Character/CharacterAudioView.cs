using Game.Helpers.Audio;
using UnityEngine;
using Zenject;

namespace Game.Views.Character
{
    public class CharacterAudioView : MonoBehaviour
    {
        private const float FootStepFrequency = 1.5f;
        private const float FootStepFrequencySq = FootStepFrequency * FootStepFrequency;
        private const float FootStepPitchVariance = 0.5f;

        private const float BushNoiseFrequency = 3f;
        private const float BushNoiseFrequencySq = BushNoiseFrequency * BushNoiseFrequency;

        private IAudioManager _audioManager;
        
        private Vector2 _lastFootStepPos;
        private Vector2 _lastBushNoisePos;

        [Inject]
        public void Initialize(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Update()
        {
            var pos = transform.position;
            var pos2d = new Vector2(pos.x, pos.z);
            
            if ((_lastFootStepPos - pos2d).sqrMagnitude > FootStepFrequencySq)
            {
                _lastFootStepPos = pos2d;
                _audioManager.Play(GameAudio.FootStep).Pitch = 1f + Random.Range(-1f, 1f) * FootStepPitchVariance;
            }
            
            if ((_lastBushNoisePos - pos2d).sqrMagnitude > BushNoiseFrequencySq)
            {
                _lastBushNoisePos = pos2d;
                _audioManager.Play(GameAudio.BushNoise);
            }
        }
    }
}