using Game.Helpers.Audio;
using Game.Helpers.Audio.Impl;
using Game.Views;
using UnityEngine;
using Zenject;

namespace Game.Injection
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private AudioClipsDataObject _audioClipsDataObject;
        
        public override void InstallBindings()
        {
            Container.BindInstance<IAudioClipsProvider>(_audioClipsDataObject).AsSingle();
            Container.Bind<IAudioManager>().To<AudioManager>().AsSingle();
            Container.Bind<AmbientAudioView>().AsSingle().NonLazy();
        }
    }
}