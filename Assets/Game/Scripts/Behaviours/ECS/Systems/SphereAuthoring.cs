using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Behaviours.ECS.Systems
{
    public struct Sphere : IComponentData
    {
        public float3 Center;
        public float Radius;
    }
    public class SphereAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private SphereCollider _collider;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var sphere = new Sphere
            {
                Center = _collider.transform.position,
                Radius = _collider.radius,
            };
            
            dstManager.AddComponentData(entity, sphere);
        }
    }
}