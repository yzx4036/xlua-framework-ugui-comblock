using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class GlideState:RexState 
	{
		public const string idString = "Glide";

		public float descentSpeed = 0.5f;
		public bool suppressGravity = true;
		public int minimumGlideFrames = 5;
		public int cooldownFrames = 5;
		public EnableType enableType;
		public Button button;
		public AudioClip endClip;
		public Animations animations;

		protected int currentGlideFrame = 0;
		protected int currentCooldownFrame = 0;

		protected Substate substate;

		[System.Serializable]
		public class Animations
		{
			public AnimationClip beginAnimation;
			public AnimationClip endAnimation;
		}

		public enum Substate
		{
			Beginning,
			Body,
			Ending
		}

		public enum EnableType
		{
			Toggle,
			Hold
		}

		public enum Button
		{
			Up,
			Down
		}

		void Awake() 
		{
			id = idString;
			GetController();
			currentCooldownFrame = cooldownFrames;
			willPlayAnimationOnBegin = false;
		}

		void Update() 
		{
			if(isEnabled && !IsFrozen() && !controller.isKnockbackActive)
			{
				bool isGlideAttempted = controller.slots.input && GetIsButtonDownThisFrame() && !controller.slots.physicsObject.IsOnSurface();
				if(isGlideAttempted && !IsLockedForAttack(Attack.ActionType.Gliding) && controller.StateID() != id && currentCooldownFrame >= cooldownFrames)
				{
					currentGlideFrame = 0;
					Begin();
				}
			}
		}

		void FixedUpdate()
		{
			if(controller.StateID() != id)
			{
				currentCooldownFrame ++;
			}

			if(controller.slots.physicsObject.IsOnSurface())
			{
				currentCooldownFrame = cooldownFrames;
			}
		}

		#region override public methods

		public override void UpdateMovement()
		{
			if(controller.StateID() == id)
			{
				bool isGlideContinued = false; 
				bool isButtonDownThisFrame = GetIsButtonDownThisFrame();
				bool isButtonHeld = GetIsButtonHeld();
				if(enableType == EnableType.Toggle)
				{
					isGlideContinued = !isButtonDownThisFrame;
				}
				else if(enableType == EnableType.Hold)
				{
					isGlideContinued = isButtonHeld;
				}

				if(currentGlideFrame < minimumGlideFrames)
				{
					isGlideContinued = true;
				}

				if(isGlideContinued)
				{
					currentGlideFrame ++;

					controller.slots.physicsObject.SetVelocityY(descentSpeed * -controller.GravityScaleMultiplier());
					if(suppressGravity)
					{
						controller.slots.physicsObject.FreezeGravityForSingleFrame();
					}
				}
				else
				{
					StartCoroutine("AnimateEndCoroutine");
				}

				if((controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.glide) || (controller.slots.physicsObject.IsOnSurface()))
				{
					if(!hasEnded)
					{
						controller.SetStateToDefault();
					}

					End();
				}
			}
		}

		public override void OnBegin()
		{
			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onGlide)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			if(controller.isDashing)
			{
				DashState dashState = controller.GetComponent<DashState>();
				if(dashState)
				{
					dashState.End();
				}
			}

			substate = Substate.Beginning;

			StartCoroutine("BeginCoroutine");
		}

		public override void OnStateChanged()
		{
			currentCooldownFrame = 0;
			currentGlideFrame = 0;
		}

		public override void OnEnded()
		{
			currentCooldownFrame = 0;
			currentGlideFrame = 0;
			controller.SetStateToDefault();

			if(endClip != null)
			{
				controller.slots.actor.PlaySoundIfOnCamera(endClip);
			}
		}

		#endregion

		#region protected methods

		protected IEnumerator BeginCoroutine()
		{
			float delay = 0.0f;
			if(animations.beginAnimation != null)
			{
				PlaySecondaryAnimation(animations.beginAnimation);
				delay = animations.beginAnimation.length;
			}

			yield return new WaitForSeconds(delay);

			substate = Substate.Body;
			PlayAnimation();
		}

		protected IEnumerator AnimateEndCoroutine()
		{
			substate = Substate.Ending;

			float delay = 0.0f;
			if(animations.endAnimation != null)
			{
				PlaySecondaryAnimation(animations.endAnimation);
				delay = animations.endAnimation.length;
			}

			yield return new WaitForSeconds(delay);

			End();
		}

		protected bool GetIsButtonDownThisFrame()
		{
			bool isButtonDownThisFrame = false;
			switch(button)
			{
				case Button.Up:
					isButtonDownThisFrame = controller.slots.input.IsUpButtonDownThisFrame();
					break;
				case Button.Down:
					isButtonDownThisFrame = controller.slots.input.IsDownButtonDownThisFrame();
					break;
			}

			return isButtonDownThisFrame;
		}

		protected bool GetIsButtonHeld()
		{
			bool isButtonHeld = false;
			switch(button)
			{
				case Button.Up:
					isButtonHeld = controller.slots.input.verticalAxis == 1.0f;
					break;
				case Button.Down:
					isButtonHeld = controller.slots.input.verticalAxis == -1.0f;
					break;
			}

			return isButtonHeld;
		}

		#endregion
	}
}
