using Unity.Mathematics;
using Unity.NetCode;

namespace Systems.Gameplay
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInputData : IInputComponentData
    {
        public float2 move;
        public float2 look;
        public InputEvent jump;
        public InputEvent sprint;
        public InputEvent shoot;
    }
}