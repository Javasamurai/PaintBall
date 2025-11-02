using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Systems.Gameplay
{
    [UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerShootingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<PlayerInputData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (player, inputData, transform, entity) in SystemAPI.Query<RefRW<PlayerData>, RefRW<PlayerInputData>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                if (inputData.ValueRO.shoot.IsSet)
                {
                    var allHits = new NativeList<RaycastHit>(Allocator.Temp);
                    var rayOrigin = transform.ValueRO.Position + new float3(0, 1.5f, 0);
                    var rayDirection = transform.ValueRO.Forward();
                    
                    // TODO: Move this to a config variable,
                    var filter = new CollisionFilter
                    {
                        BelongsTo = (uint) 1 << 0,
                        CollidesWith = (uint) 1 << 6 | (uint) 1 << 0,
                        GroupIndex = 0
                    };
                    
                    var raycastInput = new RaycastInput
                    {
                        Start = rayOrigin,
                        End = rayOrigin + rayDirection * 100f,
                        Filter = filter
                    };
                    Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.red, 0.1f);

                    if (physicsWorld.CastRay(raycastInput, ref allHits))
                    {
                        foreach (var hit in allHits)
                        {
                            ProcessHit(entity, hit, ref state, player);
                        }
                    }
                }
            }
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }

        private void ProcessHit(Entity sourceEntity, RaycastHit hit, ref SystemState state, RefRW<PlayerData> player)
        {
            var hitEntity = hit.Entity;

            if (hitEntity == sourceEntity)
            {
                return;
            }
            // if (SystemAPI.HasComponent<GhostOwnerIsLocal>(hitEntity))
            // {
            //     return;
            // }
            if (!SystemAPI.HasComponent<HealthComponent>(hitEntity))
            {
                return;
            }

            var hitHealth = SystemAPI.GetComponent<HealthComponent>(hitEntity);
            if (hitHealth.IsAlive)
            {
                // Choosing 34 as the damage value for the hit
                hitHealth.CurrentHealth = (ushort)math.max(0, hitHealth.CurrentHealth - 34);
                state.EntityManager.SetComponentData(hitEntity, hitHealth);
                            
                if (hitHealth.CurrentHealth <= 0)
                {
                    if (SystemAPI.HasComponent<BoxComponent>(hitEntity))
                    {
                        state.EntityManager.SetComponentData(hitEntity, new BoxComponent
                        {
                            toDestroy = true
                        });
                    }
                }
            }
        }
    }
}