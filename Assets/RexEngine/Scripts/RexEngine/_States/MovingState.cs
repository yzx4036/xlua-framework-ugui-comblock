/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class MovingState:RexState
	{
		[System.Serializable]
		public class MovementSpeed
		{
			[Tooltip("The horizontal movement speed of the actor.")]
			public float speed = 20.0f;
			[Tooltip("The horizontal acceleration of the actor. Setting this to 0 means the actor will achieve top speed immediately upon moving. For any value above 0, lower values will accelerate to top speed more slowly than higher values.")]
			public float acceleration;
			[Tooltip("The horizontal deceleration of the actor after it stops moving. Setting this to 0 means the actor will stop immediately once movement ends. For any value above 0, lower values will decelerate to a stop more slowly than higher values.")]
			public float deceleration;
		}

		[System.Serializable]
		public class RunProperties
		{
			[Tooltip("If True, the actor can move at a higher-than-normal speed when the run button is held.")]
			public bool canRun = false;
			[Tooltip("If True, and Can Run is also True, running can be initiated while the actor is airborne.")]
			public bool canInitiateInAir = false;
			[Tooltip("If Can Run is True, this animation will play while the actor is running.")]
			public AnimationClip runAnimation;
		}

		[System.Serializable]
		public class FootstepSounds
		{
			public bool useFootstepSounds;
			public float durationBetweenFootsteps = 0.35f;
			public float footstepVolume = 0.125f;

			public List<FootstepSoundWithTag> sounds;

			[HideInInspector]
			public float timeSinceLastFootstep;
		}

		[System.Serializable]
		public class FootstepSoundWithTag
		{
			public string surfaceTag = "Terrain";
			public AudioClip sound;
		}

		public enum MovementAxis
		{
			Horizontal,
			Vertical
		}

		public enum MoveType
		{
			Walking,
			Running
		}

		public const string idString = "Moving";

		[Tooltip("Options for speed, acceleration, and deceleration of normal movement.")]
		public MovementSpeed movementProperties;
		[Tooltip("Options for speed, acceleration, and deceleration of running.")]
		public MovementSpeed runProperties;
		[Tooltip("Additional options for running.")]
		public RunProperties runSettings;
		[Tooltip("Whether or not the actor can move up and down with the up and down inputs.")]
		public bool canMoveVertically;
		[Tooltip("If true, moving diagonally slows both x and y movement to keep your overall distance covered consistent with the amount of distance covered when moving in only a cardinal direction.")]
		public bool willMaintainSpeedOnDiagonal = true;

		public FootstepSounds footstepSounds;

		public Direction.Horizontal autorun = Direction.Horizontal.Neutral;

		protected JumpState jumpState;
		protected MoveType currentMoveType;
		protected bool isSlowingDownFromRun = false;

		void Awake() 
		{
			id = idString;
			doesTurnAnimationHavePriority = true;
			isConcurrent = true;
			willAllowDirectionChange = true;
			blockAutoChange.toFalling = false;
			blockAutoChange.toDefaultOnLanding = false;
			jumpState = GetComponent<JumpState>();
			GetController();
		}

		#region override public methods

		public override void OnNewStateAdded(RexState _state)
		{
			if(_state.id == JumpState.idString && jumpState == null)
			{
				jumpState = _state as JumpState;
			}	
		}

		public override void OnStateChanged()
		{
			if(controller.StateID() == LadderState.idString)
			{
				End();
			}
		}

		public override void PlayAnimationForSubstate()
		{
			if(currentMoveType == MoveType.Running && runSettings.runAnimation != null)
			{
				PlaySecondaryAnimation(runSettings.runAnimation);
			}
			else
			{
				PlayAnimation();
			}
		}

		public override void UpdateMovement()
		{
			float xAxis = controller.axis.x;
			if((int)autorun != 0.0f)
			{
				xAxis = (int)autorun;
			}

			ApplyMovement(xAxis, MovementAxis.Horizontal);

			if(canMoveVertically)
			{
				ApplyMovement(controller.axis.y, MovementAxis.Vertical);
			}

			if(footstepSounds.useFootstepSounds && controller.StateID() == idString && controller.slots.physicsObject.IsOnSurface())
			{
				CheckFootstepSounds();
			}
		}

		public override void OnBegin()
		{
			isSlowingDownFromRun = false;
			currentMoveType = MoveType.Walking;
		}

		public override void OnEnded()
		{
			isSlowingDownFromRun = false;
			currentMoveType = MoveType.Walking;
			footstepSounds.timeSinceLastFootstep = 0.0f;
		}

		#endregion

		protected void ApplyHorizontalJump(JumpState jumpState)
		{
			controller.slots.physicsObject.properties.velocityCap.x = movementProperties.speed * (int)jumpState.direction;
			controller.slots.physicsObject.MoveX(movementProperties.speed * (int)jumpState.direction);
			controller.slots.physicsObject.properties.acceleration.x = 0.0f;
			controller.slots.physicsObject.properties.deceleration.x = 0.0f;
		}

		protected void ApplyMovement(float _inputDirection, MovementAxis movementAxis)
		{
			bool isLockedForAttack = (controller.slots.physicsObject.IsOnSurface()) ? IsLockedForAttack(Attack.ActionType.GroundMoving) : IsLockedForAttack(Attack.ActionType.AirMoving);
			bool isOverriddenByDash = controller.isDashing;
			bool isRunning = false;

			if(controller.StateID() == StairClimbingState.idString)
			{
				return;
			}

			//TODO: This needs to be used for other states -- dash, ladders, stair climbing, etc.
			if(controller.currentState.blockMovingStateMovement)
			{
				return;
			}

			if(runSettings.canRun && controller.slots.input && controller.slots.input.isRunButtonDown)
			{
				if((controller.slots.physicsObject.IsOnSurface() || runSettings.canInitiateInAir) || currentMoveType == MoveType.Running)
				{
					isRunning = true;
					isSlowingDownFromRun = false;

					if(runSettings.runAnimation && currentMoveType != MoveType.Running && Mathf.Abs(_inputDirection) != 0.0f && controller.slots.physicsObject.IsOnSurface() && controller.StateID() == idString)
					{
						PlaySecondaryAnimation(runSettings.runAnimation);
					}

					currentMoveType = MoveType.Running;
				}
				else
				{
					if(currentMoveType == MoveType.Running && Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) > movementProperties.speed)
					{
						isSlowingDownFromRun = true;
						PlayAnimation();
					}

					currentMoveType = MoveType.Walking;
				}
			}
			else
			{
				if(currentMoveType == MoveType.Running && Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) > movementProperties.speed && controller.slots.physicsObject.IsOnSurface())
				{
					isSlowingDownFromRun = true;
					PlayAnimation();
				}

				currentMoveType = MoveType.Walking;
			}

			if(Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) <= movementProperties.speed)
			{
				isSlowingDownFromRun = false;
			}

			if(isOverriddenByDash || controller.isKnockbackActive || controller.isStunned || controller.StateID() == WallClingState.idString)
			{
				return;
			}

			bool isLockedIntoJump = (jumpState && !controller.slots.physicsObject.IsOnSurface() && jumpState.IsHorizontalMovementFrozen());
			if(isLockedIntoJump)
			{
				ApplyHorizontalJump(jumpState);
				return;
			}

			if(movementProperties == null)
			{
				return;
			}

			float baseSpeed = (isRunning) ? runProperties.speed : movementProperties.speed;
			float baseAcceleration = (isRunning) ? runProperties.acceleration : movementProperties.acceleration;
			float baseDeceleration = (isRunning || isSlowingDownFromRun) ? runProperties.deceleration : movementProperties.deceleration;

			//If we're moving diagonally and willMaintainSpeedOnDiagonal is true, cut our speed in each direction in half to compensate
			float diagonalMultiplier = 0.707f; //Moving in two directions at once is 1.4x faster than just one; therefore, we multiply speed in each direction by 0.707
			bool isMovingDiagonally = (canMoveVertically && willMaintainSpeedOnDiagonal && Mathf.Abs(controller.axis.x) > 0.0f && Mathf.Abs(controller.axis.y) > 0.0f);
			float speed = (isMovingDiagonally) ? baseSpeed * diagonalMultiplier : baseSpeed;
			float acceleration = (isMovingDiagonally) ? baseAcceleration * diagonalMultiplier : baseAcceleration;
			float deceleration = baseDeceleration;

			if(controller.currentState.id == CrouchState.idString)
			{
				CrouchState crouchState = controller.GetComponent<CrouchState>();
				if(crouchState)
				{
					if(crouchState.isSkidComplete && !crouchState.allowAccelerationOnMove)
					{
						acceleration = 0.0f;
						deceleration = 0.0f;
					}

					if(crouchState.isSkidComplete)
					{
						speed = crouchState.moveSpeed;
					}
				}
			}

			if(_inputDirection != 0.0f && !controller.isKnockbackActive && !isLockedForAttack && !controller.IsOveriddenByCrouch() && !isSlowingDownFromRun) 
			{	
				if(movementAxis == MovementAxis.Horizontal) //Let the controller know that we turned around horizontally
				{
					Direction.Horizontal actorDirection = controller.direction.horizontal;
					actorDirection = (_inputDirection > 0.0f) ? Direction.Horizontal.Right : Direction.Horizontal.Left;
					controller.FaceDirection(actorDirection);
				}

				hasEnded = false; //We animate the sprite moving, but only if it's moving horizontally
				if(CanPlayAnimation(movementAxis))
				{
					Begin();
				}

				if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onMove)
				{
					controller.slots.actor.currentAttack.Cancel();
				}

				if(movementAxis == MovementAxis.Horizontal)
				{
					controller.slots.physicsObject.MoveX(speed * _inputDirection);
					controller.slots.physicsObject.properties.acceleration.x = acceleration * _inputDirection;
					controller.slots.physicsObject.properties.deceleration.x = deceleration * _inputDirection;
				}
				else 
				{
					controller.slots.physicsObject.MoveY(speed * _inputDirection);
					controller.slots.physicsObject.properties.acceleration.y = acceleration * _inputDirection;
					controller.slots.physicsObject.properties.deceleration.y = deceleration * _inputDirection;
				}
			} 
			else //We can't move directly, but we can apply residual deceleration
			{
				if(movementAxis == MovementAxis.Horizontal)
				{
					controller.slots.physicsObject.properties.acceleration.x = 0.0f;
					controller.slots.physicsObject.properties.deceleration.x = deceleration;

					if(Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) <= 0.0f && (controller.slots.physicsObject.IsOnSurface() || canMoveVertically))
					{
						if(!hasEnded)
						{
							if(!controller.isKnockbackActive && controller.StateID() != LadderState.idString && controller.StateID() != StairClimbingState.idString)
							{
								controller.SetStateToDefault();
							}
						}

						End();
					}
				}
				else
				{
					controller.slots.physicsObject.properties.acceleration.y = 0.0f;
					controller.slots.physicsObject.properties.deceleration.y = deceleration;
				}
			}
		}

		protected void CheckFootstepSounds()
		{
			footstepSounds.timeSinceLastFootstep += Time.fixedDeltaTime;
			if(footstepSounds.timeSinceLastFootstep >= footstepSounds.durationBetweenFootsteps)
			{
				footstepSounds.timeSinceLastFootstep = 0.0f;
				for(int i = 0; i < footstepSounds.sounds.Count; i ++)
				{
					if(controller.slots.physicsObject.GetSurfaceTag() == footstepSounds.sounds[i].surfaceTag)
					{
						controller.slots.actor.PlaySoundIfOnCamera(footstepSounds.sounds[i].sound, 1.0f, footstepSounds.footstepVolume);
						break;
					}
				}
			}
		}

		protected bool CanPlayAnimation(MovementAxis movementAxis)
		{
			if(((controller.slots.physicsObject.IsOnSurface() && !canMoveVertically) || (canMoveVertically && movementAxis == MovementAxis.Horizontal)) && controller.StateID() != id && controller.currentState.id != JumpState.idString && controller.currentState.id != LadderState.idString && controller.currentState.id != CrouchState.idString)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
