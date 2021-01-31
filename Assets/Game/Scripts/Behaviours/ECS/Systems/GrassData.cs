using Unity.Entities;

namespace Game.Behaviours.ECS.Systems
{
    public struct GrassData : IComponentData
    {
        public bool IsDynamic;
        public int StaticCullingId;
        public float SwayDuration;
    }
}