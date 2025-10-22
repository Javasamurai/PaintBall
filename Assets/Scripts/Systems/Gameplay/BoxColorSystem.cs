using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Gameplay
{
    public partial class BoxColorSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            
        }

        protected override void OnUpdate()
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (physicsMass, healthComponent, entity) in SystemAPI.Query<RefRW<PhysicsMass>, RefRW<HealthComponent>>().WithEntityAccess())
            {
                if (healthComponent.ValueRO.CurrentHealth <= 100 && healthComponent.ValueRO.CurrentHealth > 50)
                {
                    
                }
                else if (healthComponent.ValueRO.CurrentHealth <= 50 && healthComponent.ValueRO.CurrentHealth > 0)
                {
                    
                }
                else if (healthComponent.ValueRO.CurrentHealth <= 0)
                {
                    physicsMass.ValueRW.InverseMass = 0f;
                }
            }
        }
    }
}