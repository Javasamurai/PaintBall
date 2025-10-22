using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

namespace Systems.Gameplay
{
    public partial class BoxColorSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<BoxComponent>();
        }

        protected override void OnUpdate()
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            
            foreach (var (box,  healthComponent, baseColor) in SystemAPI.Query<RefRW<BoxComponent>, RefRO<HealthComponent>, RefRW<URPMaterialPropertyBaseColor>>())
            {
                var color = new float4(1f, 1f, 1f, 1f);

                if (healthComponent.ValueRO.CurrentHealth <= 100 && healthComponent.ValueRO.CurrentHealth > 50)
                {
                    color = new float4(0f, 1f, 0f, 1f);
                }
                else if (healthComponent.ValueRO.CurrentHealth <= 50 && healthComponent.ValueRO.CurrentHealth > 30)
                {
                    color = new float4(1f, 1f, 0f, 1f);
                }
                else if (healthComponent.ValueRO.CurrentHealth <= 0)
                {
                    color = new float4(1f, 0f, 0f, 1f);
                }
                baseColor.ValueRW.Value = color;
            }
            entityCommandBuffer.Playback(EntityManager);
        }
    }
}