/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class BounceState:RexState 
	{
		[Tooltip("The minimum number of frames the bounce will go, assuming the jump button is let go of as soon as possible after the bounce begins.")]
		public int minFrames = 6;
		[Tooltip("The maximum number of frames the bounce will go, assuming the jump button is held as long as possible after the bounce begins.")]
		public int maxFrames = 15;
		[Tooltip("The upward speed of the bounce.")]
		public float speed = 8.0f;
		[Tooltip("Whether or not a bounce can be initiated if the actor is already performing a ground pound.")]
		public bool canBounceFromGroundPound = false;
		[Tooltip("The damage the bounce deals to the actor being bounced on top of.")]
		public int damageDealt = 1;

		public const string idString = "Bouncing";

		protected int currentBounceFrame = 0;
		protected bool isBounceActive;

		void Awake() 
		{
			id = idString;
			isConcurrent = true;
			blockAutoChange.toFalling = false;
			blockAutoChange.toDefaultOnLanding = false;
			GetController();
		}

		#region unique public methods

		//Called automatically in collision handling of RexActor
		public void StartBounce(Collider2D bouncerCol, Collider2D otherCol)
		{
			if(!isBounceActive)
			{
				Begin();
				currentBounceFrame = 0;
				controller.slots.physicsObject.properties.isFalling = false;
				isBounceActive = true;
				controller.slots.physicsObject.SetVelocityY(0.0f);
				controller.slots.physicsObject.FreezeGravityForSingleFrame();

				float newY = transform.position.y; //Adjust our position so we aren't inside the thing we're bouncing on
				float buffer = 0.05f;
				float adjustedDistance = (PhysicsManager.Instance.gravityScale >= 0.0f) ? Mathf.Abs(bouncerCol.bounds.min.y - otherCol.bounds.max.y) + buffer : -Mathf.Abs(otherCol.bounds.min.y - bouncerCol.bounds.max.y) - buffer;
				newY += adjustedDistance;
				controller.slots.actor.SetPosition(new Vector2(transform.position.x, newY));
			}
		}

		//Uses positioning and collider data to determine if we can start a bounce from an object
		public bool CanBounce(Collider2D bouncerCol, Collider2D otherCol)
		{
			bool canBounce = false;
			if(isEnabled && controller.isEnabled && IsColliderBelow(bouncerCol, otherCol) && !controller.slots.physicsObject.IsOnSurface() && !(controller.StateID() == GroundPoundState.idString && !canBounceFromGroundPound))
			{
				RexActor actorToBounceOn = otherCol.GetComponent<RexActor>();
				if(actorToBounceOn != null && actorToBounceOn.canBounceOn)
				{
					canBounce = true;
				}
			}

			return canBounce;
		}

		#endregion

		#region override public methods

		public override void OnBegin()
		{
			if(GetComponent<JumpState>())
			{
				GetComponent<JumpState>().OnBounce();
			}
		}

		public override void OnEnded()
		{
			isBounceActive = false;
		}

		public override bool AttemptContactEffect(Collider2D thisCollider, Collider2D otherCollider)
		{
			if(CanBounce(thisCollider, otherCollider))
			{
				DoContactDamage(otherCollider);
				return true;
			}

			return false;
		}

		public override void UpdateMovement()
		{
			if(isBounceActive) //The bounce is active, so we update its movement
			{
				currentBounceFrame ++;
				if(currentBounceFrame >= maxFrames) //End the bounce
				{
					currentBounceFrame = 0;
					isBounceActive = false;
					controller.slots.actor.NotifyOfControllerJumpCresting();
					controller.slots.physicsObject.SetVelocityY(0.0f);
					End();
				}
				else //Continue the bounce
				{
					controller.slots.physicsObject.ApplyForce(new Vector2(0.0f, speed * controller.GravityScaleMultiplier()));
				}
			}
		}

		public override void OnStateChanged()
		{
			if(controller.StateID() == JumpState.idString)
			{
				isBounceActive = false;
				currentBounceFrame = 0;
				End();
			}
			else if(controller.StateID() == KnockbackState.idString)
			{
				End();
			}
		}

		#endregion

		#region override protected methods

		protected override void OnDoContactDamage(RexActor damagedActor)
		{
			StartBounce(controller.slots.actor.slots.collider, damagedActor.GetComponent<Collider2D>());

			damagedActor.Damage(damageDealt, false, BattleEnums.DamageType.Regular, controller.slots.actor.slots.collider);
			damagedActor.OnBouncedOn(controller.slots.actor.slots.collider);
		}

		#endregion
	}

}
