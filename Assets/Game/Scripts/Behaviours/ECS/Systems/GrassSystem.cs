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
            var rotations = entityQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
            var colliders = collidersQuery.ToComponentDataArray<AABB>(Allocator.TempJob);
            
            var collisionResults = new NativeArray<bool>(translations.Length, Allocator.TempJob);
            var nextPositions = new NativeArray<float3>(translations.Length, (Allocator.TempJob));
            var directions = new NativeArray<float3>(translations.Length, (Allocator.TempJob));
            var distances = new NativeArray<float>(translations.Length, (Allocator.TempJob));
            
            var distanceCheckJob = new DistanceCheckJob
            {
                PlayerLoc = playerLoc,
                Translations = translations,
                NextPositions = nextPositions,
                Direction = directions,
                Distances = distances,
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
                    
                    if (distances[i] < Constants.GrassCloseThresholdSq)
                    {
                        var rotation = new Rotation
                        {
                            Value = quaternion.AxisAngle(playerForward,
                                (1f - distances[i] / Constants.GrassCloseThresholdSq) * (math.PI / 12f) *
                                (math.dot(playerRight, -directions[i]) > 0 ? -1f : 1f)),
                        };
                        
                        EntityManager.SetComponentData(entities[i], rotation);
                    }
                    else
                    {
                        var rotation = new Rotation
                        {
                            Value = quaternion.identity
                        };
                        
                        EntityManager.SetComponentData(entities[i], rotation);
                    }
                
                    EntityManager.SetComponentData(entities[i], translation);
                }
            }
            
            translations.Dispose();
            rotations.Dispose();
            distances.Dispose();
            directions.Dispose();
            entities.Dispose();
            collisionResults.Dispose();
            nextPositions.Dispose();
            colliders.Dispose();
        }

        private struct DistanceCheckJob : IJobParallelFor
        {
            [ReadOnly] public float3 PlayerLoc;
            [ReadOnly] public NativeArray<Translation> Translations;
            public NativeArray<float3> NextPositions;
            public NativeArray<float3> Direction;
            public NativeArray<float> Distances;
            
            public void Execute(int i)
            {
                Direction[i] = PlayerLoc - Translations[i].Value;
                Distances[i] = math.lengthsq(Direction[i]);
                if (Distances[i] > Constants.MaxDistSq)
                {
                    NextPositions[i] = PlayerLoc + math.normalize(Direction[i]) * Constants.MaxDist;
                }
                else
                {
                    NextPositions[i] = Translations[i].Value;
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
