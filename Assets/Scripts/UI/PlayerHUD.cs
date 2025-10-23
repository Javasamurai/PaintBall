using Core;
using Systems.Gameplay;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace UI
{
    public class PlayerHUD : UIView
    {
        [SerializeField]
        private TextMeshPro playerNameText;

        private void Start()
        {
            foreach (var world in World.All)
            {
                if (world.IsClient() && !world.IsThinClient())
                {
                    var hudSystem = world.GetOrCreateSystemManaged<HUDUISystem>();
                    hudSystem.PlayerHUD = this;
                    var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simulationSystemGroup.AddSystemToUpdateList(hudSystem);
                }
            }
        }
        
        public void SetName(string name)
        {
            if (playerNameText != null)
            {
                playerNameText.text = name;
            }
        }
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class HUDUISystem : SystemBase
    {
        private string pingText;
        public PlayerHUD PlayerHUD;
        
        protected override void OnUpdate()
        {
            CompleteDependency();
            foreach (var (player, entity) in SystemAPI.Query<RefRW<PlayerData>>().WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<GhostOwnerIsLocal>(entity))
                {
                    if (PlayerHUD != null && player.ValueRO.PlayerName != null)
                    {
                        PlayerHUD.SetName(player.ValueRO.PlayerName.ToString());
                    }
                }
            }
        }
    }
}