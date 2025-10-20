using DefaultNamespace;

namespace Core
{
    public class PlayerServices : IService
    {
        private bool _isSpawned = false;
        private bool _canSpawn => Game.GetService<SceneService>().GetActiveSceneName() == Utils.PLAYGROUND_SCENE;
        public bool IsSpawned => _isSpawned;
        public bool CanSpawn => _canSpawn;
        
        
        public void Initialize()
        {
            _isSpawned = false;
        }

        public void SpawnPlayer()
        {
            _isSpawned = true;
        }
        
        public void DespawnPlayer()
        {
            _isSpawned = false;
        }

        public void Update()
        {
            
        }

        public void Shutdown()
        {
        }
    }
}