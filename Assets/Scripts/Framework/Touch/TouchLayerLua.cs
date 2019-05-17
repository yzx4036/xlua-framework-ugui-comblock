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
        
        public LuaTable TargetLua
        {
            get
            {
                return _luaModule;
            }
        }
        private LayerInfo _layer;
        private LuaTable _luaModule;
        private const string FUNC_NAME_ON_TOUCH = "OnTap";
        private const string FUNC_NAME_ON_TOUCH_START = "OnTouchStart";
        private const string FUNC_NAME_ON_TOUCH_END = "OnTouchEnd";
        private const string FUNC_NAME_ON_DRAG = "OnDrag";
        private const string FUNC_NAME_ON_DRAG_START = "OnDragStart";
        private const string FUNC_NAME_ON_DRAG_END = "OnDragEnd";

        public TouchLayerLua(int layerIndex, LuaTable luaModule)
        {
            _luaModule = luaModule;

            _layer = new LayerInfo(layerIndex);
            _layer[TouchEventType.Tap] = OnTouchSceneObject;
            _layer[TouchEventType.TouchStart] = OnTouchStartSceneObject;
            _layer[TouchEventType.TouchEnd] = OnTouchEndSceneObject;
            _layer[TouchEventType.DragBegin] = delegate (Gesture gt)
            {
                return _call(FUNC_NAME_ON_DRAG_START, gt);
            };
            _layer[TouchEventType.Drag] = delegate (Gesture gt)
            {
                return _call(FUNC_NAME_ON_DRAG, gt);
            };
            _layer[TouchEventType.DragEnd] = delegate (Gesture gt)
            {
                return _call(FUNC_NAME_ON_DRAG_END, gt);
            };
        }
        ~TouchLayerLua()
        {
            _layer = null;
            _luaModule = null;
        }

        private bool OnTouchSceneObject(Gesture gt)
        {
            return _call(FUNC_NAME_ON_TOUCH, gt);
        }
        private bool OnTouchStartSceneObject(Gesture gt)
        {
            return _call(FUNC_NAME_ON_TOUCH_START, gt);
        }
        private bool OnTouchEndSceneObject(Gesture gt)
        {
            return _call(FUNC_NAME_ON_TOUCH_END, gt);
        }

        internal bool _call(string methodName, Gesture go)
        {
            if (_luaModule != null && go != null)
            {
                var func = _luaModule.Get<EventCallLua>(methodName);
                if (func != null)
                {
                    return func(_luaModule, go);
                }
            }
            return false;
        }
//        internal object[] _call(string methodName, Gesture gt)
//        {
//            if (_luaModule != null && gt.pickedObject)
//            {
//                var func = _luaModule.Get<LuaFunction>(methodName);
//                if (func != null)
//                {
//                    return func.Call(_luaModule, gt.pickedObject, gt.position, gt.deltaPosition);
////                    return (bool)func.Invoke<LuaTable, GameObject, Vector3, Vector3, bool>(_luaModule, gt.pickedObject, gt.position, gt.deltaPosition);
//                }
//            }
//            return null;
//        }
    }
}
