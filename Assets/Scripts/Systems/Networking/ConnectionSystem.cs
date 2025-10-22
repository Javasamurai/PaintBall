using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateAfter(typeof(NetworkReceiveSystemGroup))]
[BurstCompile]
public partial struct ConnectionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamDriver>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var connectionEventsForClient = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;
        foreach (var evt in connectionEventsForClient)
        {
            UnityEngine.Debug.Log($"[{state.WorldUnmanaged.Name}] {evt.ToFixedString()}!");
        }
    }
}