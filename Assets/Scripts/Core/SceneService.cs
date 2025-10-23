using System;
using DefaultNamespace;
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
            // ShowLoadingScreen();
            if (!additive)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                if (currentScene.IsValid() && currentScene.name != Utils.BOOTSTRAP_SCENE)
                {
                    SceneManager.UnloadSceneAsync(currentScene);
                }
            }
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            asyncOperation.completed += operation =>
            {
                if (operation.isDone)
                {
                    var scene = SceneManager.GetSceneByName(sceneName);
                    if (scene.IsValid())
                    {
                        onComplete?.Invoke(scene);
                        // HideLoadingScreen();
                    }
                    else
                    {
                        onComplete?.Invoke(default);
                        // HideLoadingScreen();
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load scene {sceneName}");
                    onComplete?.Invoke(default);
                    // HideLoadingScreen();
                }
            };
            // Load sub-scenes if any, because ECS follow sub-scene and bake into the server world, in the background
            LoadSubScenes();
        }

        private void ShowLoadingScreen()
        {
            SceneManager.LoadScene(Utils.LOADING_SCENE, LoadSceneMode.Additive);
        }
        
        private void HideLoadingScreen()
        {
            SceneManager.UnloadSceneAsync(Utils.LOADING_SCENE);
        }

        private void LoadSubScenes()
        {
            SubScene[] subScenes = Object.FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            World clientWorld = Game.Instance.ClientWorld;
            World serverWorld = Game.Instance.ServerWorld;
            
            if (subScenes == null || subScenes.Length == 0)
            {
                return;
            }

            if (serverWorld != null)
            {
                foreach (var subScene in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters
                    {
                        Flags = SceneLoadFlags.BlockOnStreamIn
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