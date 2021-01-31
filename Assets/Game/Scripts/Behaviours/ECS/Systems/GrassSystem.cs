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
            float3 playerPosition = PlayerTransform.position;
            playerPosition.y = 0f;
            
            float3 playerRight = PlayerTransform.right;
            float3 playerForward = PlayerTransform.forward;
            
            var query = new EntityQueryDesc{
                All = new ComponentType[] {typeof(GrassComponent), typeof(Translation),  typeof(Rotation), typeof(GrassData)},
                Options = EntityQueryOptions.IncludeDisabled
            };
            
            var entityQuery = GetEntityQuery(query);
            var AABBQuery = GetEntityQuery(typeof(AABB));
            var sphereQuery = GetEntityQuery(typeof(Sphere));
            var cullingQuery = GetEntityQuery(typeof(StaticGrassCulling));
            
            var translations = entityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var grassData = entityQuery.ToComponentDataArray<GrassData>(Allocator.TempJob);
            var aabbColliders = AABBQuery.ToComponentDataArray<AABB>(Allocator.TempJob);
            var sphereColliders = sphereQuery.ToComponentDataArray<Sphere>(Allocator.TempJob);
            var cullers = cullingQuery.ToComponentDataArray<StaticGrassCulling>(Allocator.TempJob);
            
            var rotations = new NativeArray<Rotation>(translations.Length, Allocator.TempJob);
            var collisionResults = new NativeArray<bool>(translations.Length, Allocator.TempJob);
            var nextPositions = new NativeArray<float3>(translations.Length, Allocator.TempJob);
            var cullingResults = new NativeArray<bool>(cullers.Length, Allocator.TempJob);

            var distanceCheckJob = new DistanceCheckJob
            {
                PlayerPosition = playerPosition,
                PlayerForward = playerForward,
                PlayerRight = playerRight,
                Translations = translations,
                NextPositions = nextPositions,
                Rotations = rotations,
                GrassData = grassData,
            };
            
            var distanceCheckHandle = distanceCheckJob.Schedule(translations.Length, 32);
            distanceCheckHandle.Complete();
            
            var collisionCheckJob = new AABBCollisionJob
            {
                AABBColliders = aabbColliders,
                SphereColliders = sphereColliders,
                NextPositions = nextPositions,
                CollisionResult = collisionResults,
            };
            
            var collisionCheckHandle = collisionCheckJob.Schedule(translations.Length, 32);
            collisionCheckHandle.Complete();

            var cullingCheckJob = new CullingCheckJob
            {
                PlayerPosition = playerPosition,
                Results = cullingResults,
                Cullers = cullers,
            };
            
            var cullingCheckHandle = cullingCheckJob.Schedule(cullers.Length, 32);
            cullingCheckHandle.Complete();
            
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            for (var i = 0; i < entities.Length; i++)
            {
                var translation = new Translation
                {
                    Value = nextPositions[i]
                };
                    
                EntityManager.SetComponentData(entities[i], rotations[i]);

                if (!grassData[i].IsDynamic)
                {
                    for (var j = 0; j < cullingResults.Length; j++)
                    {
                        if (grassData[i].StaticCullingId == cullers[j].Id)
                        {
                            if (cullingResults[j])
                            {
                                var group = EntityManager.GetBuffer<LinkedEntityGroup>(entities[i]);
                                EntityManager.AddComponent<Disabled>(group[1].Value);
                                EntityManager.AddComponent<Disabled>(entities[i]);
                            }
                            else
                            {
                                var group = EntityManager.GetBuffer<LinkedEntityGroup>(entities[i]);
                                EntityManager.RemoveComponent<Disabled>(group[1].Value);
                                EntityManager.RemoveComponent<Disabled>(entities[i]);
                            }
                        }
                    }

                    continue;
                }
                
                EntityManager.SetComponentData(entities[i], translation);

                if (!collisionResults[i])
                {
                    var group = EntityManager.GetBuffer<LinkedEntityGroup>(entities[i]);
                    EntityManager.RemoveComponent<Disabled>(group[1].Value);
                    EntityManager.RemoveComponent<Disabled>(entities[i]);
                    
                }
                else
                {
                    var group = EntityManager.GetBuffer<LinkedEntityGroup>(entities[i]);
                    EntityManager.AddComponent<Disabled>(group[1].Value);
                    EntityManager.AddComponent<Disabled>(entities[i]);
                }
            }
            
            entities.Dispose();
            translations.Dispose();
            aabbColliders.Dispose();
            sphereColliders.Dispose();
            cullers.Dispose();
            grassData.Dispose();
            rotations.Dispose();
            collisionResults.Dispose();
            nextPositions.Dispose();
            cullingResults.Dispose();
        }

        private struct CullingCheckJob : IJobParallelFor
        {
            [ReadOnly] public float3 PlayerPosition;
            [ReadOnly] public NativeArray<StaticGrassCulling> Cullers;
            public NativeArray<bool> Results;
            public void Execute(int i)
            {
                Results[i] = math.distance(PlayerPosition, Cullers[i].Position) > Cullers[i].CullDistance;
            }
        }
        
        private struct DistanceCheckJob : IJobParallelFor
        {
            [ReadOnly] public float3 PlayerPosition;
            [ReadOnly] public float3 PlayerForward;
            [ReadOnly] public float3 PlayerRight;
            [ReadOnly] public NativeArray<Translation> Translations;
            [ReadOnly] public NativeArray<GrassData> GrassData;
            [WriteOnly] public NativeArray<float3> NextPositions;
            [WriteOnly] public NativeArray<Rotation> Rotations;
            
            public void Execute(int i)
            {
                var direction = PlayerPosition - Translations[i].Value;
                var distance = math.lengthsq(direction);

                if (GrassData[i].IsDynamic)
                {
                    if (distance > Constants.MaxDistSq)
                    {
                        NextPositions[i] = PlayerPosition + math.normalize(direction) * Constants.MaxDist;
                    }
                    else
                    {
                        NextPositions[i] = Translations[i].Value;
                    }
                }

                if (distance < Constants.GrassCloseThresholdSq)
                {
                    var rotation = new Rotation
                    {
                        Value = quaternion.AxisAngle(PlayerForward,
                            (1f - distance / Constants.GrassCloseThresholdSq) * (math.PI / 6f) *
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
            [ReadOnly] public NativeArray<AABB> AABBColliders;
            [ReadOnly] public NativeArray<Sphere> SphereColliders;
            [ReadOnly] public NativeArray<float3> NextPositions;
            public NativeArray<bool> CollisionResult;

            public void Execute(int i)
            {
                for (int j = 0; j < AABBColliders.Length; j++)
                {
                    if (ECSPhysics.Intersect(NextPositions[i], AABBColliders[j]))
                    {
                        CollisionResult[i] = true;
                    }
                }
                
                for (int j = 0; j < SphereColliders.Length; j++)
                {
                    if (ECSPhysics.Intersect(NextPositions[i], SphereColliders[j]))
                    {
                        CollisionResult[i] = true;
                    }
                }
            }
        }
    }
}
