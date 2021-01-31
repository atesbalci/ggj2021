using Game.Behaviours.ECS.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Behaviours.ECS
{
    public class StaticGrassSpawner : MonoBehaviour
    {
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _activationDistance;
        [SerializeField] private float _distanceIncrement;
        [SerializeField] private float _radialGapIncrement;
        [SerializeField] private float _randomization;
        
        [SerializeField] private GameObject _grassObject;

        private EntityManager _entityManager;
        
        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = _entityManager.CreateEntity(typeof(StaticGrassCulling));
            var cullingId = gameObject.GetHashCode();
            
            var grassCulling = new StaticGrassCulling
            {
                Id = cullingId,
                Position = transform.position,
                CullDistance = _activationDistance
            };

            _entityManager.SetComponentData(entity, grassCulling);
            
            Spawn(_minDistance, _maxDistance, _distanceIncrement, _radialGapIncrement, _randomization, cullingId);
        }

        private void Spawn(float minDistance, float maxDistance, float distanceIncrement, float radialGapIncrement, float randomization, int cullingId)
        {
            var templateEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_grassObject,
                new GameObjectConversionSettings
                {
                    DestinationWorld = World.DefaultGameObjectInjectionWorld
                });

            var grassSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GrassSystem>();
            grassSystem.GrassTemplateEntity = templateEntity;
            
            float distance = minDistance, angle = 0f;
            var pos = transform.position;
            pos.y = 0f;
            
            while (distance < maxDistance)
            {
                var loc = pos + new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * distance;
                var randomLoc = new Vector3(Random.Range(-randomization, randomization), 0, Random.Range(-randomization, randomization));
                var grassEntity = _entityManager.Instantiate(templateEntity);
               
                _entityManager.SetComponentData(grassEntity, new Translation
                {
                    Value = loc + randomLoc
                });
                
                _entityManager.AddComponent<GrassData>(grassEntity);
                _entityManager.SetComponentData(grassEntity, new GrassData()
                {
                    IsDynamic = false,
                    SwayDuration = 0,
                    StaticCullingId = cullingId
                });
                
                angle += radialGapIncrement;
                if (angle > 360)
                {
                    angle = angle % 360;
                    distance += distanceIncrement;
                }
            }
        }
    }
}