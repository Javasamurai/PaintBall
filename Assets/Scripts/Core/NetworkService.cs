using UnityEngine;
using Unity.NetCode;

namespace Core
{
    public class NetworkService : IService
    {
        public void Initialize()
        {
            var networkOveride = new GameObject("NetworkOverride");
            var overrideNetwork = networkOveride.AddComponent<OverrideAutomaticNetcodeBootstrap>();
            overrideNetwork.ForceAutomaticBootstrapInScene = NetCodeConfig.AutomaticBootstrapSetting.EnableAutomaticBootstrap;
        }

        public void Update()
        {
        }

        public void Shutdown()
        {
            
        }
    }
}