using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Gameplay
{
    public partial class SpawnPointSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<PlayerData>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            
            foreach (var (player, transform, entity) in SystemAPI.Query<RefRO<PlayerData>, RefRO<LocalTransform>>().WithNone<SpawnPointTag>().WithEntityAccess())
            {
                foreach (var ( s, t, e) in SystemAPI.Query<RefRO<SpawnPointComponent>, RefRO<LocalTransform>>().WithEntityAccess())
                {
                    if (!s.ValueRO.occupied)
                    {
                        commandBuffer.SetComponent(e, new SpawnPointComponent
                        {
                            occupied = true
                        });
                        commandBuffer.SetComponent(entity, new LocalTransform()
                        {
                            Position = t.ValueRO.Position,
                            Rotation = quaternion.identity,
                            Scale = 1f
                        });
                        commandBuffer.AddComponent<SpawnPointTag>(entity);
                        break;
                    }
                }
            }
            commandBuffer.Playback(EntityManager);
        }
    }
}