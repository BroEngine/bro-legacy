using UnityEngine;

namespace Bro.Toolbox.Client
{
    public interface ICameraProjection
    {
        Rect CameraBounds { get; }
        void OnChangeViewPort();
    }
}