/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

namespace RexEngine
{
    public class RexInput:MonoBehaviour
    {
		public bool isEnabled = true;
        public int playerId = 0;
		public bool willAcceptInputWhenPaused;

		[HideInInspector]
        public bool isJumpButtonDown = false;
		[HideInInspector]
        public bool isJumpButtonDownThisFrame = false;
		[HideInInspector]
        public bool isDashButtonDown = false;
		[HideInInspector]
        public bool isRunButtonDown = false;
		[HideInInspector]
		public bool isAttackButtonDown = false;
		[HideInInspector]
        public bool isSubAttackButtonDown = false;
		[HideInInspector]
		public bool isSubAttack_2ButtonDown = false;
		[HideInInspector]
		public bool isSubAttack_3ButtonDown = false;
		[HideInInspector]
		public bool isAttackButtonDownThisFrame = false;
		[HideInInspector]
        public bool isSubAttackButtonDownThisFrame = false;
		[HideInInspector]
		public bool isSubAttack_2ButtonDownThisFrame = false;
		[HideInInspector]
		public bool isSubAttack_3ButtonDownThisFrame = false;
		[HideInInspector]
        public bool isAdvanceTextButtonDown = false;
		[HideInInspector]
        public bool isPauseButtonDown = false;
		[HideInInspector]
		public bool isMisc_1ButtonDown = false;
		[HideInInspector]
		public bool isMisc_2ButtonDown = false;
		[HideInInspector]
		public bool isMisc_1ButtonDownThisFrame = false;
		[HideInInspector]
		public bool isMisc_2ButtonDownThisFrame = false;

		[HideInInspector]
        public float horizontalAxis;
		[HideInInspector]
        public float verticalAxis;

		[HideInInspector]
		public float previousHorizontalAxis;

		[HideInInspector]
		public float previousVerticalAxis;

        protected bool isKeyboardEnabled = true;

        public bool GetIsKeyboardEnabled()
        {
            return isKeyboardEnabled;
        }

        void Update()
        {
            SetInputs();
        }

		public bool IsUpButtonDownThisFrame()
		{
			return (verticalAxis == 1.0f && previousVerticalAxis != 1.0f);
		}

		public bool IsDownButtonDownThisFrame()
		{
			return (verticalAxis == -1.0f && previousVerticalAxis != -1.0f);
		}

		public bool IsRightButtonDownThisFrame()
		{
			return (horizontalAxis == 1.0f && previousHorizontalAxis != 1.0f);
		}

		public bool IsLeftButtonDownThisFrame()
		{
			return (horizontalAxis == -1.0f && previousHorizontalAxis != -1.0f);
		}

        protected virtual void SetInputs()
        {
            if(isEnabled && Time.timeScale > 0 || willAcceptInputWhenPaused)
            {
                isAttackButtonDown = InputManager.Instance.GetButton(playerId, InputAction.Attack);
                isSubAttackButtonDown = InputManager.Instance.GetButton(playerId, InputAction.SubAttack);
				isSubAttack_2ButtonDown = InputManager.Instance.GetButton(playerId, InputAction.SubAttack_2);
				isSubAttack_3ButtonDown = InputManager.Instance.GetButton(playerId, InputAction.SubAttack_3);
                isAttackButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.Attack);
                isSubAttackButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.SubAttack);
				isSubAttack_2ButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.SubAttack_2);
				isSubAttack_3ButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.SubAttack_3);
                isJumpButtonDown = InputManager.Instance.GetButton(playerId, InputAction.Jump);
                isJumpButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.Jump);
                isDashButtonDown = InputManager.Instance.GetButtonDown(playerId, InputAction.Dash);
                isRunButtonDown = InputManager.Instance.GetButton(playerId, InputAction.Run);

				isMisc_1ButtonDown = InputManager.Instance.GetButton(playerId, InputAction.Misc_1);
				isMisc_2ButtonDown = InputManager.Instance.GetButton(playerId, InputAction.Misc_2);
				isMisc_1ButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.Misc_1);
				isMisc_2ButtonDownThisFrame = InputManager.Instance.GetButtonDown(playerId, InputAction.Misc_2);

				previousHorizontalAxis = horizontalAxis;
				previousVerticalAxis = verticalAxis;

                if(InputManager.Instance.GetAxis(playerId, InputAction.MoveHorizontal) > 0.5f)
                {
                    horizontalAxis = 1.0f;
                }
                else if(InputManager.Instance.GetAxis(playerId, InputAction.MoveHorizontal) < -0.5f)
                {
                    horizontalAxis = -1.0f;
                }
                else
                {
                    horizontalAxis = 0.0f;
                }

                if(InputManager.Instance.GetAxis(playerId, InputAction.MoveVertical) > 0.5f)
                {
                    verticalAxis = 1.0f;
                }
                else if(InputManager.Instance.GetAxis(playerId, InputAction.MoveVertical) < -0.5f)
                {
                    verticalAxis = -1.0f;
                }
                else
                {
                    verticalAxis = 0.0f;
                }
            }
            else
            {
                horizontalAxis = 0.0f;
                verticalAxis = 0.0f;
				previousHorizontalAxis = 0.0f;
				previousVerticalAxis = 0.0f;
                isAttackButtonDown = false;
                isSubAttackButtonDown = false;
				isSubAttack_2ButtonDown = false;
				isSubAttack_3ButtonDown = false;
                isAttackButtonDownThisFrame = false;
                isSubAttackButtonDownThisFrame = false;
				isSubAttack_2ButtonDownThisFrame = false;
				isSubAttack_3ButtonDownThisFrame = false;
                isJumpButtonDown = false;
                isJumpButtonDownThisFrame = false;
                isPauseButtonDown = false;
                isDashButtonDown = false;
                isRunButtonDown = false;
            }

            if(isEnabled) //Pause can't take Time.timeScale into account, since it will always be 0 if the game is paused already
            {
                isPauseButtonDown = InputManager.Instance.GetButtonDown(playerId, InputAction.Pause);
            }
        }
    }
}
