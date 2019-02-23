/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class LadderState:RexState
	{
		[System.Serializable]
		public class Animations
		{
			[Tooltip("The looping animation that plays while the actor climbs a ladder.")]
			public AnimationClip moving;
			[Tooltip("The animation that plays when the actor reaches the top of a ladder.")]
			public AnimationClip cresting;
		}

		public enum Substate
		{
			Stopped,
			Moving,
			Cresting,
			Attacking
		}

		public enum OnJump
		{
			Drop,
			Jump
		}

		public const string idString = "ClimbingLadder";

		[Tooltip("The speed with which the actor climbs ladders.")]
		public float climbSpeed = 5.0f;
		[Tooltip("Whether or not the actor is allowed to turn horizontally while climbing a ladder.")]
		public bool canTurn = true;
		[Tooltip("AnimationClips that play while the actor is on a ladder.")]
		public Animations animations;
		[Tooltip("Allows you to set whether pressing the Jump button on a ladder makes the actor jump upwards or drop down immediately.")]
		public OnJump onJump;

		[HideInInspector]
		public bool isTouching;

		[HideInInspector]
		public bool isClimbing;

		protected BoxCollider2D boxCollider;
		protected float activeLadderXPosition;
		protected float activeLadderTopCrestPosition;
		protected float activeLadderBottomCrestPosition;
		protected float activeLadderTop;
		protected float activeLadderBottom;
		protected Substate substate;
		protected float crestBuffer = 0.5f;

		void Awake() 
		{
			id = idString;
			boxCollider = GetComponent<BoxCollider2D>();
			GetController();
			willPlayAnimationOnBegin = false;
		}

		void Update()
		{
			if(!IsFrozen())
			{
				willAllowDirectionChange = (!canTurn && isClimbing) ? false : true;
				CheckLadders(controller.axis.y);
			}
		}

		#region unique public methods

		public void Drop()
		{
			controller.slots.actor.GetComponent<BoxCollider2D>().enabled = false;
			controller.slots.actor.GetComponent<BoxCollider2D>().enabled = true;
			substate = Substate.Stopped;

			isClimbing = false;
			willAllowDirectionChange = true;
			hasEnded = true;
			controller.framesSinceDrop = 0;

			JumpState jumpState = controller.GetComponent<JumpState>();
			if(jumpState)
			{
				jumpState.OnLadderExit();
			}

			controller.SetStateToDefault();
		}

		public float GetDistanceFromTop()
		{
			return Mathf.Abs(activeLadderTopCrestPosition - transform.position.y);
		}

		public float GetDistanceFromBottom()
		{
			return Mathf.Abs(activeLadderBottomCrestPosition - transform.position.y);
		}

		#endregion

		#region override public methods

		public override void OnEnded()
		{
			substate = Substate.Stopped;
			controller.aerialPeak = transform.position.y;
			isClimbing = false;
			willAllowDirectionChange = true;
			controller.SetStateToDefault();
		}

		public override void OnStateChanged()
		{
			if(controller.StateID() == JumpState.idString && onJump == OnJump.Jump)
			{
				controller.framesSinceDrop = 0;
				End();
			}
		}

		public override void PlayAnimationForSubstate()
		{
			AnimationClip animationToPlay = animation;
			switch(substate)
			{
				case Substate.Stopped:
					animationToPlay = animation;
					break;
				case Substate.Cresting:
					animationToPlay = animations.cresting;
					break;
				case Substate.Moving:
					animationToPlay = animations.moving;
					break;
			}

			if(controller.StateID() == id)
			{
				PlaySecondaryAnimation(animationToPlay);
			}
		}

		public override void UpdateMovement()
		{
			if(isClimbing)
			{
				controller.slots.physicsObject.FreezeGravityForSingleFrame();
				controller.slots.physicsObject.SetVelocityX(0.0f);
				controller.slots.physicsObject.FreezeXMovementForSingleFrame();
			}

			if(isTouching)
			{
				if(isClimbing)
				{
					controller.slots.actor.SetPosition(new Vector2(activeLadderXPosition, transform.position.y));
					controller.slots.physicsObject.SetVelocityY(climbSpeed * (int)controller.axis.y * controller.GravityScaleMultiplier());

					bool isAtLadderCrest = (controller.GravityScaleMultiplier() > 0.0f) ? transform.position.y >= activeLadderTopCrestPosition + boxCollider.size.y * 0.5f - boxCollider.offset.y : transform.position.y <= activeLadderBottomCrestPosition - boxCollider.size.y * 0.5f + boxCollider.offset.y; //TODO: Gravity scale needs offset
					bool willSnapToLadderTop = (controller.GravityScaleMultiplier() > 0.0f) ? transform.position.y >= activeLadderTop + boxCollider.size.y * 0.5f - boxCollider.offset.y : transform.position.y <= activeLadderBottom - boxCollider.size.y * 0.5f + boxCollider.offset.y; //TODO: Gravity scale needs offset

					if(willSnapToLadderTop)
					{
						controller.slots.physicsObject.FreezeYMovementForSingleFrame();
						controller.slots.physicsObject.SetVelocityY(0.0f);
						controller.SetStateToDefault(true, true);
						controller.slots.physicsObject.properties.isFalling = false; //This has to be set to prevent the player from doing their falling animation for one frame
						isTouching = false;
						float snapPosition = (controller.GravityScaleMultiplier() > 0.0f) ? activeLadderTop + boxCollider.size.y * 0.5f - boxCollider.offset.y : activeLadderBottom - boxCollider.size.y * 0.5f + boxCollider.offset.y;
						controller.slots.actor.SetPosition(new Vector2(controller.slots.actor.transform.position.x, snapPosition)); //TODO: Gravity scale

						return;
					}

					if(isAtLadderCrest)
					{
						if(substate != Substate.Cresting && !controller.slots.actor.currentAttack)
						{
							PlaySecondaryAnimation(animations.cresting);
						}

						substate = Substate.Cresting;
					}
					else if((int)controller.axis.y != 0.0f)
					{
						if(substate != Substate.Moving && !controller.slots.actor.currentAttack)
						{
							PlaySecondaryAnimation(animations.moving);
						}

						substate = Substate.Moving;
					}
					else
					{
						if(substate != Substate.Stopped && !controller.slots.actor.currentAttack)
						{
							PlayAnimation();
						}

						substate = Substate.Stopped;
					}

					if(controller.slots.actor.currentAttack)
					{
						substate = Substate.Attacking;
					}
				}
			}
		}

		#endregion

		protected void CheckLadders(float _inputDirection)
		{
			if(isClimbing && isEnabled)
			{
				float topBuffer = 0.1f;
				bool isOnTopOfLadder = (controller.GravityScaleMultiplier() > 0.0f) ? !(boxCollider.bounds.min.y < activeLadderTopCrestPosition - topBuffer) : !(boxCollider.bounds.max.y > activeLadderBottomCrestPosition + topBuffer);
				if(!isOnTopOfLadder) //You're not on top of the ladder
				{
					isTouching = true;
				}
			}

			if(!isEnabled && isClimbing)
			{
				Drop();
			}

			if(!isEnabled || controller.framesSinceDrop < 10)
			{
				return;
			}

			if(isTouching)
			{
				if(_inputDirection == 1.0f && !IsLockedForAttack(Attack.ActionType.Climbing) && !controller.isKnockbackActive && !controller.isStunned && !isClimbing && IsHorizontallyCenteredOnLadder() && !(controller.StateID() == GroundPoundState.idString && !controller.GetComponent<GroundPoundState>().canCancelWithLadder)) //Mount a ladder
				{
					Begin();
					PlayAnimationForSubstate();
					controller.slots.physicsObject.SetVelocityX(0.0f);
					isClimbing = true;
					willAllowDirectionChange = false;
				}
				else if(controller.slots.input && controller.slots.input.isJumpButtonDownThisFrame && isClimbing)
				{
					if(onJump == OnJump.Drop)
					{
						Drop();
						controller.TemporarilyDisableOneWayPlatforms();
					}
				}
				else if(_inputDirection == -1.0f && controller.slots.physicsObject.IsOnSurface() && isClimbing) //Dismounting by climbing to the floor
				{
					if((controller.GravityScaleMultiplier() > 0.0f && transform.position.y < activeLadderTopCrestPosition) || (controller.GravityScaleMultiplier() < 0.0f && transform.position.y > activeLadderBottomCrestPosition)) //TODO: Gravity scale
					{
						End();
					}
				}
			}
			else
			{
				if(!controller.isKnockbackActive/* && !isClimbing*/) //On top of a ladder; going down
				{
					if(_inputDirection == -1.0f && !IsLockedForAttack(Attack.ActionType.Climbing))
					{
						if(RaycastHelper.IsOnSurface("Ladder", (Direction.Vertical)(-1.0f * controller.GravityScaleMultiplier()), boxCollider, controller.GravityScaleMultiplier()))
						{
							Collider2D col = RaycastHelper.GetColliderForSurface("Ladder", (Direction.Vertical)(-1.0f * controller.GravityScaleMultiplier()), boxCollider, controller.GravityScaleMultiplier());
							SetLadderValues(col);

							if(!IsHorizontallyCenteredOnLadder())
							{
								return;
							}

							controller.slots.actor.SetPosition(new Vector2(activeLadderXPosition, transform.position.y));
							controller.slots.physicsObject.SetVelocityX(0.0f);
							controller.slots.physicsObject.SetVelocityY(0.0f);

							isTouching = true;
							isClimbing = true;
							willAllowDirectionChange = false;
							Begin();

							float snapPosition = (controller.GravityScaleMultiplier() > 0.0f) ? (activeLadderTop + boxCollider.size.y * 0.5f - boxCollider.offset.y - (crestBuffer * 0.5f)) : (activeLadderBottom - boxCollider.size.y * 0.5f + boxCollider.offset.y + (crestBuffer * 0.5f));
							controller.slots.actor.SetPosition(new Vector2(transform.position.x, snapPosition));
							controller.slots.physicsObject.FreezeGravityForSingleFrame();

							substate = Substate.Cresting;
							PlayAnimationForSubstate();
						}
					}
				}
			}
		}

		protected void SetLadderValues(Collider2D col)
		{
			activeLadderXPosition = col.transform.position.x;
			activeLadderTopCrestPosition = col.bounds.max.y - crestBuffer;
			activeLadderBottomCrestPosition = col.bounds.min.y + crestBuffer;
			activeLadderTop = col.bounds.max.y;
			activeLadderBottom = col.bounds.min.y;
		}

		protected bool IsHorizontallyCenteredOnLadder()
		{
			float gridSize = 1.0f;
			float buffer = 0.675f * gridSize;
			return (Mathf.Abs(transform.position.x - activeLadderXPosition) < buffer);
		}

		protected virtual void ProcessCollision(Collider2D col)
		{
			float topBuffer = 0.1f;
			bool isOnTopOfLadder = (controller.GravityScaleMultiplier() > 0.0f) ? !(boxCollider.bounds.min.y < col.bounds.max.y - topBuffer) : !(boxCollider.bounds.max.y > col.bounds.min.y + topBuffer);
			if(col.tag == "Ladder" && !isOnTopOfLadder) //You're not on top of the ladder
			{
				isTouching = true;
				SetLadderValues(col);
			}
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			ProcessCollision(col);
		}

		protected void OnLadderExited(Collider2D col)
		{
			bool isOnTop = (controller.GravityScaleMultiplier() > 0.0f) ? boxCollider.bounds.min.y >= col.bounds.max.y : boxCollider.bounds.max.y <= col.bounds.min.y;
			if(controller.slots.physicsObject && isOnTop) //You're on top of the ladder
			{
				isClimbing = false;
				willAllowDirectionChange = true;
				hasEnded = true;

				controller.slots.physicsObject.SetVelocityY(0.0f);
				float topOfLadderPosition = (controller.GravityScaleMultiplier() > 0.0f) ? boxCollider.size.y * 0.5f - boxCollider.offset.y + col.bounds.max.y : col.bounds.min.y - boxCollider.size.y * 0.5f - boxCollider.offset.y;
				controller.slots.actor.SetPosition(new Vector2(transform.position.x, topOfLadderPosition)); //Need to account for the offset of the collider (in the above if statement too)
			}

			controller.SetStateToDefault();
		}

		protected void OnTriggerExit2D(Collider2D col)
		{
			if(col.tag == "Ladder")
			{
				if(isTouching && isEnabled && isClimbing)
				{
					OnLadderExited(col);
				}

				isClimbing = false;
				willAllowDirectionChange = true;
				isTouching = false;
			}
		}
	}
}
