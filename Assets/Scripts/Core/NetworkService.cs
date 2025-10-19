using DefaultNamespace;
using Unity.Entities;
using UnityEngine;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Core
{
    public class NetworkService : IService
    {
        private const int _port = 7979;
        public void Initialize()
        {
            var networkOverride = new GameObject("NetworkOverride");
            var overrideNetwork = networkOverride.AddComponent<OverrideAutomaticNetcodeBootstrap>();
            overrideNetwork.ForceAutomaticBootstrapInScene = NetCodeConfig.AutomaticBootstrapSetting.EnableAutomaticBootstrap;
            StartConnection();
        }
        
        private void StartConnection()
        {
            // NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(_port);
            // using var drvQuery = ClientServerBootstrap.ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            // drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ClientServerBootstrap.DefaultListenAddress.WithPort(_port));
            
            // Start server
            // using var drvQuery = ClientServerBootstrap.ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            // drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientServerBootstrap.ClientWorld.EntityManager, ep);

            if (NetCodeBootstrap.IsBootstrappingEnabledForScene())
            {
                Game.GetService<SceneService>().LoadSceneAsync(Utils.PLAYGROUND_SCENE);
            }
        }

        public void Update()
        {
            
        }

        public void Shutdown()
        {
            
        }
    }
}