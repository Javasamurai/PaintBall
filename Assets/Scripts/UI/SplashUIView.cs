using Core;
using UnityEngine.SceneManagement;
using DefaultNamespace;

namespace UI
{
    public class SplashUIView : UIView
    {
        private void Start()
        {
            Game.GetService<AudioService>().PlaySound("SplashSound");
            SceneManager.LoadSceneAsync(Utils.MAIN_MENU_SCENE, LoadSceneMode.Additive);
        }
    }
}