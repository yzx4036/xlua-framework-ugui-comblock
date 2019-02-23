/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DemoEnemy:Enemy 
	{
		public bool willLaunchOnDeath = true;

		protected float deathVelocity = 15.0f;

		void FixedUpdate()
		{
			if(isDead)
			{
				if(slots.spriteHolder && slots.physicsObject.isEnabled)
				{
					slots.physicsObject.SetVelocityX(deathVelocity);
					slots.spriteHolder.transform.localEulerAngles = new Vector3(0.0f, 0.0f, slots.spriteHolder.transform.localEulerAngles.z + 5.0f);
				}
			}
		}

		public override void OnSpawned()
		{
			slots.physicsObject.AddToCollisions("Terrain");
		}

		protected override void OnHit(int damageTaken, Collider2D col = null)
		{
			if(isDead)
			{
				if(gameObject.name != "Ankylosaur")
				{
					slots.physicsObject.RemoveFromCollisions("Terrain");
					slots.physicsObject.isEnabled = true;
					if(willLaunchOnDeath && col.transform.position.x > transform.position.x)
					{
						deathVelocity *= -1.0f;
					}

					slots.physicsObject.gravitySettings.usesGravity = true;
					slots.physicsObject.SetVelocityX(deathVelocity);
					slots.physicsObject.ApplyForce(new Vector2(0.0f, 30.0f));
				}
			}
		}
	}
}
