using Game.Behaviours.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Behaviours.ECS.Systems
{
    public class GrassSystem : SystemBase
    {
        public Entity GrassTemplateEntity { get; set; }
        public Transform PlayerTransform { get; set; }

        protected override void OnUpdate()
        {
            float3 playerLoc = PlayerTransform.position;
            playerLoc.y = 0f;
            Entities.ForEach((ref GrassComponent grass, ref Translation translation) =>
            {
                var vec = playerLoc - translation.Value;
                if (math.lengthsq(vec) > Constants.MaxDistSq)
                {
                    translation.Value = playerLoc + math.normalize(vec) * Constants.MaxDist;
                }
            }).Schedule();
        }
    }
}