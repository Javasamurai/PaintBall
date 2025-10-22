using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Systems.Gameplay
{
    public class Box : MonoBehaviour
    {
        [SerializeField] private float minSpeed = 2f;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float radius = 5f;
        
        public float MinSpeed => minSpeed;
        public float MaxSpeed => maxSpeed;
        public float Radius => radius;
    }

    public class BoxBaker : Baker<Box>
    {
        public override void Bake(Box authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BoxComponent
            {
                MinSpeed = authoring.MinSpeed,
                MaxSpeed = authoring.MaxSpeed,
                Radius = authoring.Radius
            });
            AddComponent(entity, new HealthComponent()
            {
                CurrentHealth = 100,
                MaxHealth = 100,
                IsAlive = true
            });
            AddComponent(entity, new URPMaterialPropertyBaseColor());
        }
    }
}