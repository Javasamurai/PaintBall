using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Gameplay
{
    public struct RespawnComponent : IComponentData
    {
        public float RespawnTime;
    }
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class RespawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp);
            queryBuilder.WithAll<SpawnPointTag, LocalTransform, SpawnPointComponent>();
            
            EntityQuery spawnPointQuery = GetEntityQuery(queryBuilder);
            RequireForUpdate(spawnPointQuery);
        }

        protected override void OnUpdate()
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (health, spawnPoint, entity) in SystemAPI.Query<RefRW<HealthComponent>, RefRW<RespawnComponent>>().WithEntityAccess())
            {
                if (!health.ValueRO.IsAlive)
                {
                    if (spawnPoint.ValueRO.RespawnTime <= 0f)
                    {
                        // Respawn logic
                        spawnPoint.ValueRW.RespawnTime = 5f;
                        health.ValueRW.IsAlive = true;
                        health.ValueRW.CurrentHealth = health.ValueRO.MaxHealth;
                        entityCommandBuffer.SetComponent(entity, new HealthComponent
                        {
                            CurrentHealth = health.ValueRO.MaxHealth,
                            MaxHealth = health.ValueRO.MaxHealth,
                            IsAlive = true
                        });
                        // Remove the spawn point tag
                        if (SystemAPI.HasComponent<SpawnPointTag>(entity))
                        {
                            entityCommandBuffer.RemoveComponent<SpawnPointTag>(entity);
                        }
                    }
                    else if (spawnPoint.ValueRO.RespawnTime > 0f)
                    {
                        // Decrease respawn time
                        spawnPoint.ValueRW.RespawnTime -= SystemAPI.Time.DeltaTime;
                        
                        // Move to a safe position while respawning
                        entityCommandBuffer.SetComponent(entity, new LocalTransform
                        {
                            Position = new float3(1000, 1000, 1000),
                        });
                    }
                }
            }
            entityCommandBuffer.Playback(EntityManager);
        }
    }
}