using System.Collections;
using Core;
using UnityEngine.SceneManagement;
using DefaultNamespace;
using UnityEngine;

namespace UI
{
    public class SplashUIView : UIView
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private void Start()
        {
            StartCoroutine(ShowSplash());
        }
        
        private IEnumerator ShowSplash()
        {
            yield return new WaitWhile(() => Game.IsReady);

            _canvasGroup.alpha = 1;
            while (_canvasGroup.alpha > 0f)
            {
                _canvasGroup.alpha -= Time.deltaTime;
                yield return null;
            }
            
            if (false)
            {
                Game.GetService<SceneService>().LoadSceneAsync(Utils.PLAYGROUND_SCENE, true, (scene) =>
                {
                    if (scene == default) return;
                    SceneManager.SetActiveScene(scene);
                    DeactivateCanvas();
                });
            }
            else
            {
                Game.GetService<SceneService>().LoadSceneAsync(Utils.MAIN_MENU_SCENE, true, (scene) =>
                {
                    if (scene == default) return;
                    SceneManager.SetActiveScene(scene);
                    DeactivateCanvas();
                });
            }
        }
        
        private void DeactivateCanvas()
        {
            _canvasGroup.alpha = 0f;
            _canvas.enabled = false;
        }
    }
}