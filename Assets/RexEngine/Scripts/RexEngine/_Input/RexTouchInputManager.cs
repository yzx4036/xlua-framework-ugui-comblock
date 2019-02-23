/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
    public class RexTouchInputManager:MonoBehaviour, ITouchInputManager
    {
        protected bool isInitialTouchSet;
        protected Vector2 initialTouchPosition;
        protected Camera uiCamera;
        protected int collisionLayerMask;
        protected GameObject touchInterfaceObject;

        private InputState[] inputStates;

        protected bool isTouchInterfaceEnabled = false;

        public virtual bool Enabled
        {
            get
            {
                return this.isActiveAndEnabled;
            }
            set
            {
                if(!value) ToggleTouchInterface(false); // disable touch ui when disabled
                this.enabled = value;
            }
        }

        protected virtual void Awake()
        {
            int[] values = (int[])System.Enum.GetValues(typeof(InputAction));
            inputStates = new InputState[values.Length];
            for(int i = 0; i < values.Length; i++)
            {
                inputStates[values[i]] = new InputState();
            }
        }

        protected virtual void Start()
        {
            collisionLayerMask = 1 << LayerMask.NameToLayer("UI");

			if(GameObject.Find("UICamera"))
			{
				uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
			}

			#if UNITY_ANDROID || UNITY_IPHONE
			isTouchInterfaceEnabled = true;
			#endif

			ToggleTouchInterface(isTouchInterfaceEnabled);
        }

        protected virtual void OnDisable()
        {
            ClearInputs();
        }

        protected virtual void Update()
        {
            ClearInputs();

            //float halfWidth = Screen.width * 0.5f;
            Vector3 mousePosition = Vector3.zero;
            bool isTouchStartingThisFrame = true;

            //#if UNITY_IPHONE || UNITY_ANDROID
            if(Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                if(Input.touchCount > 0)
                {
                    for(int i = 0; i < Input.touchCount; i++)
                    {
                        mousePosition = Input.touches[i].position;
                        isTouchStartingThisFrame = (Input.touches[i].phase == TouchPhase.Began) ? true : false;
                        CheckTouchAtPosition(mousePosition, isTouchStartingThisFrame);
                    }
                }
                else
                {
                    mousePosition = Input.mousePosition;
                    isTouchStartingThisFrame = (Input.GetMouseButtonDown(0)) ? true : false;
                    CheckTouchAtPosition(mousePosition, isTouchStartingThisFrame);
                }
            }
            //#endif
        }

        public virtual void ToggleTouchInterface(bool willShow)
        {
			if(!isTouchInterfaceEnabled && willShow)
			{
				return;
			}

            if(touchInterfaceObject == null)
            {
                touchInterfaceObject = GameObject.Find("TouchInterface");
            }

            if(touchInterfaceObject != null)
            {
                touchInterfaceObject.SetActive(willShow);
            }
        }

        protected virtual void CheckTouchAtPosition(Vector3 position, bool isTouchStartingThisFrame = false)
        {
			if(uiCamera == null)
			{
				return;
			}

            RaycastHit2D hit = Physics2D.Raycast(uiCamera.ScreenToWorldPoint(position), Vector2.zero, 0.0f, collisionLayerMask);
            InputState input;

            if((hit.collider && hit.collider.name == "JumpButton"))
            {
                input = GetInput(InputAction.Jump);
            }
            else if((hit.collider && hit.collider.name == "AttackButton"))
            {
                input = GetInput(InputAction.Attack);
            }
            else if((hit.collider && hit.collider.name == "SubAttackButton"))
            {
                input = GetInput(InputAction.SubAttack);
            }
            else input = null;

            if(input != null)
            {
                input.SetButton(ButtonState.On | (isTouchStartingThisFrame ? ButtonState.Down : ButtonState.Off));
            }

            if((hit.collider && hit.collider.name == "LeftButton"))
            {
                GetInput(InputAction.MoveHorizontal).SetAxis(-1.0f);
            }
            else if((hit.collider && hit.collider.name == "RightButton"))
            {
                GetInput(InputAction.MoveHorizontal).SetAxis(1.0f);
            }
            else if((hit.collider && hit.collider.name == "UpButton"))
            {
                GetInput(InputAction.MoveVertical).SetAxis(1.0f);
            }
            else if((hit.collider && hit.collider.name == "DownButton"))
            {
                GetInput(InputAction.MoveVertical).SetAxis(-1.0f);
            }
        }

        public virtual bool GetButton(int playerId, InputAction action)
        {
            return (inputStates[(int)action].buttonState & ButtonState.On) != 0;
        }

        public virtual bool GetButtonDown(int playerId, InputAction action)
        {
            return (inputStates[(int)action].buttonState & ButtonState.Down) != 0;
        }

        public virtual bool GetButtonUp(int playerId, InputAction action)
        {
            return (inputStates[(int)action].buttonState & ButtonState.Up) != 0;
        }

        public virtual float GetAxis(int playerId, InputAction action)
        {
            return inputStates[(int)action].axisValue;
        }

        private InputState GetInput(InputAction action)
        {
            return inputStates[(int)action];
        }

        private void ClearInputs()
        {
            for(int i = 0; i < inputStates.Length; i++)
            {
                inputStates[i].Clear();
            }
        }

        private class InputState
        {
            public float axisValue;
            public ButtonState buttonState;

            public void SetAxis(float value)
            {
                axisValue = value;
                buttonState = value >= 1f || value <= -1f ? ButtonState.On : ButtonState.Off;
            }

            public void SetButton(ButtonState state)
            {
                this.buttonState = state;
                if((state & ButtonState.On) == 0) axisValue = 0f;
            }

            public void Clear()
            {
                axisValue = 0f;
                buttonState = ButtonState.Off;
            }
        }

        [System.Flags]
        private enum ButtonState
        {
            Off = 0,
            On = 1,
            Down = 2,
            Up = 3
        }
    }
}