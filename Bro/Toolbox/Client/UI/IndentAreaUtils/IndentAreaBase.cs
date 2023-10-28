using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class IndentAreaBase : MonoBehaviour
    {
        protected IndentAreaUtils.AnchoredSide AnchoredSide => _anchoredSide;

        [SerializeField] private IndentAreaUtils.AnchoredSide _anchoredSide;
        private ScreenOrientation _curOrientation = 0;
        
        protected virtual void Start()
        {
            _curOrientation = Screen.orientation;

            UpdateOffsets();
        }
    
        protected abstract void UpdateOffsets();
        
        private void Update()
        {
            if (Screen.orientation != _curOrientation)
            {
                _curOrientation = Screen.orientation;
                UpdateOffsets();
            }
        }
    }
}