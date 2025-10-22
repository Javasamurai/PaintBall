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
            foreach (var health in SystemAPI.Query<RefRW<HealthComponent>>())
            {
                if (health.ValueRW.CurrentHealth <= 0)
                {
                    health.ValueRW.IsAlive = false;
                }
            }
        }
    }
}