using UnityEngine;

namespace Core
{
    public class AudioService : IService
    {
        public void Initialize()
        {
            
        }

        public void Shutdown()
        {
            
        }

        public void Update()
        {
            
        }

        public void PlaySound(string soundName)
        {
            Debug.Log($"Playing sound: {soundName}");
        }
    }
}