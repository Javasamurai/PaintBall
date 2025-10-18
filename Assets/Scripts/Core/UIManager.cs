using UnityEngine;

namespace Core
{
    public class UIManager : MonoBehaviour
    {
        [Tooltip("Default views that should show up")]
        [SerializeField]
        public UIView[] defaultViews;
        
        private void Start()
        {
            foreach (var view in defaultViews)
            {
                view.Show();
            }
        }
    }
}