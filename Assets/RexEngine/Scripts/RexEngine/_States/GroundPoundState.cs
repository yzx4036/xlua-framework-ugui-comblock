using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class GroundPoundState:RexState 
	{
		public const string idString = "GroundPound";

		public bool canMoveHorizontally;
		public int hoverFrames = 12;
		public int fallSpeed = 24;
		public int stunFramesOnLand = 12;
		public bool canCancelWithJump = false;
		public bool canCancelWithDash = false;
		public bool canCancelWithLadder = false;
		public bool shakeScreenOnImpact = true;
		public Animations animations;

		[Tooltip("Whether or not the actor is invincible during the ground pound.")]
		public bool isInvincibleDuringGroundPound = false;

		[Tooltip("The damage the ground pound deals to enemies.")]
		public int damageDealt = 1;

		protected int currentHoverFrame = 0;
		protected int currentStunFrame = 0;
		protected Substate substate;

		[System.Serializable]
		public class Animations
		{
			public AnimationClip beginAnimation;
			public AnimationClip hitGroundAnimation;
		}

		public enum Substate
		{
			Beginning,
			Body,
			Landing
		}

		void Awake() 
		{
			id = idString;
			willPlayAnimationOnBegin = false;
			GetController();
		}

		void Update() 
		{
			if(isEnabled && !IsFrozen() && !controller.isKnockbackActive)
			{
				bool isGroundPoundAttempted = controller.slots.input && controller.slots.input.IsDownButtonDownThisFrame() && !controller.slots.physicsObject.IsOnSurface() && controller.StateID() != WallClingState.idString && controller.StateID() != LadderState.idString;
				if(isGroundPoundAttempted && !IsLockedForAttack(Attack.ActionType.GroundPounding))
				{
					Begin();
				}
			}
		}

		#region unique public methods

		public bool CanDamage(Collider2D bouncerCol, Collider2D otherCol)
		{
			bool canDamage = false;
			if(isEnabled && controller.isEnabled && controller.StateID() == idString && IsColliderBelow(bouncerCol, otherCol) && currentStunFrame < 2/* && !controller.slots.physicsObject.IsOnSurface()*/)
			{
				RexActor actorToGroundPound = otherCol.GetComponent<RexActor>();
				if(actorToGroundPound != null && actorToGroundPound.canGroundPoundOn)
				{
					canDamage = true;
				}
			}

			return canDamage;
		}

		#endregion

		#region override public methods

		public override void UpdateMovement()
		{
			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.cancels.groundPound)
			{
				if(!hasEnded && controller.StateID() == id)
				{
					controller.SetStateToDefault();
				}

				End();
			}

			if(currentHoverFrame < hoverFrames)
			{
				controller.slots.physicsObject.FreezeGravityForSingleFrame();
				controller.slots.physicsObject.FreezeYMovementForSingleFrame();
				currentHoverFrame ++;
			}
			else
			{
				if(controller.slots.physicsObject.IsOnSurface())
				{
					if(currentStunFrame == 0)
					{
						if(animations.hitGroundAnimation)
						{
							PlaySecondaryAnimation(animations.hitGroundAnimation);
						}

						blockMovingStateMovement = true;
						controller.isStunned = true;
						substate = Substate.Landing;

						if(shakeScreenOnImpact)
						{
							ScreenShake.Instance.Shake();
						}
					}

					if(currentStunFrame >= stunFramesOnLand)
					{
						controller.isStunned = false;
						controller.SetStateToDefault();
					}

					currentStunFrame ++;
				}
				else
				{
					controller.slots.physicsObject.FreezeGravityForSingleFrame();
					controller.slots.physicsObject.MoveY(-controller.GravityScaleMultiplier() * fallSpeed);
				}
			}
		}

		public override void OnBegin()
		{
			if(controller.slots.actor.currentAttack != null && controller.slots.actor.currentAttack.canceledBy.onGroundPound)
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

			blockMovingStateMovement = !canMoveHorizontally;
			currentHoverFrame = 0;
			currentStunFrame = 0;
			controller.slots.physicsObject.SetVelocityY(0);
			substate = Substate.Beginning;

			StartCoroutine("BeginCoroutine");
		}

		public override void OnStateChanged()
		{
			if(controller.slots.physicsObject.IsOnSurface())
			{
				controller.isStunned = false;
			}
		}

		public override bool AttemptContactEffect(Collider2D thisCollider, Collider2D otherCollider)
		{
			if(CanDamage(thisCollider, otherCollider))
			{
				DoContactDamage(otherCollider);
				return true;
			}

			return false;
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

		#endregion

		#region override protected methods

		protected override void OnDoContactDamage(RexActor damagedActor)
		{
			damagedActor.Damage(damageDealt, false, BattleEnums.DamageType.Regular, controller.slots.actor.slots.collider);
			controller.slots.actor.ToggleCollider();
		}

		#endregion
	}
}
