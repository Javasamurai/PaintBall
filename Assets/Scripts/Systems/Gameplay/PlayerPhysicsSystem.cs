using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Gameplay
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct PlayerPhysicsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            var builder = new EntityQueryBuilder(Allocator.Temp);
            
            builder.WithAny<PlayerData, GhostOwnerIsLocal>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            RaycastHit hit;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            foreach (var (playerData, localTransform, ghostOwner) in SystemAPI.Query<RefRW<PlayerData>, RefRW<LocalTransform>, RefRO<GhostOwnerIsLocal>>())
            {
                var rayOrigin = localTransform.ValueRW.Position + new float3(0, 1.5f, 0);
                var rayDirection = new float3(0, -1, 0);
                var raycastInput = new RaycastInput
                {
                    Start = rayOrigin,
                    End = rayOrigin + rayDirection * 10f,
                    Filter = new CollisionFilter() 
                    {
                        CollidesWith = CollisionFilter.Default.CollidesWith,
                        GroupIndex = 0
                    },
                };
                
                raycastInput.Filter.CollidesWith = playerData.ValueRW.groundLayer;

                if (physicsWorld.CastRay(raycastInput, out hit))
                {
                    // If the player is grounded, apply gravity
                    if (hit.Entity != Entity.Null)
                    {
                        localTransform.ValueRW.Position.y = hit.Position.y + 1.5f;
                    }
                }
            }
        }
    }
}