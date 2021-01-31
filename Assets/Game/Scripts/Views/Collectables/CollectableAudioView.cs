using Game.Behaviours.Interactable;
using Game.Helpers.Audio;

namespace Game.Views.Collectables
{
    public class CollectableAudioView
    {
        public CollectableAudioView(IAudioManager audioManager)
        {
            Collectable.OnCollected += collectable => audioManager.Play(GameAudio.Paper);
        }
    }
}