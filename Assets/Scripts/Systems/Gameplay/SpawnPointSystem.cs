using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Gameplay
{

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class SpawnPointSystem : SystemBase
    {
        private EntityQuery spawnPointQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<PlayerData>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            spawnPointQuery = GetEntityQuery(
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<SpawnPointComponent>()
            );
            foreach (var (player, transform, entity) in SystemAPI.Query<RefRW<PlayerData>,RefRW<LocalTransform>>().WithNone<SpawnPointTag>().WithEntityAccess())
            {
                commandBuffer.AddComponent<SpawnPointTag>(entity);
                commandBuffer.SetComponent(entity, new PlayerData
                {
                    PlayerName = player.ValueRW.PlayerName,
                    MoveSpeed = player.ValueRW.MoveSpeed,
                    LookSensitivity = player.ValueRW.LookSensitivity,
                    SprintSpeed = player.ValueRW.SprintSpeed,
                    groundLayer = player.ValueRW.groundLayer
                });
                var randomSpawnPoint = ChooseSpawnPoint(spawnPointQuery);
                transform.ValueRW.Position = randomSpawnPoint.Position;
            }
            commandBuffer.Playback(EntityManager);
        }
        
        private LocalTransform ChooseSpawnPoint(EntityQuery spawnPoints)
        {
            uint frameSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
            var random = Random.CreateFromIndex(frameSeed);
            
            int count = spawnPoints.CalculateEntityCount();
            var randomIndex = random.NextInt(0, count);

            var spawnPointsArray = spawnPoints.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.Temp);
            if (spawnPointsArray.Length == 0)
            {
                UnityEngine.Debug.LogWarning("No spawn points available. Defaulting to first spawn point.");
                return new LocalTransform { Position = float3.zero, Rotation = quaternion.identity, Scale = 1f };
            }
            if (randomIndex < spawnPointsArray.Length)
            {
                return spawnPointsArray[randomIndex];
            }
            else
            {
                return spawnPointsArray[0];
            }
        }
    }
}