/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DashState:RexState 
	{
		public const string idString = "Dashing";

		[Tooltip("The horizontal speed of the dash.")]
		public float speed;
		[Tooltip("The minimum number of frames the dash will go, assuming the dash button is let go of as soon as possible after the dash begins.")]
		public int minFrames;
		[Tooltip("The maximum number of frames the dash will go, assuming the dash button is held for as long as possible after the dash begins.")]
		public int maxFrames;
		[Tooltip("Whether or not a jump can be executed while the dash is active.")]
		public bool canJump;
		[Tooltip("If Can Jump is enabled, this determines if beginning a jump will cancel any active dashes.")]
		public bool isCanceledByJump;
		[Tooltip("If True, any jumps that begin while the dash is active will retain the horizontal momentum of the dash.")]
		public bool isMomentumRetainedOnJump;
		[Tooltip("Whether or not the actor can change its horizontal direction in mid-dash.")]
		public bool requireDirectionalHoldToRetainMomentumOnJump;
		[Tooltip("If Is Momentum Retained On Jump is enabled, this determines if the player must continue holding the d-pad in the direction of movement to keep their momentum.")]
		public bool canChangeDirection;
		[Tooltip("If Can Change Direction is enabled, this determines if changing direction in mid-dash cancels the dash.")]
		public bool isCanceledByDirectionChange;
		[Tooltip("If True, dashes can be initialized while the actor is airborne.")]
		public bool canStartDashInAir;
		[Tooltip("If Can Start Dash In Air is enabled, this number determines how many air dashes are allowed before touching the ground. Setting this number to 0 allows infinite air dashes.")]
		public int maxAirDashes;
		[Tooltip("If True, air dashes are restored upon climbing a wall or ladder.")]
		public bool resetAirDashesOnClimb = true;
		[Tooltip("If True, dashes can be initialized while the actor is on a ladder.")]
		public bool canDashFromLadders;
		[Tooltip("If True, you can initiate a dash from the Crouching state.")]
		public bool canDashFromCrouch = false;
		[Tooltip("If True, you can initiate a dash while clinging to a wall.")]
		public bool canDashFromWallCling;
		[Tooltip("If the actor was airborne when the current dash started, this determines if the dash is canceled when the actor lands.")]
		public bool willStopDashUponLanding;
		[Tooltip("If True, the actor's vertical movement will be frozen if they begin a dash in midair.")]
		public bool freezeVerticalMovementOnAirDash = true;
		[Tooltip("If True, the actor's vertical movement will be frozen if they dash off a ledge.")]
		public bool freezeVerticalMovementWhenDashingOffLedge = true;
		[Tooltip("If True, dashes will be canceled upon contact with a wall.")]
		public bool isCanceledByWallContact;
		[Tooltip("If True, the actor's horizontal acceleration will be stopped when the dash ends.")]
		public bool killAccelerationOnEnd = false;
		[Tooltip("The duration from when you perform the dash to when you're allowed to perform it again.")]
		public float cooldownTime = 0.2f;

		public DashDirectionsAllowed dashDirectionsAllowed;

		[System.Serializable]
		public class DashDirectionsAllowed
		{
			public bool horizontal = true;
			public bool up;
			public bool down;
			public bool diagonalUp;
			public bool diagonalDown;
		}

		protected int currentAirDash;
		protected int currentFrame;
		protected bool didCurrentDashStartInAir;
		protected bool hasReleasedButtonSinceDash;
		protected float currentCooldownTime;
		protected bool isCooldownComplete = true;
		protected Vector2 dashStartDirection = Vector2.zero;
		protected const float diagonalMultiplier = 0.707f; //Moving in two directions at once is 1.4x faster than just one; therefore, we multiply speed in each direction by 0.707
		protected float currentDiagonalMultiplier = 1.0f;
		protected bool wasJumpingWhenDashBegan;
		protected bool didJumpOutOfDash;

		protected Vector2 dashStartPosition;

		void Awake() 
		{
			id = idString;
			isConcurrent = true;
			blockAutoChange.toDefaultOnLanding = false;
			GetController();
		}

		void Update()
		{
			if(isEnabled && !IsFrozen() && !controller.isKnockbackActive)
			{
				willAllowDirectionChange = (controller.isDashing && !canChangeDirection) ? false : true;
				bool isDashAttempted = controller.slots.input && controller.slots.input.isDashButtonDown;
				if(isDashAttempted && !(controller.StateID() == CrouchState.idString && !canDashFromCrouch))
				{
					SetDashStartDirection();
					if(dashStartDirection == Vector2.zero)
					{
						return;
					}

					wasJumpingWhenDashBegan = false;
					if(!controller.slots.physicsObject.IsOnSurface() && controller.StateID() == JumpState.idString)
					{
						wasJumpingWhenDashBegan = true;
						controller.GetComponent<JumpState>().End();
					}

					Begin();
				}
			}
		}

		void FixedUpdate()
		{
			CheckCooldown();
		}

		public int GetCurrentFrame()
		{
			return currentFrame;
		}

		#region override public methods

		public override bool CanInitiate()
		{
			bool isBlockedByStairs = controller.StateID() == StairClimbingState.idString;
			bool isBlockedByGroundPound = controller.StateID() == GroundPoundState.idString && !controller.GetComponent<GroundPoundState>().canCancelWithDash;
			bool isBlockedByGlide = controller.StateID() == GlideState.idString;
			bool isBlockedByJump = controller.StateID() == JumpState.idString && isCanceledByJump;
			bool isBlockedByWallCling = controller.StateID() == WallClingState.idString && !canDashFromWallCling;
			return (!controller.isDashing && !controller.isStunned && !isBlockedByStairs && !isBlockedByGroundPound && !isBlockedByGlide && !isBlockedByJump && !isBlockedByWallCling && hasReleasedButtonSinceDash && !IsLockedForAttack(Attack.ActionType.Dashing) && !(!canDashFromLadders && controller.StateID() == "Climbing") && isCooldownComplete && (controller.slots.physicsObject.IsOnSurface() || (controller.StateID() == WallClingState.idString && canDashFromWallCling) || (canStartDashInAir && (currentAirDash < maxAirDashes || maxAirDashes == 0))));
		}

		public override void OnBegin()
		{
			controller.isDashing = true;
			hasReleasedButtonSinceDash = false;
			currentFrame = 0;
			didJumpOutOfDash = false;

			dashStartPosition = new Vector2(transform.position.x, transform.position.y);

			if(dashStartPosition.y != 0.0f)
			{
				controller.slots.physicsObject.SetVelocityY(0.0f);
			}

			controller.FaceDirection((Direction.Horizontal)((int)dashStartDirection.x));


			controller.slots.physicsObject.MoveX(speed * dashStartDirection.x * currentDiagonalMultiplier);
			controller.slots.physicsObject.MoveY(speed * dashStartDirection.y * currentDiagonalMultiplier);

			if(dashStartDirection.y != 0.0f)
			{
				controller.slots.physicsObject.FreezeGravityForSingleFrame();
				if(wasJumpingWhenDashBegan)
				{
					controller.GetComponent<JumpState>().End();
				}
			}

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onDash)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			if(!controller.slots.physicsObject.IsOnSurface())
			{
				currentAirDash ++;
				didCurrentDashStartInAir = true;

				if(freezeVerticalMovementOnAirDash && dashStartDirection.y == 0.0f)
				{	
					controller.slots.physicsObject.FreezeYMovementForSingleFrame();
				}
			}
			else
			{
				didCurrentDashStartInAir = false;
			}
		}

		public override void OnEnded()
		{
			if(controller.slots.physicsObject.properties.externalDeceleration.x == 0.0f && killAccelerationOnEnd && dashStartDirection.x != 0.0f)
			{
				controller.slots.physicsObject.SetVelocityX(0.0f);
			}

			if(controller.slots.physicsObject.properties.externalDeceleration.y == 0.0f && killAccelerationOnEnd && dashStartDirection.y == 1.0f * controller.GravityScaleMultiplier())
			{
				controller.slots.physicsObject.SetVelocityY(0.0f);
			}
			else if(dashStartDirection.y == 0.0f)
			{
				controller.slots.physicsObject.SetVelocityY(0.0f);
			}

			currentCooldownTime = 0;
			isCooldownComplete = false;
			controller.isDashing = false;
			currentFrame = 0;
		}

		public override void OnStateChanged()
		{
			if(controller.StateID() == LadderState.idString)
			{
				End();
			}

			if(controller.StateID() == JumpState.idString)
			{
				if(isCanceledByJump)
				{
					controller.isDashing = false;
				}

				didJumpOutOfDash = true;
			}
		}

		public override void UpdateMovement()
		{
			ContinueDash(controller.axis.x);
		}

		#endregion

		protected void ContinueDash(float _inputDirection)
		{
			if((controller.slots.input && controller.slots.input.isDashButtonDown) || controller.isDashing)
			{
				if(controller.isDashing) //Continue dash
				{
					currentFrame ++;
					bool willCancelDash = false;

					Direction.Horizontal actorDirection = (_inputDirection > 0.0f) ? Direction.Horizontal.Right : Direction.Horizontal.Left;
					bool didChangeDirection = (actorDirection != controller.direction.horizontal && _inputDirection != 0.0f) ? true : false;
					if(currentFrame >= minFrames && ((controller.slots.input && !controller.slots.input.isDashButtonDown) || !controller.slots.input))
					{
						willCancelDash = true;
					}

					if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.dash)
					{
						willCancelDash = true;
					}

					if(currentFrame >= maxFrames)
					{
						willCancelDash = true;
					}

					if(canChangeDirection && _inputDirection != 0.0f)
					{
						if(didChangeDirection)
						{
							controller.FaceDirection(actorDirection);
							if(isCanceledByDirectionChange)
							{
								willCancelDash = true;
							}
						}
					}

					if(isMomentumRetainedOnJump && !controller.slots.physicsObject.IsOnSurface() && didJumpOutOfDash)
					{
						if(requireDirectionalHoldToRetainMomentumOnJump && _inputDirection == 0.0f)
						{
							willCancelDash = true;
						}
						else if(didChangeDirection && isCanceledByDirectionChange)
						{
							willCancelDash = true;
						}
						else if(!didCurrentDashStartInAir && dashStartDirection.y == 0.0f)
						{
							willCancelDash = false;
						}
					}

					if(willStopDashUponLanding && controller.slots.physicsObject.DidLandThisFrame())
					{
						willCancelDash = true;
					}

					if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.dash)
					{
						willCancelDash = true;
					}

					if(isCanceledByWallContact && controller.slots.physicsObject.IsAgainstEitherWall())
					{
						willCancelDash = true;
					}

					if(willCancelDash) //Dash is canceled
					{
						if(!hasEnded && controller.StateID() == id)
						{
							controller.SetStateToDefault();
						}

						//Vector2 dashDistance = new Vector2(Mathf.Abs(dashStartPosition.x - transform.position.x), Mathf.Abs(dashStartPosition.y - transform.position.y));

						End();

						controller.isDashing = false;
					}
					else //Dash continues
					{
						if(dashStartDirection.x != 0.0f)
						{
							controller.slots.physicsObject.MoveX(speed * (int)controller.direction.horizontal * currentDiagonalMultiplier);
							controller.slots.physicsObject.properties.externalAcceleration.x = 0.0f;
						}

						if(dashStartDirection.y != 0.0f)
						{
							controller.slots.physicsObject.MoveY(speed * dashStartDirection.y * currentDiagonalMultiplier);
							controller.slots.physicsObject.properties.externalAcceleration.y = 0.0f;
							controller.slots.physicsObject.FreezeGravityForSingleFrame();
						}

						if(freezeVerticalMovementWhenDashingOffLedge && !controller.slots.physicsObject.IsOnSurface() && !didCurrentDashStartInAir && controller.isDashing && dashStartDirection.y == 0.0f && controller.StateID() != JumpState.idString && currentFrame < maxFrames)
						{
							controller.slots.physicsObject.FreezeYMovementForSingleFrame();
						}
						else if(freezeVerticalMovementOnAirDash && dashStartDirection.y == 0.0f)
						{
							if(controller.isDashing && didCurrentDashStartInAir && !controller.slots.physicsObject.IsOnSurface() && controller.StateID() == idString)
							{
								controller.slots.physicsObject.FreezeYMovementForSingleFrame();
							}
						}
					}
				}
			}
			else
			{
				hasReleasedButtonSinceDash = true;
			}

			bool willResetAirDashesFromClimbing = resetAirDashesOnClimb && (controller.StateID() == LadderState.idString || controller.StateID() == WallClingState.idString);
			if(currentAirDash != 0 && controller.slots.physicsObject.IsOnSurface() || willResetAirDashesFromClimbing)
			{
				currentAirDash = 0;
			}
		}

		protected void SetDashStartDirection()
		{
			if(controller.slots.input)
			{
				bool isDashingDiagonally = false;
				dashStartDirection = Vector2.zero;

				if(controller.slots.input.verticalAxis == 1.0f && Mathf.Abs(controller.slots.input.horizontalAxis) == 1.0f && dashDirectionsAllowed.diagonalUp)
				{
					isDashingDiagonally = true;
					dashStartDirection.x = (int)controller.direction.horizontal;
					dashStartDirection.y = 1.0f * controller.GravityScaleMultiplier();
				}
				else if(controller.slots.input.verticalAxis == -1.0f && Mathf.Abs(controller.slots.input.horizontalAxis) == 1.0f && dashDirectionsAllowed.diagonalDown)
				{
					isDashingDiagonally = true;
					dashStartDirection.x = (int)controller.direction.horizontal;
					dashStartDirection.y = -1.0f * controller.GravityScaleMultiplier();
				}
				else if(controller.slots.input.verticalAxis == 1.0f && dashDirectionsAllowed.up)
				{
					dashStartDirection.y = 1.0f * controller.GravityScaleMultiplier();
				}
				else if(controller.slots.input.verticalAxis == -1.0f && dashDirectionsAllowed.down)
				{
					dashStartDirection.y = -1.0f * controller.GravityScaleMultiplier();
				}
				else if(dashDirectionsAllowed.horizontal)
				{
					dashStartDirection.x = (int)controller.direction.horizontal;
				}

				if(controller.StateID() == WallClingState.idString)
				{
					dashStartDirection.x = (int)controller.direction.horizontal * -1;
				}

				currentDiagonalMultiplier = (isDashingDiagonally) ? diagonalMultiplier : 1.0f;
			}
		}

		protected void CheckCooldown()
		{
			if(!isCooldownComplete)
			{
				currentCooldownTime += Time.fixedDeltaTime;
				if(currentCooldownTime >= cooldownTime)
				{
					isCooldownComplete = true;
					currentCooldownTime = 0;
				}
			}
		}
	}

}
