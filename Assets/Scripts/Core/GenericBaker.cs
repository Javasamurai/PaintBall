// using Unity.Entities;
// using UnityEngine;
//
// namespace Core
// {
//     public class GenericBaker<T> : Baker<T> where T : MonoBehaviour
//     {
//         public override void Bake(T authoring)
//         {
//             var entity = GetEntity(TransformUsageFlags.Dynamic);
//             AddComponentObject(entity, authoring);
//         }
//     }
// }