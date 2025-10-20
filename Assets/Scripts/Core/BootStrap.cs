using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.NetCode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class BootStrap : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        
        private static bool _autoLoadBootStrapScene;
        private Game _game;

# if UNITY_EDITOR
        [MenuItem("Game/AutoLoadBootStrapScene")]
        private static void EnableAutoLoadBootStrapScene()
        {
            _autoLoadBootStrapScene = !_autoLoadBootStrapScene;
            EditorPrefs.SetBool(Utils.AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
            Menu.SetChecked(Utils.AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
        }
        
        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            _autoLoadBootStrapScene = EditorPrefs.GetBool(Utils.AUTO_LOAD_BOOTSTRAP_SCENE_KEY, true);
            Menu.SetChecked(Utils.AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartGame()
        {
            Setup();
        }
        # endif

        private void Start()
        {
            InitializeGame();
        }

        private static void Setup()
        {
            bool autoLoadBootStrapScene = true;
# if UNITY_EDITOR
            autoLoadBootStrapScene = EditorPrefs.GetBool(Utils.AUTO_LOAD_BOOTSTRAP_SCENE_KEY, true);
#endif
            
            // Load the BootStrap scene, because this is the first scene that should be loaded
            if (!autoLoadBootStrapScene)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != Utils.BOOTSTRAP_SCENE)
            {
                SceneManager.LoadScene(Utils.BOOTSTRAP_SCENE);
            }
        }

        private void InitializeGame()
        {
            _game = new Game(gameConfig);
            _game.Initialize();
        }

        private void Update()
        {
            _game?.Update();
        }

        private void OnDestroy()
        {
            _game?.Shutdown();
            _game = null;
        }
    }
}

