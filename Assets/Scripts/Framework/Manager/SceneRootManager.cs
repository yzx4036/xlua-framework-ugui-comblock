using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using HedgehogTeam.EasyTouch;
using Object = UnityEngine.Object;

namespace Sword
{
    public class SceneRootManager : Singleton<SceneRootManager>
    {
        public SceneRootManager()
        {
            Root = GameObject.Find("Scene");
            if (!Root)
            {
                Root = new GameObject("Scene");
                Object.DontDestroyOnLoad(Root);
            }
        }
        public enum RootTag
        {
            Entity,
            Event,
            Flag,
            Map,
            Audio,
            Pandant,
            Pool,
            Effect,
			Lady,
        }
        private GameObject Root;
        private Dictionary<int, GameObject> _tag2go = new Dictionary<int, GameObject>();
        public GameObject GetRoot(RootTag tag)
        {
            int index = (int)tag;
            if (_tag2go.ContainsKey(index))
            {
                return _tag2go[index];
            }
            else
            {
                return InitRoot(tag);
            }
        }
        public void Remove(RootTag tag)
        {
            int index = (int)tag;
            if (_tag2go.ContainsKey(index))
            {
                _tag2go[index].DestroySelf();
                _tag2go.Remove(index);
            }
        }
        public GameObject AddTo(RootTag tag, GameObject child)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return child;
            }
#endif
            //#if UNITY_EDITOR
            child.transform.SetParent(GetRoot(tag).transform);
//#endif
            return child;
        }
        public T AddComp<T>(RootTag tag) where T : MonoBehaviour
        {
            return GetRoot(tag).AddComponent<T>();
        }
        public T AddComp<T>() where T : Component
        {
            return Root.AddComponent<T>();
        }
        public Component AddComp(Type type)
        {
            return Root.AddComponent(type);
        }
        private GameObject InitRoot(RootTag tag)
        {
            var go = Root.GetChild(tag.ToString());
            if (!go)
            {
                go = Root.AddNewChildTo(tag.ToString());
            }
            int index = (int)tag;
            _tag2go[index] = go;
            return _tag2go[index];
        }

        public override void Init()
        {
            var touchComp = GetRoot(RootTag.Event).GetComponent<EasyTouch>();
            if (touchComp == null)
            {
                GetRoot(RootTag.Event).AddComponent<EasyTouch>();
                EasyTouch.Set3DPickableLayer(ConstLayer.EasyTouch);
   
            }
        }

        public override void Dispose()
        {
//            throw new NotImplementedException();
        }
    }
}
