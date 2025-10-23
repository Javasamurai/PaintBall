using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Systems.Gameplay
{
    public struct BoxComponent : IComponentData
    {
        public float MinSpeed;
        public float MaxSpeed;
        public float Radius;
        public bool toDestroy;
        
        public float CurrentSpeed;
    }
    public partial class BoxMovementSystem : SystemBase
    {
        private SystemHandle _ecbSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            _ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
            RequireForUpdate<BoxComponent>();
        }
        protected override void OnUpdate()
        {
            // Box movement logic
            // sinosuidal movement along the x axis
            float deltaTime = SystemAPI.Time.DeltaTime;
            float amplitude = 2f;
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (box, transform) in SystemAPI.Query<RefRW<BoxComponent>, RefRW<LocalTransform>>())
            {
                if (box.ValueRO.toDestroy)
                {
                    continue;
                }
                if (box.ValueRW.CurrentSpeed == 0)
                {
                    uint frameSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
                    var random = Random.CreateFromIndex(frameSeed);
                    var speed = random.NextFloat(box.ValueRO.MinSpeed, box.ValueRO.MaxSpeed);

                    box.ValueRW.CurrentSpeed = speed;
                }
                var multiplier = math.sin((float)SystemAPI.Time.ElapsedTime * math.PI * 0.5f);
                var newPosition = multiplier * box.ValueRO.Radius * deltaTime * box.ValueRW.CurrentSpeed;
                
                transform.ValueRW.Position.x += newPosition;
            }
            // Check if any box for destruction
            
            foreach (var (box, entity) in SystemAPI.Query<RefRW<BoxComponent>>().WithEntityAccess())
            {
                if (box.ValueRO.toDestroy)
                {
                    commandBuffer.DestroyEntity(entity);
                }
            }
            
            commandBuffer.Playback(EntityManager);
        }
    }
}