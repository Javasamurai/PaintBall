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
            while (_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha -= Time.deltaTime / 2f;
                yield return null;
            }
            // var asyncOperation = SceneManager.LoadSceneAsync(Utils.MAIN_MENU_SCENE, LoadSceneMode.Additive);
            // asyncOperation.allowSceneActivation = true;
            // asyncOperation.completed += operation =>
            // {
                _canvasGroup.alpha = 0f;
                _canvas.enabled = false;
            // };
        }
    }
}