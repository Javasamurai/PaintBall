using Unity.Entities;
using Unity.Transforms;

namespace Systems.Gameplay
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class RespawnSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (health, transform) in SystemAPI.Query<RefRW<HealthComponent>, RefRO<LocalTransform>>())
            {
                // Respawn time = 3s
                if (!health.ValueRW.IsAlive)
                {
                    health.ValueRW.IsAlive = true;
                    health.ValueRW.CurrentHealth = health.ValueRW.MaxHealth;
                }
            }
        }
    }
}