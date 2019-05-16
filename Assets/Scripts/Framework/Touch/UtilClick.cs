
using System;
using UnityEngine;

namespace Sword
{
    public class UtilClick
    {
        public static bool IsVaildClick = true;
        public static DateTime last = DateTime.Now;
        public static int LastFrameCount = 0;
        private static float DelayTime = 200;
        public static int FinishTouchFrame = 0;
        public static bool IsVaildTouch()
        {
            if(Time.frameCount < FinishTouchFrame)
            {
                return false;
            }
//            if (TaskMgr.Instance.IsRunning())
//            {
//                UtilLog.LogNormal("TaskMgr is running");
//                return false;
//            }

            if (!IsVaildClick)
            {
                Logger.Log("无效点击");
                return false;
            }

            DateTime now = DateTime.Now;
            var t = now - last;
            if (t.TotalMilliseconds > DelayTime)
            {
                return true;
            }
            return false;
        }

        public static void SetFinishTouchFrame()
        {
            FinishTouchFrame = Time.frameCount + 5;
        }
    }
}
