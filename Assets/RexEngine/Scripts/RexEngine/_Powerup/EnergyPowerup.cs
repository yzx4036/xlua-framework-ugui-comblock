/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class EnergyPowerup:Powerup
	{
		public enum EnergyType
		{
			HP,
			Weapon,
			Lives
		}

		public EnergyType energyType;
		public Equation equation;
		public int amount;

		protected override void TriggerEffect(RexActor player)
		{
			if(player == null)
			{
				return;
			}

			if(energyType == EnergyType.HP)
			{
				if(equation == Equation.Increment)
				{
					player.RestoreHP(amount);
				}
				else
				{
					player.Damage(amount);
				}
			}
			else if(energyType == EnergyType.Weapon)
			{
				if(equation == Equation.Increment)
				{
					player.RestoreMP(amount);
				}
				else
				{
					player.DecrementMP(amount);
				}
			}
			else if(energyType == EnergyType.Lives)
			{
				if(equation == Equation.Increment)
				{
					LivesManager.Instance.IncrementLives(amount);
				}
				else
				{
					LivesManager.Instance.DecrementLives(amount);
				}
			}
		}
	}
}
