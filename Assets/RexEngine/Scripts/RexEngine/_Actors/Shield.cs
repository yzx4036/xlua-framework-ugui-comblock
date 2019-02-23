using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class Shield:MonoBehaviour 
	{
		[Tooltip("If True, no amount of damage will break the shield, and it will not be removed unless manually destroyed via a script.")]
		public bool hasUnlimitedHP = false;

		[Tooltip("The amount of damage the shield can sustain before being destroyed.")]
		public int hp = 1;

		[HideInInspector]
		public RexActor parentActor;

		public virtual void OnParentDamaged(int amount, int unadjustedAmount, int currentHP, int totalHP)
		{
			if(!hasUnlimitedHP)
			{
				hp -= unadjustedAmount;
				if(hp <= 0)
				{
					Destroy(gameObject);
				}
			}
		}

		void OnDestroy()
		{
			if(parentActor)
			{
				parentActor.OnDamageTaken -= this.OnParentDamaged;
				parentActor.invincibility.isShieldInvincibilityActive = false;
			}
		}
	}
}
