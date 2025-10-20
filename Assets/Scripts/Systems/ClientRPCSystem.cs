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
            if (Input.GetKey(KeyCode.W))
            {
                SendRPC("Hello from Client!", ClientServerBootstrap.ClientWorld);
            }
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