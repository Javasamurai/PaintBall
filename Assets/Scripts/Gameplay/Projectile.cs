using Systems.Gameplay;
using Unity.Entities;
using UnityEngine;

namespace Gameplay
{
    public class Grenade : MonoBehaviour
    {

    }
    public class GrenadeBaker : Unity.Entities.Baker<Grenade>
    {
        public override void Bake(Grenade authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Projectile
            {
                Position = authoring.transform.position,
                Rotation = authoring.transform.rotation,
                Velocity = new Unity.Mathematics.float3(0, 0, 0),
                Damage = 50f,
                ExplosionRadius = 5f,
                IsActive = false,
                Lifetime = 5f
            });
        }
    }
}