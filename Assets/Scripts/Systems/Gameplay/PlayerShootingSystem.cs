using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Gameplay
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [DisableAutoCreation]
    [UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
    public partial struct PlayerShootingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<PlayerInputData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerData, inputData, transform) in SystemAPI.Query<RefRO<PlayerData>, RefRW<PlayerInputData>, RefRO<LocalTransform>>())
            {
                if (inputData.ValueRO.shoot.IsSet)
                {
                    inputData.ValueRW.shoot = default;
                    
                    RaycastHit hit;
                    var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
                    var rayOrigin = transform.ValueRO.Position + new float3(0, 1.5f, 0);
                    var rayDirection = transform.ValueRO.Forward();
                    var raycastInput = new RaycastInput
                    {
                        Start = rayOrigin,
                        End = rayOrigin + rayDirection * 100f // TODO: Move this to a config variable
                    };
                    if (physicsWorld.CastRay(raycastInput, out hit))
                    {
                        var hitEntity = hit.Entity;

                        if (SystemAPI.HasComponent<HealthComponent>(hitEntity))
                        {
                            var health = SystemAPI.GetComponentRW<HealthComponent>(hitEntity);
                            health.ValueRW.CurrentHealth -= 10;
                            UnityEngine.Debug.Log($"Player {playerData.ValueRO} shot!");
                        }
                    }
                }
            }
        }
    }
}