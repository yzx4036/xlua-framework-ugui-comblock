/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class CrouchState:RexState 
	{
		public const string idString = "Crouching";

		[Tooltip("The size of the actor's collider while crouching.")]
		public Vector2 colliderSize = new Vector2(1.0f, 0.9f);
		[Tooltip("The offset of the actor's collider while crouching.")]
		public Vector2 colliderOffset = new Vector2(0.0f, -0.48f);
		[Tooltip("The speed of the actor while crouch-walking (crawling?)")]
		public float moveSpeed = 5.0f;
		[Tooltip("If True, the actor will stand up as soon as the Down button is released; if False, they remain crouching until the Up button is pressed.")]
		public bool willRiseWithButtonRelease;
		[Tooltip("Whether or not the actor can move horizontally while crouching.")]
		public bool canMove;
		[Tooltip("Whether or not the actor can begin a jump out of their crouching state.")]
		public bool canJump;
		[Tooltip("If Can Move is enabled, this determines if the player must first release the left/right button after crouching before they can crouch-walk.")]
		public bool mustReleaseButtonToMove = true;
		[Tooltip("If True, crouching immediately stops the actor in their tracks, regardless of any residual deceleration.")]
		public bool immediatelyKillDecelerationOnCrouch = true;
		[Tooltip("The AnimationClip this actor plays while crouch-walking.")]
		public AnimationClip movingAnimation;
		[Tooltip("Whether or not acceleration is enabled while this actor crouch-walks. Note that acceleration must also be enabled on the attached MovingState for this to work.")]
		public bool allowAccelerationOnMove;

		[HideInInspector]
		public bool isSkidComplete;

		[HideInInspector]
		public bool hasPlayerReleasedHorizontal = false;

		protected bool hasStoppedMovingSinceCrouch;

		protected BoxCollider2D actorCollider;
		protected BoxCollider2D controllerCollider;

		protected Substate substate;
		protected Vector2 nonCrouchingColliderSize;
		protected Vector2 nonCrouchingColliderOffset;

		protected bool isCrouchActive;

		public enum Substate
		{
			Stopped,
			Moving
		}

		void Awake() 
		{
			id = idString;
			blockAutoChange.toFalling = false;
			blockAutoChange.toDefaultOnLanding = false;
			GetController();
		}

		void Start()
		{
			GetColliders();

			if(!controller)
			{
				controller = GetComponent<RexController>();
				if(controller)
				{
					controller.AddState(this);
				}
			}

			EnemyAI enemyAI = GetComponent<EnemyAI>();
			if(enemyAI)
			{
				enemyAI.OnNewStateAdded(this);
			}
		}

		void Update()
		{
			if(!IsFrozen() && isEnabled && controller.isEnabled)
			{
				if(controller.axis.y == -1.0f)
				{
					bool isBlockedByStairs = controller.StateID() == StairClimbingState.idString;
					if(!isBlockedByStairs && controller.slots.physicsObject.IsOnSurface() && controller.StateID() != LadderState.idString && controller.StateID() != JumpState.idString && controller.StateID() != GroundPoundState.idString && controller.StateID() != KnockbackState.idString)
					{
						Begin();
					}
				}
				else if(isCrouchActive)
				{
					if(CanExitCrouch())
					{
						if(willRiseWithButtonRelease)
						{
							ExitCrouch();
						}
						else if(controller.axis.y == 1.0f)
						{
							ExitCrouch();
						}
					}
				}

				if(isCrouchActive)
				{
					if(canMove && Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) > 0.0f)
					{
						if(substate != Substate.Moving)
						{
							substate = Substate.Moving;
							PlaySecondaryAnimation(movingAnimation);
						}
					}
					else
					{
						if(substate != Substate.Stopped)
						{
							substate = Substate.Stopped;
							PlayAnimation();
						}
					}

					if(controller.axis.x == 0.0f)
					{
						hasPlayerReleasedHorizontal = true;
					}
				}
			}
		}

		public override void PlayAnimationForSubstate()
		{
			switch(substate)
			{
				case Substate.Stopped:
					PlayAnimation();
					break;
				case Substate.Moving:
					PlaySecondaryAnimation(movingAnimation);
					break;
			}
		}

		#region unique public methods

		public bool WillAllowMovement()
		{
			bool willAllowMovement = true;
			if(!canMove)
			{
				willAllowMovement = false;
			}

			if(!hasPlayerReleasedHorizontal && mustReleaseButtonToMove)
			{
				willAllowMovement = false;
			}


			if(mustReleaseButtonToMove && !hasStoppedMovingSinceCrouch)
			{
				willAllowMovement = false;
			}

			if(!isSkidComplete)
			{
				willAllowMovement = false;
			}

			return willAllowMovement;
		}

		public bool CanExitCrouch()
		{
			bool canExit = true;
			Direction.Vertical direction = (Direction.Vertical)controller.GravityScaleMultiplier();

			if(RaycastHelper.IsUnderOverhang(direction, nonCrouchingColliderSize, actorCollider.transform.position))
			{
				canExit = false;
			}

			return canExit;
		}

		#endregion

		#region override public methods

		public override void UpdateMovement()
		{
			if(controller.slots.physicsObject.properties.velocity.x == 0.0f)
			{
				hasStoppedMovingSinceCrouch = true;
			}

			if(controller.slots.physicsObject.properties.velocity.x == 0.0f)
			{
				isSkidComplete = true;
			}

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.crouch)
			{
				if(CanExitCrouch())
				{
					ExitCrouch();
				}
			}
		}

		public override void OnBegin()
		{ 
			isCrouchActive = true;

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onCrouch)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			if(actorCollider == null)
			{
				GetColliders();
			}

			SetToCrouchingCollider();

			isSkidComplete = false;

			if(immediatelyKillDecelerationOnCrouch)
			{
				controller.slots.physicsObject.SetVelocityX(0.0f);
				controller.slots.physicsObject.properties.deceleration.x = 0.0f;
				controller.slots.physicsObject.properties.acceleration.x = 0.0f;
				isSkidComplete = true;
			}

			if(controller.GetComponent<MovingState>().movementProperties.deceleration == 0.0f)
			{
				isSkidComplete = true;
			}

			if(mustReleaseButtonToMove)
			{
				if(Mathf.Abs(controller.slots.physicsObject.properties.velocity.x) > 0.0f)
				{
					hasStoppedMovingSinceCrouch = false;
				}
				else
				{
					hasStoppedMovingSinceCrouch = true;
				}
			}

			if(!canMove || mustReleaseButtonToMove)
			{
				if(Mathf.Abs(controller.axis.x) > 0.0f)
				{
					hasPlayerReleasedHorizontal = false;
				}
				else
				{
					hasPlayerReleasedHorizontal = true;
				}
			}
		}

		public override void OnStateChanged()
		{
			if(controller.StateID() != DefaultState.idString)
			{
				SetToNonCrouchingCollider();
			}

			if(controller.StateID() == JumpState.idString)
			{
				SetToNonCrouchingCollider();
				End();
			}
		}

		#endregion

		protected void ExitCrouch()
		{
			isCrouchActive = false;
			controller.SetStateToDefault();
			SetToNonCrouchingCollider();
		}

		protected void SetToCrouchingCollider()
		{
			actorCollider.size = colliderSize;
			actorCollider.offset = colliderOffset;

			controllerCollider.size = colliderSize;
			controllerCollider.offset = colliderOffset;
		}

		protected void SetToNonCrouchingCollider()
		{
			actorCollider.size = nonCrouchingColliderSize;
			actorCollider.offset = nonCrouchingColliderOffset;

			controllerCollider.size = nonCrouchingColliderSize;
			controllerCollider.offset = nonCrouchingColliderOffset;
		}

		protected void GetColliders()
		{
			actorCollider = controller.slots.actor.GetComponent<BoxCollider2D>();
			controllerCollider = controller.GetComponent<BoxCollider2D>();

			nonCrouchingColliderSize = actorCollider.size;
			nonCrouchingColliderOffset = actorCollider.offset;
		}
	}
}
