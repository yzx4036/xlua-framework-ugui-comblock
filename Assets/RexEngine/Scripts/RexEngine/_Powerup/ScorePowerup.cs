/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ScorePowerup:Powerup 
	{
		public Equation equation;
		public int amount;

		protected override void TriggerEffect(RexActor player)
		{
			if(player == null)
			{
				return;
			}

			if(equation == Equation.Increment)
			{
				//Debug.Log("Add " + amount + " to score");
				ScoreManager.Instance.IncrementScore(amount);
			}
			else
			{
				//Debug.Log("Remove " + amount + " from score");
				ScoreManager.Instance.DecrementScore(amount);
			}
		}
	}
}
