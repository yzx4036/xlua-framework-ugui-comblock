/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class FallingPlatform:PhysicsMover
	{
		public float fallSpeed = 10.0f;
		public float durationBeforeFall = 0.0f;
		public RexPhysics physicsObject;
		public AudioClip fallSound;

		[HideInInspector]
		public bool hasPlayerOnTop;

		protected bool isFalling;
		protected Vector2 distanceMovedThisFrame;

		void Awake() 
		{
			framesBeforeRemoval = 0;

			if(!physicsObject)
			{
				physicsObject = GetComponent<RexPhysics>();
			}
		}

		void FixedUpdate() 
		{
			if(isFalling)
			{
				MoveVertical();
			}

			if(hasPlayerOnTop && !isFalling)
			{
				StartCoroutine("PreFallCoroutine");
			}
		}

		public override void NotifyOfObjectOnTop(RexPhysics _physicsObject)
		{
			hasPlayerOnTop = true;
		}

		public override void MovePhysics(RexPhysics _physicsObject)
		{
			if(_physicsObject.IsOnSurface())
			{
				distanceMovedThisFrame = physicsObject.GetPositionChangeFromLastFrame();
				_physicsObject.ApplyDirectTranslation(distanceMovedThisFrame);
			}
		}

		protected IEnumerator PreFallCoroutine()
		{
			yield return new WaitForSeconds(durationBeforeFall);
		
			PlaySound(fallSound);
			isFalling = true;
		}

		protected void MoveVertical()
		{
			if(fallSpeed == 0.0f)
			{
				return;
			}

			physicsObject.SetVelocityY(-fallSpeed);
		}
	}
}
