using UnityEngine;
using UnityEngine.EventSystems;

namespace Sword
{
    public class EventManager : Singleton<EventManager>
    {
        GameObject mEvent;
        GameObject GOEventSystem
        {
            get
            {
                if (mEvent == null)
                {
                    mEvent = GameObject.Find("EventSystem");
                    Object.DontDestroyOnLoad(mEvent);
                }
                return mEvent;
            }
        }
        private EventSystem _current;
        public EventSystem Current
        {
            get
            {
                if (!_current)
                {
                    _current = GOEventSystem.GetComponent<EventSystem>();
                }
                return _current;
            }
        }
        public StandaloneInputModule Module;
        private int Count = 0;

        public EventManager()
        {

        }
        public void SetEnable(bool enable)
        {
            if (enable)
            {
                Count--;
            }
            else
            {
                Count++;
            }
            if (Count == 0)
            {
                UtilClick.IsVaildClick = true;
            }
            else
            {
                UtilClick.IsVaildClick = false;
            }
            if (Count < 0)
            {
                Debug.LogError("error " + enable);
            }
            //GOEventSystem.SetActive(enable);
        }

        public override void Init()
        {
            if (Module == null)
            {
                Module = GOEventSystem.GetComponent<StandaloneInputModule>();
                Module.forceModuleActive = true;
            }
        }

        public override void Dispose()
        {
//            throw new System.NotImplementedException();
        }
    }
}
