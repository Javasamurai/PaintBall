using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [CreateAfter(typeof(RpcSystem))]
    public partial struct SetRpcSystemDynamicAssemblyListSystem : ISystem
    {
        /// <summary>
        /// Sets the RpcSystem.DynamicAssemblyList to true.
        /// </summary>
        /// <param name="state">The state.</param>
        public void OnCreate(ref SystemState state)
        {
            SystemAPI.GetSingletonRW<RpcCollection>().ValueRW.DynamicAssemblyList = true;
            state.Enabled = false;
        }
    }
    
    /// <summary>
    /// RPC request from client to server for game to go "in game" and send snapshots / inputs
    /// </summary>
    public struct GoInGameRequest : IRpcCommand { }
    
    /// <summary>
    /// When a client has a connection with network id, go in game and tell the server to also go in game.
    /// INPUT: <see cref="NetworkId"/> network id of the client, but only if there is no NetworkStreamInGame.
    /// OUTPUT: <see cref="NetworkStreamInGame"/> stream component added and creates a <see cref="GoInGameRequest"/> request.
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem
    {
        /// <summary>
        /// Only run on entities with NetworkId and no NetworkStreamInGame.
        /// </summary>
        /// <param name="state">The state</param>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerComponent>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        /// <summary>
        /// Create a GoInGameRequest and add a NetworkStreamInGame component to the client entity.
        /// </summary>
        /// <param name="state">The state</param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess()
                         .WithNone<NetworkStreamInGame>())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(entity);
                var req = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<GoInGameRequest>(req);
                commandBuffer.AddComponent(req, new SendRpcCommandRequest {TargetConnection = entity});
                Debug.Log($"Adding RPC request to go in game from client side {entity.Index} - {id.ValueRO.Value}");
            }

            commandBuffer.Playback(state.EntityManager);
        }
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem 
    {
        private ComponentLookup<NetworkId> networkIdFromEntity;

        /// <summary>
        /// Only run on entities with GoInGameRequest and ReceiveRpcCommandRequest.
        /// </summary>
        /// <param name="state">The state</param>
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerComponent>();
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GoInGameRequest>()
                .WithAll<ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefab = SystemAPI.GetSingleton<SpawnerComponent>().PlayerPrefab;
            state.EntityManager.GetName(prefab, out var prefabName);
            var worldName = state.WorldUnmanaged.Name;

            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            networkIdFromEntity.Update(ref state);

            foreach (var (reqSrc, reqEntity) in 
                     SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequest>().WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);
                var networkId = networkIdFromEntity[reqSrc.ValueRO.SourceConnection];

                var player = commandBuffer.Instantiate(prefab);
                commandBuffer.SetComponent(player, new GhostOwner {NetworkId = networkId.Value});

                commandBuffer.SetComponent(player, new LocalTransform()
                {
                    Position = new float3(0, 10, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                // This syncs player entity after sync/desync
                commandBuffer.AppendToBuffer(reqSrc.ValueRO.SourceConnection, new LinkedEntityGroup {Value = player});

                commandBuffer.DestroyEntity(reqEntity);
            }

            commandBuffer.Playback(state.EntityManager);
        }
    }
}
