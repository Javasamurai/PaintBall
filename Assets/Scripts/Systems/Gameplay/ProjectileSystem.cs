using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Systems.Gameplay
{
    public struct Projectile : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
        public float3 Velocity;
        public float Damage;
        public float ExplosionRadius;
        public bool IsActive;
        public float Lifetime;
        public float TimeAlive;
    }

    public partial class ProjectileSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Projectile>()));
        }

        protected override void OnUpdate()
        {
            foreach (var (projectile, physicsMass, e) in SystemAPI.Query<RefRW<Projectile>, RefRW<PhysicsMass>>().WithEntityAccess())
            {
                var proj = projectile.ValueRW;
                proj.TimeAlive += SystemAPI.Time.DeltaTime;

                // Check if the projectile has exceeded its lifetime
                if (proj.TimeAlive >= proj.Lifetime)
                {
                    proj.IsActive = false;
                }
            }
        }
    }
}