using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


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

        public void Execute(ref LocalTransform transform, ref PlayerData playerData, in PlayerInputData inputData)
        {
            playerData.Yaw += inputData.look.x * playerData.LookSensitivity * DeltaTime;
            playerData.Pitch -= inputData.look.y * playerData.LookSensitivity * DeltaTime;

            playerData.Pitch = math.clamp(playerData.Pitch, -0.5f, 0.5f);
            
            quaternion yawRotation = quaternion.RotateY(playerData.Yaw);
            quaternion pitchRotation = quaternion.RotateX(playerData.Pitch);
            transform.Rotation = math.mul(yawRotation, pitchRotation);

            float3 forward = math.mul(yawRotation, new float3(0, 0, 1));
            float3 right = math.mul(yawRotation, new float3(1, 0, 0));

            float3 moveDir = forward * inputData.move.y + right * inputData.move.x;
            transform.Position += moveDir * playerData.MoveSpeed * DeltaTime;
        }
    }
}
