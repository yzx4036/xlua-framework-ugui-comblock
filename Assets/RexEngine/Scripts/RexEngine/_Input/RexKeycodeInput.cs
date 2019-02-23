/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

namespace RexEngine
{
	public class RexKeycodeInput:RexInput
	{
		public Keycodes keycodes;

		[System.Serializable]
		public class Keycodes
		{
			public KeyCode dPad_Up = KeyCode.W;
			public KeyCode dPad_Down = KeyCode.S;
			public KeyCode dPad_Left = KeyCode.A;
			public KeyCode dPad_Right = KeyCode.D;
			public KeyCode attack = KeyCode.G;
			public KeyCode subAttack_1 = KeyCode.T;
			public KeyCode subAttack_2 = KeyCode.Y;
			public KeyCode subAttack_3 = KeyCode.U;
			public KeyCode jump = KeyCode.H;
			public KeyCode dash = KeyCode.E;
			public KeyCode run = KeyCode.R;
			public KeyCode misc_1 = KeyCode.I;
			public KeyCode misc_2 = KeyCode.O;
			public KeyCode pause = KeyCode.KeypadEnter;
		}

		protected override void SetInputs()
		{
			if(isEnabled && Time.timeScale > 0 || willAcceptInputWhenPaused)
			{
				isAttackButtonDown = Input.GetKey(keycodes.attack); 
				isAttackButtonDownThisFrame = Input.GetKeyDown(keycodes.attack);

				isSubAttackButtonDown = Input.GetKey(keycodes.subAttack_1); 
				isSubAttackButtonDownThisFrame = Input.GetKeyDown(keycodes.subAttack_1); 

				isSubAttack_2ButtonDown = Input.GetKey(keycodes.subAttack_2); 
				isSubAttack_2ButtonDownThisFrame = Input.GetKeyDown(keycodes.subAttack_2); 

				isSubAttack_3ButtonDown = Input.GetKey(keycodes.subAttack_3);
				isSubAttack_3ButtonDownThisFrame = Input.GetKeyDown(keycodes.subAttack_3);

				isJumpButtonDown = Input.GetKey(keycodes.jump);
				isJumpButtonDownThisFrame = Input.GetKeyDown(keycodes.jump);

				isDashButtonDown = Input.GetKey(keycodes.dash);
				isRunButtonDown = Input.GetKey(keycodes.dash);

				isMisc_1ButtonDown = Input.GetKey(keycodes.misc_1);
				isMisc_1ButtonDownThisFrame = Input.GetKeyDown(keycodes.misc_1);

				isMisc_2ButtonDownThisFrame = Input.GetKey(keycodes.misc_2);
				isMisc_2ButtonDown = Input.GetKeyDown(keycodes.misc_2);

				previousHorizontalAxis = horizontalAxis;
				previousVerticalAxis = verticalAxis;

				if(Input.GetKey(keycodes.dPad_Right))
				{
					horizontalAxis = 1.0f;
				}
				else if(Input.GetKey(keycodes.dPad_Left))
				{
					horizontalAxis = -1.0f;
				}
				else
				{
					horizontalAxis = 0.0f;
				}

				if(Input.GetKey(keycodes.dPad_Up))
				{
					verticalAxis = 1.0f;
				}
				else if(Input.GetKey(keycodes.dPad_Down))
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
				isPauseButtonDown = Input.GetKey(keycodes.pause);
			}
		}
	}
}
