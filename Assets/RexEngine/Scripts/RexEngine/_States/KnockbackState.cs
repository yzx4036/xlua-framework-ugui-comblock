/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class KnockbackState:RexState 
	{
		[Tooltip("The speed with which the actor is knocked back upon taking damage.")]
		public float speed = 12.0f;
		[Tooltip("The number of frames the actor will be knocked back for upon taking damage.")]
		public int maxFrames = 12;

		[HideInInspector]
		public Direction.Horizontal knockbackDirection = Direction.Horizontal.Right;

		public const string idString = "Knockback";

		protected int currentKnockbackFrame = 0;
		protected int framesBeforeControlResumesAfterKnockback;

		void Awake() 
		{
			id = idString;
			framesBeforeControlResumesAfterKnockback = maxFrames + 16;
			isEnabled = true;
			GetController();
		}

		void Start() 
		{

		}

		#region override public methods

		public override void OnBegin()
		{
			currentKnockbackFrame = 0;
		}

		public override void OnEnded()
		{
			currentKnockbackFrame = 0;
			controller.slots.physicsObject.SetVelocityX(0.0f);
			controller.isKnockbackActive = false;
			controller.SetStateToDefault();
		}

		public override void OnStateChanged()
		{
			currentKnockbackFrame = 0;
			controller.isKnockbackActive = false;
		}

		#endregion

		public override void UpdateMovement()
		{
			if(controller.isKnockbackActive)
			{
				if((PhysicsManager.Instance.gravityScale > 0.0f && controller.slots.physicsObject.properties.velocity.y > 0.0f) || (PhysicsManager.Instance.gravityScale < 0.0f && controller.slots.physicsObject.properties.velocity.y < 0.0f)) //Stop actors from jumping on knockback
				{
					controller.slots.physicsObject.SetVelocityY(0.0f);
				}

				if(currentKnockbackFrame < maxFrames)
				{
					controller.slots.physicsObject.properties.velocityCap.x = speed * (int)knockbackDirection;
					controller.slots.physicsObject.SetVelocityX(speed * (int)knockbackDirection);
					controller.slots.physicsObject.properties.deceleration.x = 0.0f;
					controller.slots.physicsObject.properties.acceleration.x = 0.0f;

					if(controller.slots.physicsObject.properties.velocity.y > 0.0f)
					{
						controller.slots.physicsObject.SetVelocityY(0.0f);
					}
				}
				else if(currentKnockbackFrame >= maxFrames && currentKnockbackFrame < framesBeforeControlResumesAfterKnockback)
				{
					controller.slots.physicsObject.SetVelocityX(0.0f);
					controller.slots.physicsObject.properties.deceleration.x = 0.0f;
					controller.slots.physicsObject.properties.acceleration.x = 0.0f;
				}
				else if(currentKnockbackFrame >= framesBeforeControlResumesAfterKnockback)
				{
					End();
				}

				currentKnockbackFrame ++;
			}
		}
	}
}
