using System;
using Core;
using UnityEngine;

namespace Core
{
    public class SceneService : IService
    {
        public void Initialize()
        {
            
        }

        public void Update()
        {
            
        }

        public void Shutdown()
        {
            
        }

        public void LoadSceneAsync(string sceneName, bool additive = true, Action onComplete = null)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, additive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
            
            asyncOperation.completed += operation =>
            {
                if (operation.isDone)
                {
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to load scene {sceneName}");
                }
            };
        }
    }
}