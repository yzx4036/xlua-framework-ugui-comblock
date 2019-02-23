/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class JumpState:BaseJumpState
	{
		[Tooltip("Whether the jump is: Finite, and thus limited to a specific number of jumps before touching the ground; infinite, in which case an unlimited number of jumps can be performed without needing to touch the ground; or None, in which case the jump is disabled entirely.")]
		public JumpType type;
		[Tooltip("If JumpType is set to Finite, this governs the number of jumps that can be made before touching the ground.")]
		public int multipleJumpNumber = 1;
		[Tooltip("If multiple jumps are allowed without touching the ground, this determines if the actor is allowed to perform secondary jumps even if they became airborne by falling (and not by jumping initially.)")]
		public bool canMultiJumpOutOfFall = true;
		[Tooltip("The number of frames in which Jump is still enabled after walking off a ledge.")]
		public int graceFrames = 1;
		[Tooltip("Setting this to True prevents the actor from maneuvering in midair after a jump is started.")]
		public bool freezeHorizontalMovement;
		[Tooltip("Settings which enable the actor to jump higher while moving at faster horizontal speeds.")]
		public JumpHigherAtFasterSpeeds jumpHigherAtFasterSpeeds;

		//public const string idString = "Jumping";

		protected DashState dashState;
		protected WallClingState wallClingState;
		protected float wallJumpSpeed;
		protected int currentGraceFrame = 0;

		[System.Serializable]
		public class JumpHigherAtFasterSpeeds
		{
			[Tooltip("Whether or not jumping higher at faster horizontal movement speeds is enabled.")]
			public bool isEnabled = false;
			[Tooltip("Whether or not jumping higher at faster horizontal movement speeds is enabled for jumps beyond the primary one.")]
			public bool isEnabledForMultiJump = false;
			[Tooltip("Settings for what horizontal speed the actor must achieve for their jump to be a certain vertical speed.")]
			public List<JumpHeightForSpeed> heightForSpeed = new List<JumpHeightForSpeed>();
		}

		[System.Serializable]
		public class JumpHeightForSpeed
		{
			[Tooltip("The horizontal speed the actor must be moving...")]
			public float moveSpeedThreshhold;
			[Tooltip("... for their Jump Speed to be set to this value.")]
			public float jumpSpeed;
		}

		void Awake()
		{
			id = idString;
			isConcurrent = true;
			willPlayAnimationOnBegin = false;
			dashState = GetComponent<DashState>();
			wallClingState = GetComponent<WallClingState>();
			blockAutoChange.toFalling = false;
			blockAutoChange.toDefaultOnLanding = false;
			GetController();
		}

		void Update() 
		{
			if(!IsFrozen() && controller.isEnabled)
			{
				CheckJumpBuffer();
				bool isJumpAttempted = controller.slots.input && controller.slots.input.isJumpButtonDownThisFrame || isJumpBuffered;
				willAllowDirectionChange = (isJumpActive && freezeHorizontalMovement) ? false : true;
				if(isJumpAttempted)
				{
					Begin(true);
				}

				if(isJumpActive && controller.slots.input && !controller.slots.input.isJumpButtonDown)
				{
					hasReleasedButtonSinceJump = true;
				}
			}
		}

		void FixedUpdate()
		{
			if(Time.timeScale > 0)
			{
				ResetFlags();
				framesFrozenForWallJump --;
				if(framesFrozenForWallJump < 0)
				{
					framesFrozenForWallJump = 0;
					isWallJumpKickbackActive = false;
				}
			}
		}

		#region unique public methods

		public void NotifyOfWallJump(int framesToFreeze, Direction.Horizontal kickbackDirection, float _speed)
		{
			framesFrozenForWallJump = framesToFreeze;
			direction = kickbackDirection;
			wallJumpSpeed = _speed;

			isWallJumpKickbackActive = true;
			currentJump = 0;

			ForceBegin();
		}

		public bool IsHorizontalMovementFrozen()
		{
			return freezeHorizontalMovement || isWallJumpKickbackActive;
		}

		#endregion

		#region override public methods

		public override void OnNewStateAdded(RexState _state)
		{
			if(_state.id == WallClingState.idString && wallClingState == null)
			{
				wallClingState = _state as WallClingState;
			}	
		}

		public override bool CanInitiate()
		{
			bool canInitiateJump = false;
			if(type == JumpType.None)
			{
				canInitiateJump = false;
			}
			else if(controller.StateID() == CrouchState.idString && (!controller.GetComponent<CrouchState>().CanExitCrouch() || !controller.GetComponent<CrouchState>().canJump))
			{
				canInitiateJump = false;
			}
			else if(controller.StateID() == StairClimbingState.idString && !controller.GetComponent<StairClimbingState>().canJump)
			{
				canInitiateJump = false;
			}
			else if(controller.slots.input && controller.slots.input.verticalAxis == -1.0f && RaycastHelper.DropThroughFloorRaycast((Direction.Vertical)(controller.GravityScaleMultiplier() * -1.0f), controller.slots.actor.GetComponent<BoxCollider2D>())) //Dropping through a one-way ledge instead of jumping
			{
				canInitiateJump = false;
			}
			else if(controller.StateID() == GroundPoundState.idString && (!controller.GetComponent<GroundPoundState>().canCancelWithJump))
			{
				canInitiateJump = false;
			}
			else if(IsLockedForAttack(Attack.ActionType.Jumping))
			{
				canInitiateJump = false;
			}
			else if(controller.isKnockbackActive || controller.isStunned)
			{
				canInitiateJump = false;
			}
			else if(controller.isDashing && dashState && !dashState.canJump)
			{
				canInitiateJump = false;
			}
			else if(controller.slots.actor && controller.StateID() == LadderState.idString)
			{
				LadderState ladderState = controller.GetComponent<LadderState>();
				if(ladderState != null && ladderState.onJump == LadderState.OnJump.Jump)
				{
					canInitiateJump = true;
				}
				else
				{
					canInitiateJump = false;
				}
			}
			else if(controller.framesSinceDrop < 2)
			{
				canInitiateJump = false;
			}
			else if(wallClingState && wallClingState.IsWallJumpPossible())
			{
				canInitiateJump = false;
			}
			else
			{
				if(type == JumpType.Finite)
				{
					if(multipleJumpNumber > 0)
					{
						if((currentJump == 0 && (isGroundedWithJumpButtonUp || currentGraceFrame < graceFrames || (!controller.slots.input && controller.slots.physicsObject.IsOnSurface()))) || (currentJump < multipleJumpNumber && currentJump > 0))
						{
							currentJump ++;
							canInitiateJump = true;
						}
						else if(multipleJumpNumber > 1 && !isJumpActive && canMultiJumpOutOfFall && currentJump < multipleJumpNumber)
						{
							currentJump += 2;
							canInitiateJump = true;
						}
					}
				}
				else if(type == JumpType.Infinite)
				{
					currentJump ++;
					canInitiateJump = true;
				}
			}

			return canInitiateJump;
		}

		public override void OnBegin()
		{
			currentJumpFrame = 0;
			currentHangtimeFrame = 0;
			isJumpActive = true;
			hasReleasedButtonSinceJump = false;
			controller.slots.physicsObject.properties.isFalling = false;
			isGroundedWithJumpButtonUp = false;
			adjustedSpeed = GetAdjustedSpeed();

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onJump)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			controller.slots.physicsObject.ApplyForce(new Vector2(0.0f, adjustedSpeed * controller.GravityScaleMultiplier()));

			if(freezeHorizontalMovement)
			{
				if(controller.slots.input)
				{
					direction = (Direction.Horizontal)controller.slots.input.horizontalAxis;
				}
				else
				{
					direction = (Direction.Horizontal)controller.axis.x;
				}
			}

			StartCoroutine("JumpCoroutine");

			controller.slots.actor.NotifyOfControllerJumping(currentJump);
		}

		#endregion

		protected float GetAdjustedSpeed()
		{
			float adjustedSpeed = speed;
			if(jumpHigherAtFasterSpeeds.isEnabled && (currentJump <= 1 || jumpHigherAtFasterSpeeds.isEnabledForMultiJump))
			{
				for(int i = jumpHigherAtFasterSpeeds.heightForSpeed.Count - 1; i >= 0; i --)
				{
					if(Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) >= jumpHigherAtFasterSpeeds.heightForSpeed[i].moveSpeedThreshhold)
					{
						adjustedSpeed = jumpHigherAtFasterSpeeds.heightForSpeed[i].jumpSpeed;
						break;
					}
				}
			}

			if(isWallJumpKickbackActive)
			{
				adjustedSpeed = wallJumpSpeed;
			}

			return adjustedSpeed;
		}

		protected override void ResetFlags()
		{
			currentGraceFrame ++;

			bool isOnSurface = controller.slots.physicsObject.IsOnSurface();
			if((isOnSurface || currentGraceFrame < graceFrames) && controller.slots.input && !controller.slots.input.isJumpButtonDown && !isJumpActive)
			{
				isGroundedWithJumpButtonUp = true;
			}
			else if(!isOnSurface || isJumpActive)
			{
				isGroundedWithJumpButtonUp = false;
			}

			if(isOnSurface && !isJumpActive)
			{
				currentJump = 0;
				currentGraceFrame = 0;
			}
			else
			{
				currentGraceFrame ++;
			}
		}
	}

}
