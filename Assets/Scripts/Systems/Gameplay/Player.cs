using Unity.Entities;
using UnityEngine;

namespace Systems.Gameplay
{
    public struct PlayerData : IComponentData
    {
        public float MoveSpeed;
        public float SprintSpeed;
    }

    public class Player : MonoBehaviour
    {
        [SerializeField] public float speed;
    }

    public class PlayerBaker : Baker<Player>
    {
        public override void Bake(Player authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
                
            // Move this to a config
            AddComponent(entity, new PlayerData
            {
                MoveSpeed = authoring.speed
            });
            AddComponent<PlayerInputData>(entity);
        }
    }
}