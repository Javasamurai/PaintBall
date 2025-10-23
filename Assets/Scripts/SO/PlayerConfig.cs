using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] public float speed;
        [SerializeField] public float Look;
        [SerializeField] public float Sprint;
        [SerializeField] public uint groundLayer = 1;
    }
}