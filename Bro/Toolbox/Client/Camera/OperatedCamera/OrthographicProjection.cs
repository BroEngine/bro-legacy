using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class OrthographicProjection : ICameraProjection
    {
        private readonly Camera _camera;
        private float _screenRatio;
        public OrthographicProjection(Camera camera)
        {
            _camera = camera;
            RefreshScreenRatio();
        }
        
        Rect ICameraProjection.CameraBounds
        {
            get
            {
                var position3d = _camera.transform.localPosition;
                var position = new Vector2(position3d.x, position3d.y);
                var orthographicSize = _camera.orthographicSize;
                var quarter = new Vector2(orthographicSize * _screenRatio, orthographicSize);
                return new Rect(position - quarter, quarter * 2f);
            }
        }

        void ICameraProjection.OnChangeViewPort()
        {
            RefreshScreenRatio();
        }

        private void RefreshScreenRatio()
        {
            _screenRatio = ((float) Screen.width) / ((float) Screen.height);
        }
    }
}