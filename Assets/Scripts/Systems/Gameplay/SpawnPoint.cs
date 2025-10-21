using Unity.Entities;
using UnityEngine;

namespace Systems.Gameplay
{
    public struct SpawnPointComponent : IComponentData
    {
        public bool occupied;
    }
    
    public struct SpawnPointTag : IComponentData
    {
        
    }
    
    public class SpawnPoint : MonoBehaviour
    {
        
    }
    
    public class SpawnPointBaker : Baker<SpawnPoint>
    {
        public override void Bake(SpawnPoint authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new SpawnPointComponent
            {
                occupied = false
            });
        }
    }
}