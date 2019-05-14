/*
    Author: keemooc
    Date: 2016.05.13 AT 15:59:19
    E-Mail: 505297071@qq.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sword
{
    public static class ConstLayer
    {
        public const string sDefault = "Default";
        public const string sCamera = "Camera";
        public const string sAttacker = "Attacker";
        public const string sUI = "UI";
        public const string sUI_MAIN = "UI_MAIN";
        public const string sUI_POPUP = "UI_POPUP";
        public const string sUI_HUD = "UI_HUD";
        public const string sUI_GUIDE = "UI_GUIDE";

        public const string sFloor = "Floor";
        public const string sFloorLight = "FloorLight";
        public const string sRenderTexture = "RenderTexture";
        public const string sJiaose = "jiaos";
        public const string sGeneral = "General";
        public const string sWater = "Water";

        public static readonly int Default = LayerMask.NameToLayer(sDefault);
        public static readonly int Camera = LayerMask.NameToLayer(sCamera);
        public static readonly int Attacker = LayerMask.NameToLayer(sAttacker);
        public static readonly int General = LayerMask.NameToLayer(sGeneral);

        public static readonly int UI = LayerMask.NameToLayer(sUI);
        public static readonly int UI_MAIN = LayerMask.NameToLayer(sUI_MAIN);
        public static readonly int UI_POPUP = LayerMask.NameToLayer(sUI_POPUP);
        public static readonly int UI_HUD = LayerMask.NameToLayer(sUI_HUD);
        public static readonly int UI_GUIDE = LayerMask.NameToLayer(sUI_GUIDE);

        public static readonly int Floor = LayerMask.NameToLayer(sFloor);
        public static readonly int FloorLight = LayerMask.NameToLayer(sFloorLight);
        public static readonly int RenderTexture = LayerMask.NameToLayer(sRenderTexture);
        public static readonly int FxLayer = LayerMask.NameToLayer("FxSecond");
        public static readonly int WaterLayer = LayerMask.NameToLayer(sWater);

        public static readonly int EasyTouch = int.MaxValue;
        public static readonly int UICameraMask = LayerMask.GetMask(sUI);

        public static int GetUIMask()
        {
            return LayerMask.GetMask(
                sUI_MAIN,
                sUI_HUD,
                sUI_POPUP,
                sUI,
                sUI_GUIDE
                );
        }

        public static int GetMainMask()
        {
            return LayerMask.GetMask(
               sUI_MAIN,
                sUI,
                sUI_POPUP,
                sUI_GUIDE
                );
        }
        public static int GetHudMask()
        {
            return LayerMask.GetMask(
               sUI_HUD,
                sUI,
                sUI_GUIDE
                );
        }
        public static int GetPopUpMask()
        {
            return LayerMask.GetMask(
               sUI_POPUP,
                sUI,
                sUI_GUIDE
                );
        }
    }
}
