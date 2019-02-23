/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
    public abstract class InputManager:MonoBehaviour, IInputManager
    {
        //Static members
        private static InputManager instance;
        public static IInputManager Instance { get { return instance; } }

        public static void SetInstance(InputManager instance)
        {
            if(InputManager.instance == instance) return;
            if(InputManager.instance != null) { // disable existing input manager
                InputManager.instance.Enabled = false; // disable the old manager
            }
            InputManager.instance = instance;
        }

        //Instance members
        protected ITouchInputManager touchInputManager;

        public ITouchInputManager TouchInputManager { get { return touchInputManager; } }

        protected virtual bool UseTouchInput
        {
            get
            {
                if(touchInputManager == null) return false;
                return touchInputManager.Enabled;
            }
        }

        protected virtual void Awake()
        {
            touchInputManager = GetComponent<ITouchInputManager>();
        }

        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void OnEnable(){}
        protected virtual void OnDisable(){}
        protected virtual void OnDestroy(){}

        public virtual bool Enabled {
            get
            {
                return this.enabled;
            }
            set
            {
                if(touchInputManager != null && touchInputManager != this as ITouchInputManager) touchInputManager.Enabled = value;
                this.enabled = value;
            }
        }
        
        public abstract bool GetButton(int playerId, InputAction action);
        public abstract bool GetButtonDown(int playerId, InputAction action);
        public abstract bool GetButtonUp(int playerId, InputAction action);
        public abstract float GetAxis(int playerId, InputAction action);
    }
}
