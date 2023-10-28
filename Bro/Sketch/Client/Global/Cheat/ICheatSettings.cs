using System;
using System.Collections.Generic;

namespace Bro.Sketch.Client
{
    public interface ICheatSettings
    {
        string WindowPrefab { get; }
        string ButtonPrefab { get; }

        Dictionary<Enum, string> PopupPrefabs { get; }
    }
}