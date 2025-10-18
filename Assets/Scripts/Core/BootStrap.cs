using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class BootStrap : MonoBehaviour
    {
        public const string BOOTSTRAP_SCENE_NAME = "BootStrap_Scene";
        public const string MENU_SCENE_NAME = "Menu_Scene";

        private const string AUTO_LOAD_BOOTSTRAP_SCENE_KEY = "Game/AutoLoadBootStrapScene";

        private static bool _autoLoadBootStrapScene;

        [MenuItem("Game/AutoLoadBootStrapScene")]
        private static void EnableAutoLoadBootStrapScene()
        {
            _autoLoadBootStrapScene = !_autoLoadBootStrapScene;
            EditorPrefs.SetBool(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
            Menu.SetChecked(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, _autoLoadBootStrapScene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartGame()
        {
            Setup();
        }

        private void Start()
        {
            if (SceneManager.loadedSceneCount == 1)
            {
                SceneManager.LoadSceneAsync(MENU_SCENE_NAME, LoadSceneMode.Additive);
            }

            InitializeServices();
        }

        private static void Setup()
        {
            var autoLoadBootStrapScene = EditorPrefs.GetBool(AUTO_LOAD_BOOTSTRAP_SCENE_KEY, true);
            // Load the BootStrap scene, because this is the first scene that should be loaded
            if (!autoLoadBootStrapScene)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != BOOTSTRAP_SCENE_NAME)
            {
                SceneManager.LoadScene(BOOTSTRAP_SCENE_NAME);
            }
        }

        private void InitializeServices()
        {
            Game game = new Game();
        }
    }
}

