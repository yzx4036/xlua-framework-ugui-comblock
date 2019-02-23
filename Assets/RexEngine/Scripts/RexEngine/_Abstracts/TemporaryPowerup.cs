using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class TemporaryPowerup:Powerup 
	{
		public float durationBeforeRemoval = 5.0f;

		protected override void OnCollisionProcessed()
		{
			StartCoroutine("RemoveEffectCoroutine");
		}

		protected IEnumerator RemoveEffectCoroutine()
		{
			yield return new WaitForSeconds(durationBeforeRemoval);

			if(durationBeforeRemoval != 0.0f) //Powerup will last indefinitely if durationBeforeRemoval is 0.0f
			{
				RemoveFromAllAffected();
			}
		}
	}
}
