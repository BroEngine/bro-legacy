// ----------------------------------------------------------------------------------------------------------------------
// <summary>BroEngine - Cross-platform multithreading UDP/TCP network engine.</summary>
// <remarks>See the License Agreement at https://brogames.org/docs/license.pdf</remarks>
// <copyright>BroEngine - Copyright (C) 2017 Partnership between BroGames and Drunken Monday LLC, Russian Federation</copyright>
// <contacts>browillhelp@gmail.com</contacts>
// ----------------------------------------------------------------------------------------------------------------------

namespace Bro.Sketch.Network.TransmitProtocol
{
    public static class Converters
    {
        public static readonly ShortFloatConverter ZeroOneConverter = new ShortFloatConverter(0.0f, 1.0f);
    }
}