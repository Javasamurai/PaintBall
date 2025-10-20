using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems
{
    public struct ServerRPCCommand : IRpcCommand
    {
        public FixedString64Bytes message;
    }
    /// <summary>
    /// Marker component to indicate that a client has been initialized.
    /// </summary>
    public struct InitializedClientTag : IComponentData
    {
    }
    public partial class ServerRPCSystem : SystemBase
    {
        private ComponentLookup<NetworkId> _clients;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<NetworkId>();
            _clients = GetComponentLookup<NetworkId>(true);
        }

        protected override void OnUpdate()
        {
            _clients.Update(this);
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (receive, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientRPCCommand>>().WithEntityAccess())
            {
                UnityEngine.Debug.Log($"Server received RPC from client {entity.Index}: {command.ValueRO.message}");
                commandBuffer.DestroyEntity(entity);
            }
            
            foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClientTag>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClientTag>(entity);
                UnityEngine.Debug.Log($"Server initialized client {entity.Index} - {id.ValueRO.Value}");
            }

            foreach (var (receive, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnPlayerRPCCommand>>().WithEntityAccess())
            {
                if (SystemAPI.TryGetSingleton<SpawnerData>(out var spawnData))
                {
                    var playerEntity = commandBuffer.Instantiate(spawnData.PlayerPrefab);
                    
                    commandBuffer.SetComponent(playerEntity, new LocalTransform()
                    {
                        Position = new float3(100, 0, 100f),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });
                    var networkId = _clients[receive.ValueRO.SourceConnection];
                    commandBuffer.SetComponent(playerEntity, new GhostOwner()
                    {
                        NetworkId = networkId.Value
                    });
                     commandBuffer.AddComponent<NetworkStreamInGame>(playerEntity);

                    // Add to the buffer
                    commandBuffer.AppendToBuffer(receive.ValueRO.SourceConnection, new LinkedEntityGroup { Value = playerEntity });
                    commandBuffer.DestroyEntity(entity);
                }
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
        
        public void SendRPC(string text, World world, Entity clientEntity)
        {
            if (string.IsNullOrEmpty(text) || !world.IsCreated)
            {
                UnityEngine.Debug.Log("Cannot send RPC: invalid text or world not created.");
            }
            var entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ServerRPCCommand));
            
            var rpcCommand = new ServerRPCCommand
            {
                message = text
            };
            
            world.EntityManager.SetComponentData(entity, rpcCommand);
            if (clientEntity != Entity.Null)
            {

                world.EntityManager.AddComponentData(entity, new SendRpcCommandRequest { TargetConnection = clientEntity });
            }
        }
    }
}