/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class BaseJumpState:RexState
	{
		public const string idString = "BaseJump";

		public enum JumpType
		{
			Finite,
			Infinite,
			None
		}

		[System.Serializable]
		public class Animations
		{
			[Tooltip("The AnimationClip that plays at the very beginning of the jump. This should not loop.")]
			public AnimationClip startingJump;
			[Tooltip("The AnimationClip that plays during the body of the jump. This should loop. If this is not slotted, the AnimationClip slotted in the default Animation slot will play instead.")]
			public AnimationClip jumpBody;
			[Tooltip("The AnimationClip that plays at the top of the jump arc. This should not loop.")]
			public AnimationClip crestingJump;
		}

		public enum Substate
		{
			Starting,
			Body,
			Cresting
		}

		public bool killJumpOnCeilingHit = true;

		[Tooltip("The vertical speed of the jump.")]
		public float speed = 12.0f;
		[Tooltip("The minimum number of frames the jump will go, assuming the jump button is let go of as soon as possible after the jump begins.")]
		public int minFrames = 12;
		[Tooltip("The maximum number of frames the jump will go, assuming the jump button is held down for as long as possible after the jump begins.")]
		public int maxFrames = 15;

		[Tooltip("The number of frames the actor will hang in midair with no vertical movement before descending at the peak of their jump arc.")]
		public int hangtimeFrames = 0;

		[Tooltip("Setting this above 0 allows you to initiate a jump by pressing the jump button slightly before the actor fully lands.")]
		public float buttonBufferDuration = 0.1f;

		[Tooltip("AnimationClips that play for the start, body, and crest of the primary jump.")]
		public Animations animations;

		[Tooltip("AnimationClips that play for the start, body, and crest of secondary jumps, assuming multi-jumping is enabled. If multi-jumping is enabled and these are not slotted, multi-jumps will play the primary jump animations instead.")]
		public Animations secondaryAnimations;

		[Tooltip("AnimationClips that play for the start, body, and crest of wall jumps, assuming wall jumps are enabled (this requires a WallClingState component).")]
		public Animations wallJumpAnimations;

		[HideInInspector]
		public Substate substate;

		[HideInInspector]
		public Direction.Horizontal direction;

		[HideInInspector]
		public bool isGroundedWithJumpButtonUp = false;

		[HideInInspector]
		public int framesFrozenForWallJump;

		protected bool hasReleasedButtonSinceJump;
		protected int currentJumpFrame = 0;
		protected int currentHangtimeFrame;
		protected float jumpStartY; //Just used for logging the jump height
		protected int currentJump;
		protected bool isJumpActive = false;
		protected float adjustedSpeed;
		protected bool isWallJumpKickbackActive;
		protected float currentButtonBuffer = 0.0f;
		protected bool isJumpBuffered;

		void Awake()
		{
			id = idString;
			isConcurrent = true;
			willPlayAnimationOnBegin = false;
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

		protected void CheckJumpBuffer()
		{
			if(!isJumpBuffered && controller.slots.input && controller.slots.input.isJumpButtonDownThisFrame)
			{
				if(!controller.slots.physicsObject.IsOnSurface())
				{
					currentButtonBuffer = 0.0f;
					isJumpBuffered = true;
				}
			}
			else if(isJumpBuffered)
			{
				currentButtonBuffer += Time.deltaTime;
				if(currentButtonBuffer > buttonBufferDuration)
				{
					currentButtonBuffer = 0.0f;
					isJumpBuffered = false;
				}
			}
		}

		#region unique public methods

		public bool IsJumpActive()
		{
			return isJumpActive;
		}

		public void OnBounce()
		{
			currentJump = 1;
		}

		public void OnLadderExit()
		{
			currentJump = 0;
		}

		public bool CanEnd()
		{
			return (currentJumpFrame > 1) ? true : false;
		}

		#endregion

		#region override public methods

		public override void OnBegin()
		{
			currentJumpFrame = 0;
			currentHangtimeFrame = 0;
			isJumpActive = true;
			hasReleasedButtonSinceJump = false;
			controller.slots.physicsObject.properties.isFalling = false;
			isGroundedWithJumpButtonUp = false;

			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onJump)
			{
				controller.slots.actor.currentAttack.Cancel();
			}

			StartCoroutine("JumpCoroutine");

			controller.slots.actor.NotifyOfControllerJumping(currentJump);
		}

		public override void OnStateChanged()
		{
			EndOtherStates();
		}

		public override void OnEnded()
		{
			currentJumpFrame = 0;
			isJumpActive = false;
			//framesFrozenForWallJump = 0;

			StopCoroutine("JumpCoroutine");
		}

		public override void UpdateMovement()
		{
			if(!controller.isKnockbackActive)
			{
				if((controller.slots.input != null && controller.slots.input.isJumpButtonDown && !hasReleasedButtonSinceJump) || (currentJumpFrame < minFrames && isJumpActive))
				{
					if(currentJumpFrame == 0)
					{
						jumpStartY = transform.position.y; //Just used for debugging
					}

					if(isJumpActive) //Continue a jump
					{
						if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.flutterJump) //TODO: Add this to regular jump?
						{
							if(!hasEnded && controller.StateID() == id)
							{
								controller.SetStateToDefault();
							}

							End();
						}

						currentJumpFrame ++;

						int totalJumpFrames = maxFrames;
						if(currentJumpFrame >= totalJumpFrames || (controller.slots.physicsObject.DidHitCeilingThisFrame() && killJumpOnCeilingHit)) //The jump ends because we hit the max number of frames we allowed it
						{
							substate = Substate.Cresting;
							isJumpActive = false;
							End(); //TODO: Didn't have this before; added it for flutterjump

							if(!controller.slots.actor.IsAttacking() && controller.StateID() == id)
							{
								Animations currentAnimations = (currentJump > 1) ? secondaryAnimations : animations;
								if(isWallJumpKickbackActive)
								{
									currentAnimations = wallJumpAnimations;
								}

								AnimationClip crestingJumpAnimation = currentAnimations.crestingJump;

								PlaySecondaryAnimation(crestingJumpAnimation);
							}

							controller.slots.actor.NotifyOfControllerJumpCresting();

							currentHangtimeFrame = 0;

							//float jumpHeight = Mathf.Abs(transform.position.y - jumpStartY);
							//Debug.Log("Position is: " + transform.position.y + "     Jump height: " + jumpHeight);
						}
						else
						{
							controller.slots.physicsObject.ApplyForce(new Vector2(0.0f, adjustedSpeed * controller.GravityScaleMultiplier()));
						}
					}
					else if(substate == Substate.Cresting && currentHangtimeFrame < hangtimeFrames)
					{
						CheckHangtimeFrames();
					}
				}
				else //The jump ends because the input requesting the jump ended
				{
					if(isJumpActive)
					{
						substate = Substate.Cresting;
						isJumpActive = false;

						if(controller.StateID() == FlutterJumpState.idString)
						{
							End();
						}

						if(!controller.slots.actor.IsAttacking() && controller.StateID() == id)
						{
							Animations currentAnimations = (currentJump > 1) ? secondaryAnimations : animations;
							if(isWallJumpKickbackActive)
							{
								currentAnimations = wallJumpAnimations;
							}

							AnimationClip crestingJumpAnimation = currentAnimations.crestingJump;

							PlaySecondaryAnimation(crestingJumpAnimation);
						}

						controller.slots.actor.NotifyOfControllerJumpCresting();
					}
					else if(substate == Substate.Cresting && currentHangtimeFrame < hangtimeFrames)
					{
						CheckHangtimeFrames();
					}
				}
			}
			else
			{
				currentJumpFrame = 0;
				isJumpActive = false;
			}

			HandleJumpExtras();
		}

		public override void PlayAnimationForSubstate()
		{
			Animations currentAnimations = (currentJump > 1) ? secondaryAnimations : animations;
			if(isWallJumpKickbackActive)
			{
				currentAnimations = wallJumpAnimations;
			}

			AnimationClip animationToPlay = animation;
			switch(substate)
			{
				case Substate.Body:
					animationToPlay = (currentAnimations.jumpBody != null) ? currentAnimations.jumpBody : animations.jumpBody;
					if(animationToPlay == null)
					{
						animationToPlay = animation;
					}
					break;
				case Substate.Cresting:
					animationToPlay = currentAnimations.crestingJump;
					break;
				case Substate.Starting:
					animationToPlay = currentAnimations.startingJump;
					break;
			}

			if(controller.StateID() == id)
			{
				PlaySecondaryAnimation(animationToPlay);
			}
		}

		#endregion

		protected virtual void HandleJumpExtras(){}

		protected virtual void ResetFlags(){}

		protected virtual IEnumerator JumpCoroutine()
		{
			substate = Substate.Starting;
			float duration = 0.0f;

			Animations currentAnimations = (currentJump > 1) ? secondaryAnimations : animations;
			if(isWallJumpKickbackActive)
			{
				currentAnimations = wallJumpAnimations;
			}

			if(!controller.slots.actor.IsAttacking())
			{
				AnimationClip startingJumpAnimation = currentAnimations.startingJump;
				PlaySecondaryAnimation(startingJumpAnimation);
			}

			while(controller.slots.actor.timeStop.isTimeStopped) yield return null;
			yield return new WaitForSeconds(duration);

			if(isJumpActive)
			{
				substate = Substate.Body;

				if(!controller.slots.actor.IsAttacking())
				{
					AnimationClip bodyAnimation = (currentAnimations.jumpBody != null) ? currentAnimations.jumpBody : animations.jumpBody;
					if(bodyAnimation == null)
					{
						bodyAnimation = animation;
					}

					PlaySecondaryAnimation(bodyAnimation);
				}
			}
		}

		protected void CheckHangtimeFrames()
		{
			if((controller.slots.physicsObject.properties.velocity.y <= 0.0f && controller.GravityScaleMultiplier() > 0.0f) || (controller.slots.physicsObject.properties.velocity.y >= 0.0f && controller.GravityScaleMultiplier() < 0.0f))
			{
				currentHangtimeFrame ++;
				controller.slots.physicsObject.SetVelocityY(0.0f);
				controller.slots.physicsObject.FreezeGravityForSingleFrame();
			}
		}

		protected void EndOtherStates()
		{
			if(controller.StateID() == DashState.idString)
			{
				currentJumpFrame = 0;
				framesFrozenForWallJump = 0;
			}
			else if(controller.StateID() == LadderState.idString)
			{
				End();
			}
			else if(controller.StateID() == KnockbackState.idString)
			{
				End();
			}
		}
	}

}
