using System.Net;
using System.Net.Sockets;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Core
{
    public class NetworkService : IService
    {
        // private const string IP_ADDRESS_LISTEN = "127.0.0.1";
        // private const string IP_ADDRESS_CONNECT = "127.0.0.1";
        private const int _port = 7979;
        
        public void Initialize()
        {
            #if UNITY_EDITOR
            Game.Instance.CreateWorlds();
            var localIP = GetLocalIPAddress();
            StartConnection(localIP, _port);
            #endif
        }
        
        public string GetLocalIPAddress()
        {
            string localIP = "127.0.0.1";

            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    if (socket.LocalEndPoint is IPEndPoint endPoint)
                        localIP = endPoint.Address.ToString();
                }
            }
            catch
            {
                localIP = "127.0.0.1";
            }

            return localIP;
        }
        public void StartConnection(string Ip_Address, ushort port)
        {
            if (Game.Instance.ServerWorld != null)
            {
                NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(port);
                using var drvQuery = Game.Instance.ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(Ip_Address, port));
            }

            if (Game.Instance.ClientWorld != null)
            {
                IPAddress ipAddress = IPAddress.Parse(Ip_Address);
                NativeArray<byte> nativeArray =
                    new NativeArray<byte>(ipAddress.GetAddressBytes().Length, Allocator.Temp);
                
                nativeArray.CopyFrom(ipAddress.GetAddressBytes());
                NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
                endpoint.SetRawAddressBytes(nativeArray);
                endpoint.Port = port;
                
                using var drvQuery = Game.Instance.ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(Game.Instance.ClientWorld.EntityManager, endpoint);
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