using System;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public struct SpawnerData : IComponentData
    {
        public Entity PlayerPrefab;
    }
    
    [DisallowMultipleComponent]
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField]
        public GameObject playerPrefab;
        class PlayerSpawnerBaker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                if (authoring.playerPrefab != null)
                {
                    var entity=  GetEntity(TransformUsageFlags.Dynamic);
                    AddComponent(entity, new SpawnerData
                    {
                        PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}