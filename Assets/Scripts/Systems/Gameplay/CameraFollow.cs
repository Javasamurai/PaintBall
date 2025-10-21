using Cinemachine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Gameplay
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        CinemachineVirtualCamera virtualCamera;
        
        [SerializeField]
        private Transform cameraTarget;

        private void Start()
        {
            foreach (var world in World.All)
            {
                if (world.IsClient() && !world.IsThinClient())
                {
                    var cameraFollowSystem = world.GetOrCreateSystemManaged<CameraFollowSystem>();
                    cameraFollowSystem.cameraTarget = cameraTarget;
                    virtualCamera.Follow = cameraTarget;
                    var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simulationSystemGroup.AddSystemToUpdateList(cameraFollowSystem);
                }
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class CameraFollowSystem : SystemBase
    {
        public Transform cameraTarget;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<PlayerData>(), ComponentType.ReadOnly<GhostOwnerIsLocal>()));
        }

        protected override void OnUpdate()
        {
            if (cameraTarget == null)
            {
                Debug.LogWarning("Virtual Camera is not assigned in CameraFollowSystem.");
                return;
            }

            Entity localPlayerEntity = Entity.Null;
            foreach (var (networkStream, entity) in SystemAPI.Query<RefRO<PlayerData>>().WithAny<GhostOwnerIsLocal>().WithEntityAccess())
            {
                localPlayerEntity = entity;
                break;
            }

            if (localPlayerEntity != Entity.Null)
            {
                // Get the position of the local player
                var localTransform = SystemAPI.GetComponent<LocalTransform>(localPlayerEntity);
                cameraTarget.position = localTransform.Position;
            }
        }
    }
}