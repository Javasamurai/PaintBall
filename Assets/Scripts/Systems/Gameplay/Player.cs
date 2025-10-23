using DefaultNamespace;
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
        [SerializeField]
        public PlayerConfig playerConfig;
    }

    public class PlayerBaker : Baker<Player>
    {
        public override void Bake(Player authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
                
            // Move this to a config
            AddComponent(entity, new PlayerData
            {
                MoveSpeed = authoring.playerConfig.speed,
                LookSensitivity = authoring.playerConfig.Look,
                SprintSpeed = authoring.playerConfig.Sprint,
                groundLayer = authoring.playerConfig.groundLayer
            });
            AddComponent<PlayerInputData>(entity);
            AddComponent(entity, new HealthComponent
            {
                CurrentHealth = 100,
                MaxHealth = 100,
                IsAlive = true
            });
            AddComponent(entity, new RespawnComponent()
            {
                RespawnTime = 5f
            });
        }
    }
}