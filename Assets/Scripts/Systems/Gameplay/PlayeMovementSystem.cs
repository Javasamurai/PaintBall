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
            float2 look = inputData.look;

            float deltaTimeMultiplier = 1;

            // _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            var rotationVelocity = look.x * playerData.LookSensitivity * DeltaTime;

            // clamp our pitch rotation
            // _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            // CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            // var quaternion = Quaternion.Euler(look.y * playerData.LookSensitivity * DeltaTime, look.x * playerData.LookSensitivity * DeltaTime, 0);
            transform.Rotation = math.mul(transform.Rotation, quaternion.RotateY(rotationVelocity));
            var localMove = math.mul(transform.Rotation, new float3(movement.x, 0, movement.z));
            transform.Position += localMove;
        }
    }
}