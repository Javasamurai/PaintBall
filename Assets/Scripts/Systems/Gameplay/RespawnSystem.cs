using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Gameplay
{
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
            var entitycommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (health, spawnPoint, transform, entity) in SystemAPI.Query<RefRW<HealthComponent>, RefRW<SpawnPointComponent>, RefRW<LocalTransform>>().WithEntityAccess())
            {
                if (!health.ValueRO.IsAlive)
                {
                    if (spawnPoint.ValueRO.RespawnTime <= 0f)
                    {
                        // Respawn logic
                        spawnPoint.ValueRW.spawned = true;
                        spawnPoint.ValueRW.RespawnTime = 5f;
                        health.ValueRW.IsAlive = true;
                        health.ValueRW.CurrentHealth = health.ValueRO.MaxHealth;
                        entitycommandBuffer.SetComponent(entity, new HealthComponent
                        {
                            CurrentHealth = health.ValueRO.MaxHealth,
                            MaxHealth = health.ValueRO.MaxHealth,
                            IsAlive = true
                        });
                    }
                    else if (spawnPoint.ValueRO.RespawnTime > 0f)
                    {
                        // Decrease respawn time
                        spawnPoint.ValueRW.RespawnTime -= SystemAPI.Time.DeltaTime;
                        
                        // Move to a safe position while respawning
                        entitycommandBuffer.SetComponent(entity, new LocalTransform
                        {
                            Position = new float3(1000, 1000, 1000),
                        });
                    }

                }
            }
        }
    }
}