using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private int maxPlayers = 10;
    }
}