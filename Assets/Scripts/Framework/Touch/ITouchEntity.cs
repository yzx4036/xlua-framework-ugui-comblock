/*
	Creator: keemooc
    Created: 2017.01.09 AT 16:02:26
    Contact: 505297071@qq.com
*/
using UnityEngine;
using System.Collections;

namespace Sword
{
	public interface ITouchEntity
	{
        void DoTouch();
	}


    public static class TouchEventType
    {
        public const int None = 0;
        public const int Tap = 1;
        public const int TouchStart = 2;
        public const int TouchEnd = 13;

        public const int Swipe = 3;
        public const int SwipeBegin = 4;
        public const int SwipeEnd = 5;

        public const int Drag = 6;
        public const int DragBegin = 7;
        public const int DragEnd = 8;

        public const int PinchBegin = 9;
        public const int PinchIn = 10;
        public const int PinchOut = 11;
        public const int PinchEnd = 12;
        public const int TouchUI = 14;

        public static System.Collections.Generic.List<string> i2s = new System.Collections.Generic.List<string>
        {
            "None = 0;          ",
            "Tap = 1;           ",
            "TouchStart = 2;    ",

            "Swipe = 3;         ",
            "SwipeBegin = 4;    ",
            "SwipeEnd = 5;      ",

            "Drag = 6;          ",
            "DragBegin = 7;     ",
            "DragEnd = 8;       ",

            "PinchBegin = 9;    ",
            "PinchIn = 10;      ",
            "PinchOut = 11;     ",
            "PinchEnd = 12;     ",
            "TouchEnd = 13;     ",
        };
    }

    //priority: from High to Low
    public static class TouchLayerIndex
    {
        public const int None = -1;
        public const int Guide = int.MinValue;

        public const int TowerRange = 9000;
        public const int Attacker = 10000;
        public const int HeroSkillCast = 80000;
        public const int PVPEdit = 88888;
        public const int Camera = 90000;
        public const int EditorGuideUtil = int.MinValue;
    }
}
