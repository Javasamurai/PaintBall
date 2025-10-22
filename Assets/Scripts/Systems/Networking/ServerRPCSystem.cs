using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

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
    
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
                UnityEngine.Debug.Log($"Server initialized client {id.ValueRO.Value}");
            }


            commandBuffer.Playback(EntityManager);
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