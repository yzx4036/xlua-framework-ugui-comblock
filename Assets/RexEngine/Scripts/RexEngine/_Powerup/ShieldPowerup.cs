using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ShieldPowerup:Powerup
	{
		public RexPool spawnPool;

		void Awake()
		{
			idString = "Shield";
		}

		protected override void TriggerEffect(RexActor actor)
		{
			if(actor == null)
			{
				return;
			}

			if(!actor.invincibility.isShieldInvincibilityActive && spawnPool)
			{
				Shield shield = spawnPool.Spawn().GetComponent<Shield>();
				if(shield)
				{
					shield.transform.parent = actor.transform;
					shield.transform.localPosition = Vector2.zero;
					DontDestroyOnLoad(shield.gameObject);
					shield.parentActor = actor;

					actor.OnDamageTaken += shield.OnParentDamaged;
				}
			}

			actor.invincibility.isShieldInvincibilityActive = true;
		}
	}
}
