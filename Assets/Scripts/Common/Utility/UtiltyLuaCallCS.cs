using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sword;
using UnityEngine;
using XLua;

namespace Util
{
        public class UtiltyLuaCallCS
        {
            #region gameObject

            

            #endregion
            
            #region touch
            
            #region Touch
            public static TouchLayerLua CreateTouchLayer(int key, LuaTable module)
            {
                return new TouchLayerLua(key, module);
            }
            public static void AddTouchLayer(TouchLayerLua layer)
            {
                TouchManager.instance.AddLayer(layer.Layer);
            }
            public static void RemoveTouchLayer(TouchLayerLua layer)
            {
                TouchManager.instance.RemoveLayer(layer.Layer);
            }
            #endregion

            public static void AddCameraToEasyTouch(GameObject cameraGo)
                    {
                        if (cameraGo)
                        {
                            var camera = cameraGo.transform.GetComponent<Camera>();
                            if (camera)
                            {
                                var  sourceCam = HedgehogTeam.EasyTouch.EasyTouch.GetCamera();
                                HedgehogTeam.EasyTouch.EasyTouch.AddCamera(camera);
                            }
                        }
                    }
            
                    public static void RemoveCameraFromEasyTouch(GameObject cameraGo)
                    {
                        if (cameraGo)
                        {
                            var camera = cameraGo.transform.GetComponent<Camera>();
                            if (camera) HedgehogTeam.EasyTouch.EasyTouch.RemoveCamera(camera);
                        }
                    }

            #endregion
        }
}
