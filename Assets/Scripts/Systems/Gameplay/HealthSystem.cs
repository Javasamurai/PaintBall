using Unity.Entities;
using Unity.NetCode;

namespace Systems.Gameplay
{
    public struct HealthComponent : IComponentData
    {
        [GhostField]
        public int CurrentHealth;
        public int MaxHealth;
        [GhostField]
        public bool IsAlive;
    }
    
    public partial class HealthSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<HealthComponent>(), ComponentType.ReadOnly<GhostOwnerIsLocal>()));
        }

        protected override void OnUpdate()
        {
            var commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (health, e) in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
            {
                if (health.ValueRW.CurrentHealth <= 0)
                {
                    health.ValueRW.IsAlive = false;
                    // remove the spawn point tag
                    if (SystemAPI.HasComponent<SpawnPointTag>(e))
                    {
                        commandBuffer.RemoveComponent<SpawnPointTag>(e);
                    }
                }
            }
            commandBuffer.Playback(EntityManager);
        }
    }
}