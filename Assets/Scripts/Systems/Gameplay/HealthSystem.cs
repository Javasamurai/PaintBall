using Unity.Entities;

namespace Systems.Gameplay
{
    public struct HealthComponent : IComponentData
    {
        public int CurrentHealth;
        public int MaxHealth;
        public bool IsAlive;
    }
    
    public partial class HealthSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            // Update health logic
        }
    }
}