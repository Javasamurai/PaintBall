using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Gameplay
{
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<PlayerData, PlayerInputData, LocalTransform>();
            
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            var job = new PlayerMovementJob
            {
                DeltaTime = deltaTime
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform transform, in PlayerData playerData, in PlayerInputData inputData)
        {
            float3 direction = new float3(inputData.move.x, 0, inputData.move.y);
            float3 movement = direction * playerData.MoveSpeed * DeltaTime;
            transform.Position += movement;
        }
    }
}