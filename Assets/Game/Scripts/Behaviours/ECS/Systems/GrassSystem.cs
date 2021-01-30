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
            float3 playerRightVec = PlayerTransform.right;
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
                    rotation.Value = quaternion.Euler(
                        0f,
                        playerYRot,
                        (1f - distSq / Constants.GrassCloseThresholdSq) * (math.PI / 12f) * (math.dot(playerRightVec, -vec) > 0 ? -1f : 1f),
                        math.RotationOrder.ZYX);
                }
                else
                {
                    rotation.Value = quaternion.identity;
                }
            }).ScheduleParallel();
        }
    }
}