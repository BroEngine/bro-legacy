using System;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class PerspectiveProjection : ICameraProjection
    {
        private readonly Camera _camera;
        private float _screenRatio;
        public PerspectiveProjection(Camera camera)
        {
            _camera = camera;
            
        }
        
        Rect ICameraProjection.CameraBounds => throw new NotImplementedException();

        public void OnChangeViewPort()
        {
            throw new NotImplementedException(); 
        }

    }
}