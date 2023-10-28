using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class InputInterceptor : MonoBehaviour
    {
        private InputControllerBase _inputController;
        public void Awake()
        {
#if UNITY_EDITOR
            _inputController = new DefaultInputController();
#else
            _inputController = new MobileInputController();
#endif
        }
        void Update()
        {
            _inputController.ProcessInput();
        }

        private void OnDestroy()
        {
            _inputController.Dispose();
        }
    }
}