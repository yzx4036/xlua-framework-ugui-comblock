using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class FlutterJumpState:BaseJumpState 
	{
		public new const string idString = "FlutterJump";

		public float buttonHoldDurationForBegin = 0.2f;
		public int requiredFallFrames = 12;
		public int cooldownFrames = 24;
		public bool allowMultipleJumps = true;

		protected float currentButtonHoldDuration;
		protected int currentCooldownFrame;

		void Awake() 
		{
			id = idString;
			isConcurrent = true;
			blockAutoChange.toFalling = false;
			blockAutoChange.toDefaultOnLanding = false;
			GetController();
			adjustedSpeed = speed;
		}

		public override bool CanInitiate()
		{
			return HasFallenEnoughToInitiate();
		}

		void Update() 
		{
			if(!IsFrozen() && controller.isEnabled)
			{
				if(GetButtonHoldDuration() && controller.StateID() == FallingState.idString && !controller.slots.physicsObject.IsOnSurface())
				{
					Begin(true);
				}
			}
		}

		void FixedUpdate()
		{
			bool isOnSurface = controller.slots.physicsObject.IsOnSurface();
			if(isOnSurface)
			{
				currentJump = 0;
				if(isJumpActive)
				{
					End();
					controller.SetStateToDefault();
				}
			}
		}

		#region override public methods

		public override void OnBegin()
		{
			currentJump ++;
			currentJumpFrame = 0;
			currentHangtimeFrame = 0;
			isJumpActive = true;
			hasReleasedButtonSinceJump = false;
			controller.slots.physicsObject.properties.isFalling = false;
			controller.slots.physicsObject.ApplyForce(new Vector2(0.0f, speed * controller.GravityScaleMultiplier()));

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onFlutterJump)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			blockAutoChange.toFalling = true;

			StartCoroutine("JumpCoroutine");
		}

		public override void OnEnded()
		{
			blockAutoChange.toFalling = false;
			isJumpActive = false;
		}

		public override void OnStateChanged()
		{
			currentCooldownFrame = 0;
			EndOtherStates();
		}

		#endregion

		protected bool HasFallenEnoughToInitiate()
		{
			int fallFrames = controller.GetCurrentFallFrame();
			return (currentJump == 0 && fallFrames >= requiredFallFrames) || (allowMultipleJumps && currentJump > 0 && currentCooldownFrame >= cooldownFrames);
		}

		protected bool GetButtonHoldDuration()
		{
			bool hasHeldForDuration = false;
			if(controller.slots.input && controller.slots.input.isJumpButtonDown)
			{
				currentButtonHoldDuration += Time.deltaTime;
				if(currentButtonHoldDuration >= buttonHoldDurationForBegin)
				{
					hasHeldForDuration = true;
				}
			}
			else
			{
				currentButtonHoldDuration = 0.0f;
			}

			return hasHeldForDuration;
		}

		protected override void HandleJumpExtras()
		{
			if(!isJumpActive && currentHangtimeFrame >= hangtimeFrames) 
			{
				currentCooldownFrame ++;
			}
		}
	}
}
