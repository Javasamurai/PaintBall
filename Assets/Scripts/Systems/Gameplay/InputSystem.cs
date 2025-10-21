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
            
            Debug.Log($"Local player ghost count");
            foreach (RefRW<PlayerInputData> inputData in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
            {
                inputData.ValueRW.move = playerMove;
            }
            
            int count = SystemAPI.QueryBuilder()
                .WithAll<PlayerInputData, GhostOwnerIsLocal>()
                .Build()
                .CalculateEntityCount();

            Debug.Log($"Local player ghost count: {count}");
        }
    }
}