using Unity.Entities;
using Unity.Mathematics;

namespace Game.Behaviours.ECS
{
    public struct StaticGrassCulling : IComponentData
    {
        public int Id;
        public float3 Position;
        public float CullDistance;
    }
}