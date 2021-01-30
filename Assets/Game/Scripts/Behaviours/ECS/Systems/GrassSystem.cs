using Game.Behaviours.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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
            
            float3 playerRight = PlayerTransform.right;
            float3 playerForward = PlayerTransform.forward;
            
            var entityQuery = GetEntityQuery(typeof(GrassComponent), typeof(Translation),  typeof(Rotation));
            var collidersQuery = GetEntityQuery(typeof(AABB));
            
            var translations = entityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var colliders = collidersQuery.ToComponentDataArray<AABB>(Allocator.TempJob);
            
            var rotations = new NativeArray<Rotation>(translations.Length, Allocator.TempJob);
            var collisionResults = new NativeArray<bool>(translations.Length, Allocator.TempJob);
            var nextPositions = new NativeArray<float3>(translations.Length, Allocator.TempJob);

            var distanceCheckJob = new DistanceCheckJob
            {
                PlayerLoc = playerLoc,
                PlayerForward = playerForward,
                PlayerRight = playerRight,
                Translations = translations,
                NextPositions = nextPositions,
                Rotations = rotations,
            };
            
            var distanceCheckHandle = distanceCheckJob.Schedule(translations.Length, 32);
            distanceCheckHandle.Complete();
            
            var collisionCheckJob = new AABBCollisionJob
            {
                Colliders = colliders,
                NextPositions = nextPositions,
                CollisionResult = collisionResults,
            };
            
            var collisionCheckHandle = collisionCheckJob.Schedule(translations.Length, 32);
            collisionCheckHandle.Complete();

            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            for (var i = 0; i < collisionResults.Length; i++)
            {
                if (!collisionResults[i])
                {
                    var translation = new Translation
                    {
                        Value = nextPositions[i]
                    };
                    
                    EntityManager.SetComponentData(entities[i], rotations[i]);
                    EntityManager.SetComponentData(entities[i], translation);
                }
            }
            
            entities.Dispose();
            translations.Dispose();
            colliders.Dispose();
            rotations.Dispose();
            collisionResults.Dispose();
            nextPositions.Dispose();
        }

        private struct DistanceCheckJob : IJobParallelFor
        {
            [ReadOnly] public float3 PlayerLoc;
            [ReadOnly] public float3 PlayerForward;
            [ReadOnly] public float3 PlayerRight;
            [ReadOnly] public NativeArray<Translation> Translations;
            public NativeArray<float3> NextPositions;
            public NativeArray<Rotation> Rotations;
            
            public void Execute(int i)
            {
                var direction = PlayerLoc - Translations[i].Value;
                var distance = math.lengthsq(direction);
                if (distance > Constants.MaxDistSq)
                {
                    NextPositions[i] = PlayerLoc + math.normalize(direction) * Constants.MaxDist;
                }
                else
                {
                    NextPositions[i] = Translations[i].Value;
                }
                
                if (distance < Constants.GrassCloseThresholdSq)
                {
                    var rotation = new Rotation
                    {
                        Value = quaternion.AxisAngle(PlayerForward,
                            (1f - distance / Constants.GrassCloseThresholdSq) * (math.PI / 12f) *
                            (math.dot(PlayerRight, -direction) > 0 ? -1f : 1f)),
                    };
                    
                    Rotations[i] = rotation;
                }
                else
                {
                    var rotation = new Rotation
                    {
                        Value = quaternion.identity
                    };
                    
                    Rotations[i] = rotation;
                }
            }
        }
        
        [BurstCompile]
        private struct AABBCollisionJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<AABB> Colliders;
            [ReadOnly] public NativeArray<float3> NextPositions;
            public NativeArray<bool> CollisionResult;

            public void Execute(int i)
            {
                for (int j = 0; j < Colliders.Length; j++)
                {
                    if (ECSPhysics.Intersect(NextPositions[i], Colliders[j]))
                    {
                        CollisionResult[i] = true;
                    }
                }
            }
        }
    }
}
