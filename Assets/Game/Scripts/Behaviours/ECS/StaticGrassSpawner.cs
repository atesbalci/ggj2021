using Game.Behaviours.ECS.Systems;
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

        private void Start()
        {
            Spawn(_minDistance, _maxDistance, _activationDistance, _distanceIncrement, _radialGapIncrement, _randomization);
        }

        private void Spawn(float minDistance, float maxDistance, float activationDistance, float distanceIncrement, float radialGapIncrement, float randomization)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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
                var grassEntity = entityManager.Instantiate(templateEntity);
                entityManager.SetComponentData(grassEntity, new Translation
                {
                    Value = loc + randomLoc
                });
                
                entityManager.AddComponent<GrassData>(grassEntity);
                entityManager.SetComponentData(grassEntity, new GrassData()
                {
                    IsDynamic = false,
                    SwayDuration = 0,
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