using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class BootStrap : MonoBehaviour
    {
        private const string AUTO_LOAD_BOOTSTRAP_SCENE_KEY = "Game/AutoLoadBootStrapScene";

        private static bool _autoLoadBootStrapScene;
        private Game _game;

        [MenuItem("Game/AutoLoadBootStrapScene")]
        private static void EnableAutoLoadBootStrapScene()
        {
            _autoLoadBootStrapScene = !_autoLoadBootStrapScene;
            EditorPrefs.SetBool(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
            Menu.SetChecked(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
        }
        
        [InitializeOnLoadMethod]
        private static void OnEditorLoad()
        {
            _autoLoadBootStrapScene = EditorPrefs.GetBool(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, true);
            Menu.SetChecked(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartGame()
        {
            Setup();
        }

        private void Start()
        {
            InitializeGame();
        }

        private static void Setup()
        {
            var autoLoadBootStrapScene = EditorPrefs.GetBool(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, true);
            
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
            _game = new Game();
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

