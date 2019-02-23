using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DestroyEnemiesPowerup:Powerup 
	{
		void Awake() 
		{
			idString = "DestroyEnemies";
		}

		protected override void TriggerEffect(RexActor actor)
		{
			actor.KillImmediately();
		}

		protected override void AnimateIn()
		{
			ScreenFlash.Instance.Flash();
		}
	}

}