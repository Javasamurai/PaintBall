using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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


        public void LoadSceneAsync(string sceneName, bool additive = true, Action<Scene> onComplete = null)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);

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
            // Load sub-scenes if any, because ECS follow sub-scene and bake into the server world, in the background
            LoadSubScenes();
        }
        
        private void LoadSubScenes()
        {
            
            SubScene[] subScenes = Object.FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            World clientWorld = Game.Instance.ClientWorld;
            World serverWorld = Game.Instance.ServerWorld;

            if (serverWorld != null)
            {
                foreach (var subScene in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters
                    {
                        Flags = SceneLoadFlags.BlockOnImport
                    };
                    var sceneEntity = SceneSystem.LoadSceneAsync(serverWorld.Unmanaged, new Unity.Entities.Hash128(subScene.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(serverWorld.Unmanaged, sceneEntity))
                    {
                        serverWorld.Update();
                    }
                }
            }

            if (clientWorld != null)
            {
                foreach (var subScene in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters
                    {
                        Flags = SceneLoadFlags.BlockOnImport
                    };
                    var sceneEntity = SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, new Unity.Entities.Hash128(subScene.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(clientWorld.Unmanaged, sceneEntity))
                    {
                        clientWorld.Update();
                    }
                }
            }
        }
        
        public string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        public void Shutdown()
        {
            // TODO: Any scene cleanup if necessary
        }
    }
}