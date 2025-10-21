using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Systems.Gameplay
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class InputSystem : SystemBase
    {
        private InputSystem_Actions inputActions;
        protected override void OnCreate()
        {
            base.OnCreate();
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAny<PlayerInputData>();
            RequireForUpdate(GetEntityQuery(builder));
        }

        protected override void OnUpdate()
        {
            var playerMove = inputActions.Player.Move.ReadValue<Vector2>();
            var playerLook = inputActions.Player.Look.ReadValue<Vector2>();
            
            foreach (RefRW<PlayerInputData> inputData in SystemAPI.Query<RefRW<PlayerInputData>>().WithAny<GhostOwnerIsLocal>())
            {
                inputData.ValueRW.move = playerMove;
                inputData.ValueRW.look = playerLook;
                if (inputActions.Player.Jump.triggered)
                {
                    inputData.ValueRW.jump.Set();
                }
            }
        }
    }
}