using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client
{
    public class CheatWindow : MonoBehaviour
    {   
        [SerializeField] private Transform _scrollContent;
        [SerializeField] private Transform _popupContent;
        [SerializeField] private Button _buttonClose;

        private CheatModule _cheatModule;
        
        public Transform ScrollTransform => _scrollContent.transform;
        public Transform PopupTransform => _popupContent.transform;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _buttonClose.onClick.AddListener(OnButtonClose);
        }

        private void OnButtonClose()
        {
            Hide();
        }

        public void Setup(CheatModule cheatModule)
        {
            _cheatModule = cheatModule;
        }

        public void Hide()
        {
            _cheatModule.Hide();
        }
    }
}
