using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public void LoadSceneAsync(string sceneName, bool additive = true, Action<Scene> onComplete = null)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
            
            asyncOperation.completed += operation =>
            {
                if (operation.isDone)
                {
                    var scene = SceneManager.GetSceneByName(sceneName);
                    if (scene.IsValid())
                    {
                        onComplete?.Invoke(scene);
                    }
                    else
                    {
                        onComplete?.Invoke(default);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load scene {sceneName}");
                    onComplete?.Invoke(default);
                }
            };
        }
    }
}