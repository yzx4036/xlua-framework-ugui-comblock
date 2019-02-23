/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class LandingState:RexState
	{
		[Tooltip("If the actor falls a greater distance than this, they'll be stunned for the duration of the landing animation. Setting this to 0.0f disables the stun altogether.")]
		public float fallDistanceForStun = 0.0f;
		[Tooltip("If Fall Distance For Stun is higher than 0.0, this lets you slot an AnimationClip to play when the actor is stunned upon landing.")]
		public AnimationClip stunnedAnimation;
		[Tooltip("If slotted, this RexParticle will play when the actor lands on a surface.")]
		public RexPool landingParticlePool;
		[Tooltip("If Landing Particle Pool is slotted, this lets you adjust the positioning of the landing particle.")]
		public Vector2 particleOffset;
		[Tooltip("If True, the landing animation will override other animations, including the moving animation, until it completes.")]
		public bool animationOverridesOtherAnimations;

		public const string idString = "Landing";

		protected bool willStun;

		void Awake() 
		{
			id = idString;
			willPlayAnimationOnBegin = false;
			blockAutoChange.toFalling = false;
			GetController();
		}

		#region unique public methods

		public void CheckStun(float distanceFallen)
		{
			willStun = (Mathf.Abs(fallDistanceForStun) > 0.0f && distanceFallen >= fallDistanceForStun);
		}

		#endregion

		#region override public methods

		public override void OnBegin()
		{
			if(willStun)
			{
				controller.Stun();
			}

			StartCoroutine("LandingCoroutine");
		}

		public override void OnEnded()
		{
			StopCoroutine("LandingCoroutine");
			controller.SetStateToDefault();
		}

		public override void OnStateChanged()
		{
			StopCoroutine("LandingCoroutine");
		}

		#endregion

		protected virtual IEnumerator LandingCoroutine()
		{
			AnimationClip animationClip = (willStun && stunnedAnimation != null) ? stunnedAnimation : animation;
			float duration = (animationClip != null ) ? animationClip.length : 0.0f;

			if(willStun && stunnedAnimation != null)
			{
				PlaySecondaryAnimation(animationClip);
			}
			else
			{
				if(animationOverridesOtherAnimations)
				{
					controller.PlaySingleAnimation(animationClip);
				}
				else
				{
					PlayAnimation();
				}
			}

			if(landingParticlePool && controller.slots.physicsObject.GetSurfaceTag() != "Stairs")
			{
				SpawnLandingParticle();
			}

			while(controller.slots.actor.timeStop.isTimeStopped) yield return null;
			yield return new WaitForSeconds(duration);

			End();
		}

		protected void SpawnLandingParticle()
		{
			GameObject particle = landingParticlePool.Spawn();
			ParentHelper.Parent(particle, ParentHelper.ParentObject.Particles);
			particle.transform.position = new Vector3(landingParticlePool.transform.position.x + particleOffset.x, landingParticlePool.transform.position.y + particleOffset.y, 0.0f);
			particle.GetComponent<RexParticle>().Play();
		}
	}
}
