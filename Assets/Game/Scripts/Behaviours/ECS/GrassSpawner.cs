using Game.Behaviours.ECS.Systems;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.Behaviours.ECS
{
    public class GrassSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _grassObject;
        [SerializeField] private Transform _playerTransform;

        private Entity _templateEntity;
        private EntityManager _entityManager;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _templateEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_grassObject, new GameObjectConversionSettings
            {
                DestinationWorld = World.DefaultGameObjectInjectionWorld
            });

            var grassSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GrassSystem>();
            grassSystem.GrassTemplateEntity = _templateEntity;
            grassSystem.PlayerTransform = _playerTransform;
            
            Spawn();
        }

        public void Spawn()
        {
            const float distanceIncrement = (Constants.MaxDist - Constants.MinDist) / Constants.PlayerGrassPoolSize;

            float distance = Constants.MinDist, angle = 0f;
            var playerLoc = _playerTransform.position;
            playerLoc.y = 0f;
            for (int i = 0; i < Constants.PlayerGrassPoolSize; i++)
            {
                var loc = playerLoc + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * distance;
                var grassEntity = _entityManager.Instantiate(_templateEntity);
                _entityManager.SetComponentData(grassEntity, new Translation
                {
                    Value = loc
                });
                distance += distanceIncrement;
                angle += Constants.GrassRadialGap / (Mathf.PI * distance);
            }
        }
    }
}