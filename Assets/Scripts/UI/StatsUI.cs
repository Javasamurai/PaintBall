using System;
using Core;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace UI
{
    public class StatsUI : UIView
    {
        [SerializeField]
        private TextMeshProUGUI _fpsText;
        [SerializeField]
        private TextMeshProUGUI _pingText;

        private void Start()
        {
            _pingText.text = "Ping: Calculating...";
            
            foreach (var world in World.All)
            {
                if (world.IsClient())
                {
                    var pingSystem = world.GetOrCreateSystemManaged<PingUISystem>();
                    pingSystem.StatsUI = this;
                    var simulationSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simulationSystemGroup.AddSystemToUpdateList(pingSystem);
                }
            }
        }

        private void Update()
        {
            if (_fpsText != null)
            {
                _fpsText.text = $"FPS: {Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime)}";
            }
        }
    }
    
    public partial class PingUISystem : SystemBase
    {
        public StatsUI StatsUI;
        private string pingText;
        
        protected override void OnUpdate()
        {
            Debug.Log("Updating PingUI System");
        }
    }
}