using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Gameplay
{
    public struct PlayerData : IComponentData
    {
        public FixedString64Bytes PlayerName;
        public float MoveSpeed;
        public float SprintSpeed;
        public float LookSensitivity;
        public uint groundLayer;
    }

    public class Player : MonoBehaviour
    {
        [SerializeField] public float speed;
        [SerializeField] public float Look;
        [SerializeField] public float Sprint;
        [SerializeField] public uint groundLayer = 1;
    }

    public class PlayerBaker : Baker<Player>
    {
        public override void Bake(Player authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
                
            // Move this to a config
            AddComponent(entity, new PlayerData
            {
                MoveSpeed = authoring.speed,
                LookSensitivity = authoring.Look,
                SprintSpeed = authoring.Sprint,
                groundLayer = authoring.groundLayer
            });
            AddComponent<PlayerInputData>(entity);
            AddComponent(entity, new HealthComponent
            {
                CurrentHealth = 100,
                MaxHealth = 100,
                IsAlive = true
            });
        }
    }
}