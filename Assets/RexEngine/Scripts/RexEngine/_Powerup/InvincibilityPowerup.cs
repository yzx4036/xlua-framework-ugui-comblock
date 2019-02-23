using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class InvincibilityPowerup:TemporaryPowerup 
	{
		public RexPool spawnPool;

		void Awake()
		{
			idString = "Invincibility";
		}

		public override void RemoveEffect(RexActor actor)
		{
			actor.invincibility.isPowerupInvincibilityActive = false;
			actor.ToggleCollider();

			base.RemoveEffect(actor);
		}

		protected override void TriggerEffect(RexActor actor)
		{
			actor.invincibility.isPowerupInvincibilityActive = true;
		}

		protected override void AnimateInForActor(RexActor actor)
		{
			if(spawnPool)
			{
				ParticleSystem particle = spawnPool.Spawn().GetComponent<ParticleSystem>();
				if(particle)
				{
					particle.transform.parent = actor.transform;
					particle.transform.localPosition = Vector2.zero;
					DontDestroyOnLoad(particle.gameObject);
					particle.name = "InvincibilityParticle";
					particle.Play();
				}
			}
		} 

		protected override void AnimateOutForActor(RexActor actor)
		{
			GameObject particle = actor.transform.Find("InvincibilityParticle").gameObject;
			if(particle != null)
			{
				Destroy(particle);
			}
		}
	}
}
