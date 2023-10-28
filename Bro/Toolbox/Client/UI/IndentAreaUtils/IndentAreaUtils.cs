using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public static class IndentAreaUtils
    {
        public enum AnchoredSide
        {
            Undefined = -1,
            Left,
            Right
        }

#if UNITY_EDITOR
        static readonly Dictionary<string, Rect> _deviceTestScreens = new Dictionary<string, Rect>
        {

        };
#endif

        public static IndentAreaOffset GetIndentAreaOffset()
        {
            return new IndentAreaOffset(IndentArea);
        }

        public static Rect IndentArea
        {
            get
            {
#if UNITY_EDITOR
                //return _deviceTestScreens["motorola_one_action_4"];
#endif
                return Screen.safeArea;
            }
        }

        public struct IndentAreaOffset
        {
            public readonly float LeftOffset;
            public readonly float RightOffset;

            public IndentAreaOffset(Rect safeAreaRect)
            {
                RightOffset = safeAreaRect.xMax <= 0 ? 0 : -Screen.width + safeAreaRect.xMax;
                LeftOffset = safeAreaRect.xMin;
            }
        }
    }
}
