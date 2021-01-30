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
            float playerYRot = PlayerTransform.eulerAngles.y * Mathf.Deg2Rad;
            float3 playerRight = PlayerTransform.right;
            float3 playerForward = PlayerTransform.forward;
            Entities.ForEach((ref GrassComponent grass, ref Translation translation, ref Rotation rotation) =>
            {
                var vec = playerLoc - translation.Value;
                var distSq = math.lengthsq(vec);
                if (distSq > Constants.MaxDistSq)
                {
                    translation.Value = playerLoc + math.normalize(vec) * Constants.MaxDist;
                }
                
                if (distSq < Constants.GrassCloseThresholdSq)
                {
                    rotation.Value = quaternion.AxisAngle(playerForward,
                        (1f - distSq / Constants.GrassCloseThresholdSq) * (math.PI / 12f) *
                        (math.dot(playerRight, -vec) > 0 ? -1f : 1f));
                }
                else
                {
                    rotation.Value = quaternion.identity;
                }
            }).ScheduleParallel();
        }
    }
}