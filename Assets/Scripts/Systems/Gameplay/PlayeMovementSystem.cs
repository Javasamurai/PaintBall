using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Gameplay
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
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
            float3 localMove = math.mul(transform.Rotation, movement);

            // Horizontal rotation 
            float yaw = inputData.look.x * playerData.LookSensitivity * DeltaTime;
            quaternion yawRotation = quaternion.RotateY(yaw);

            // Vertical rotation
            float pitch = inputData.look.y * playerData.LookSensitivity * DeltaTime;
            pitch = math.clamp(pitch, -0.7f, 0.7f);
            quaternion pitchRotation = quaternion.RotateX(pitch);
            transform.Rotation = math.mul(yawRotation, math.mul(transform.Rotation, pitchRotation));
            transform.Position += localMove;
        }
    }
}