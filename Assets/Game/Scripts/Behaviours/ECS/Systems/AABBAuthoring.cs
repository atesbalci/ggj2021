using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Behaviours.ECS.Systems
{
    public struct AABB : IComponentData
    {
        public float3 Min;
        public float3 Max;
    }
    
    public class AABBAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BoxCollider _collider;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var bounds = _collider.bounds;
            var aabb = new AABB
            {
                Min = bounds.min,
                Max = bounds.max
            };
            
            dstManager.AddComponentData(entity, aabb);
        }
    }
}