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
            
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithNone<SpawnPointTag>().WithAll<PlayerData>().WithEntityAccess())
            {
                foreach (var ( s, t, e) in SystemAPI.Query<RefRO<SpawnPointComponent>, RefRO<LocalTransform>>().WithEntityAccess())
                {
                    // if (!s.ValueRO.occupied)
                    // {
                        commandBuffer.SetComponent(e, new SpawnPointComponent
                        {
                            occupied = true,
                            RespawnTime = 5f,
                            spawned = true
                        });
                        transform.ValueRW.Position = t.ValueRO.Position;
                        commandBuffer.AddComponent<SpawnPointTag>(entity);
                        break;
                    // }
                }
            }
            commandBuffer.Playback(EntityManager);
        }
    }
}