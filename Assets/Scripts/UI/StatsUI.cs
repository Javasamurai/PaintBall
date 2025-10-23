using Core;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace UI
{
    public class StatsUI : UIView
    {
        [SerializeField] TextMeshProUGUI _ipText;
        [SerializeField]
        private TextMeshProUGUI _fpsText;
        [SerializeField]
        private TextMeshProUGUI _pingText;
        public string Ping
        {
            set => _pingText.text = value;
        }

        private float displayTimer = 0f;
        private float displayUpdateTime = 0.1f;
        private void Start()
        {
            foreach (var world in World.All)
            {
                if (world.IsClient() && !world.IsThinClient())
                {
                    var pingSystem = world.GetOrCreateSystemManaged<PingUISystem>();
                    pingSystem.StatsUI = this;
                    var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simulationSystemGroup.AddSystemToUpdateList(pingSystem);
                }
            }
            if (_ipText != null)
            {
                var localIP = Game.GetService<NetworkService>().GetLocalIPAddress();
                _ipText.text = $"IP: {localIP}";
            }
            else
            {
                Debug.LogWarning("IP Text is not assigned in StatsUI.");
            }
        }

        private void Update()
        {
            displayTimer += Time.unscaledDeltaTime;
            if (displayTimer >= displayUpdateTime)
            {
                displayTimer = 0f;
                if (_fpsText != null)
                {
                    _fpsText.text = $"FPS: {Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime)}";
                }
            }
        }
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class PingUISystem : SystemBase
    {
        public StatsUI StatsUI;
        private string pingText;
        
        protected override void OnUpdate()
        {
            CompleteDependency();
            if (SystemAPI.TryGetSingletonEntity<NetworkStreamConnection>(out var connectionEntity))
            {
                var connection = EntityManager.GetComponentData<NetworkStreamConnection>(connectionEntity);
                var address = SystemAPI.GetSingletonRW<NetworkStreamDriver>().ValueRO.GetRemoteEndPoint(connection).Address;
                
                if (EntityManager.HasComponent<NetworkId>(connectionEntity))
                {
                    if (UnityEngine.Time.frameCount % 30 == 0)
                    {
                        var networkSnapshotAck = EntityManager.GetComponentData<NetworkSnapshotAck>(connectionEntity); 
                        pingText = networkSnapshotAck.EstimatedRTT > 0 ? $"Ping: {networkSnapshotAck.EstimatedRTT}ms" : "Ping: Connected";
                    }
                }
                else
                {
                    pingText = "Ping: Not connected!";
                }
                
                StatsUI.Ping = pingText;
            }
            else
            {
                StatsUI.Ping = "Ping: Not connected!";
            }
        }
    }
}