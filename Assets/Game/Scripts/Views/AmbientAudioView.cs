using DG.Tweening;
using Game.Helpers.Audio;

namespace Game.Views
{
    public class AmbientAudioView
    {
        private const float CrowInterval = 20f;
        
        private readonly IAudioManager _audioManager;

        public AmbientAudioView(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            ScheduleAudio();
        }

        private void ScheduleAudio()
        {
            _audioManager.Play(GameAudio.Wind, true);
            _audioManager.Play(GameAudio.Insects, true);
            DOVirtual.DelayedCall(CrowInterval, () => _audioManager.Play(GameAudio.Crow)).SetLoops(-1);
        }
    }
}