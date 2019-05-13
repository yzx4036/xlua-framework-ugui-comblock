using UnityEngine;
using XLua;
using HedgehogTeam.EasyTouch;

namespace Sword
{
    public class TouchLayerLua
    {
        public LayerInfo Layer
        {
            get
            {
                return _layer;
            }
        }
        private LayerInfo _layer;
        private LuaTable _luaModule;
        private const string FUNC_NAME_ON_TOUCH = "OnTap";
        private const string FUNC_NAME_ON_TOUCH_START = "OnTouchStart";
        private const string FUNC_NAME_ON_TOUCH_END = "OnTouchEnd";
        public TouchLayerLua(int layerIndex, LuaTable luaModule)
        {
            _luaModule = luaModule;

            _layer = new LayerInfo(layerIndex);
            _layer[TouchEventType.Tap] = OnTouchSceneObject;
            _layer[TouchEventType.TouchStart] = OnTouchStartSceneObject;
            _layer[TouchEventType.TouchEnd] = OnTouchEndSceneObject;
            _layer[TouchEventType.DragBegin] = delegate (Gesture gt)
            {
                return _call("DragBegin", gt);
            };
            _layer[TouchEventType.Drag] = delegate (Gesture gt)
            {
                return _call("Drag", gt);
            };
            _layer[TouchEventType.DragEnd] = delegate (Gesture gt)
            {
                return _call("DragEnd", gt);
            };
        }
        ~TouchLayerLua()
        {
            _layer = null;
            _luaModule = null;
        }

        private object[] OnTouchSceneObject(Gesture gt)
        {
            var go = gt.pickedObject;
            return _call(FUNC_NAME_ON_TOUCH, go);
        }
        private object[] OnTouchStartSceneObject(Gesture gt)
        {
            var go = gt.pickedObject;
            return _call(FUNC_NAME_ON_TOUCH_START, go);
        }
        private object[] OnTouchEndSceneObject(Gesture gt)
        {
            var go = gt.pickedObject;
            return _call(FUNC_NAME_ON_TOUCH_END, go);
        }

        internal object[] _call(string methodName, GameObject go)
        {
            if (_luaModule != null && go)
            {
                var func = _luaModule.Get<LuaFunction>(methodName);
                if (func != null)
                {
                    return func.Call(_luaModule, go);
//                    return (bool)func.Invoke<LuaTable, GameObject, bool>(_luaModule, go);
                }
            }
            return null;
        }
        internal object[] _call(string methodName, Gesture gt)
        {
            if (_luaModule != null && gt.pickedObject)
            {
                var func = _luaModule.Get<LuaFunction>(methodName);
                if (func != null)
                {
                    return func.Call(_luaModule, gt.pickedObject, gt.position, gt.deltaPosition);
//                    return (bool)func.Invoke<LuaTable, GameObject, Vector3, Vector3, bool>(_luaModule, gt.pickedObject, gt.position, gt.deltaPosition);
                }
            }
            return null;
        }
    }
}
