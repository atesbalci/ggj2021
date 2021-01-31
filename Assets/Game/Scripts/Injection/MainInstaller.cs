using Game.Behaviours.Boundaries;
using Game.Behaviours.Game;
using Game.Controllers;
using Game.Helpers.Audio;
using Game.Helpers.Audio.Impl;
using Game.Models;
using Game.Views.Ambient;
using Game.Views.Collectables;
using UnityEngine;
using Zenject;

namespace Game.Injection
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private AudioClipsDataObject _audioClipsDataObject;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private BoundsBehaviour _boundsBehaviour;
        
        public override void InstallBindings()
        {
            InstallParent();
            Container.BindInstance<IAudioClipsProvider>(_audioClipsDataObject).AsSingle();
            Container.Bind<IAudioManager>().To<AudioManager>().AsSingle();
            Container.Bind<AmbientAudioView>().AsSingle().NonLazy();
            Container.BindInstance(_playerTransform).AsSingle();
            Container.BindInterfacesAndSelfTo<GameBehaviour>().AsSingle().NonLazy();
            Container.BindInstance(_boundsBehaviour).AsSingle();
            Container.Bind<CollectableAudioView>().AsSingle().NonLazy();
            
            Container.Bind<LevelFinishController>().AsSingle().NonLazy();
        }

        private void InstallParent()
        {
            var parentContainer = Container.ParentContainers[0];
            if (parentContainer.TryResolve<GameStateData>() == null)
            {
                parentContainer.Bind<GameStateData>().AsSingle();
            }
        }
    }
}