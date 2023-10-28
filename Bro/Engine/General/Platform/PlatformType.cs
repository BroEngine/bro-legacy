using System.ComponentModel;

namespace Bro
{
    public enum PlatformType : byte
    {
        [Description("undefined")] Undefined = 0,
        [Description("android")] Android = 1,
        [Description("ios")] iOS = 2,
        [Description("steam")] Standalone = 3,
        [Description("web")] WebGL = 5,
        [Description("console")] Console = 6
    }
}