using Core;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems
{
    public struct ClientRPCCommand : IRpcCommand
    {
        public FixedString64Bytes message;
    }
    
    public struct SpawnPlayerRPCCommand : IRpcCommand
    {
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientRPCSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<NetworkId>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (receive, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerRPCCommand>>().WithEntityAccess())
            {
                Debug.Log($"Client received RPC from server {entity.Index}: {command.ValueRO.message}");
                commandBuffer.DestroyEntity(entity);
            }
            commandBuffer.Playback(EntityManager);
        }

        public void SendRPC(string text, World world)
        {
            if (string.IsNullOrEmpty(text) || !world.IsCreated)
            {
                Debug.Log("Cannot send RPC: invalid text or world not created.");
            }
            var entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ClientRPCCommand));
            
            var rpcCommand = new ClientRPCCommand
            {
                message = text
            };
            world.EntityManager.SetComponentData(entity, rpcCommand);
        }
    }
}