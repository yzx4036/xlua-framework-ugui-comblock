using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ChargeAttack:AttackBase 
	{
		public Attack initialAttack;
		public Energy chargeDisplayEnergy;

		protected int currentAttack;

		[HideInInspector]
		public float currentChargeTime = 0.0f;

		public List<AttacksWithTiming> chargedAttacks = new List<AttacksWithTiming>();

		[System.Serializable]
		public class AttacksWithTiming
		{
			public Attack attack;
			public float chargeTime;
		}

		public Slots slots;

		[System.Serializable]
		public class Slots
		{
			public RexActor actor;
			public SpriteRenderer spriteRenderer;
		}

		public Attack.AttackImportance button;
		public SpriteRenderer spriteToFlash;

		protected float totalChargeTime;

		void Start() 
		{
			if(initialAttack != null)
			{
				initialAttack.isEnabled = false;
			}

			for(int i = 0; i < chargedAttacks.Count; i ++)
			{
				chargedAttacks[i].attack.isEnabled = false;
				if(chargedAttacks[i].chargeTime > totalChargeTime)
				{
					totalChargeTime = chargedAttacks[i].chargeTime;
				}
			}
		}

		void Update() 
		{
			//Check to see if we're attempting to Begin this attack
			if(Time.timeScale > 0.0f && slots.actor && isEnabled)
			{
				if(slots.actor.slots.input)
				{
					if(IsAttackAttempted())
					{
						if(initialAttack != null && currentChargeTime == 0.0f)
						{
							initialAttack.isEnabled = true;
							initialAttack.Begin();
							initialAttack.isEnabled = false;
						}

						currentChargeTime += Time.deltaTime;
					}
					else if(currentChargeTime > 0.0f)
					{
						for(int i = chargedAttacks.Count - 1; i >= 0; i --)
						{
							if(currentChargeTime >= chargedAttacks[i].chargeTime && (slots.actor.mpProperties.mp == null || slots.actor.mpProperties.mp.current >= chargedAttacks[i].attack.energyCost))
							{
								currentAttack = i;

								Attack attack = chargedAttacks[i].attack;
								attack.isEnabled = true;
								attack.Begin();
								attack.isEnabled = false;

								currentChargeTime = 0.0f;
								break;
							}
						}

						currentChargeTime = 0.0f;
					}
					else
					{
						currentChargeTime = 0.0f;
					}

					SetEnergyBar();
					FlashSprite();
				}
			}
		}

		public override void OnEnabled()
		{
			if(chargeDisplayEnergy && chargedAttacks[0].attack.ui.onlyShowWhenEnabled)
			{
				chargeDisplayEnergy.bar.gameObject.SetActive(false);
			}

			if(initialAttack)
			{
				initialAttack.OnEnabled();
			}

			for(int i = 0; i < chargedAttacks.Count; i ++)
			{
				chargedAttacks[i].attack.OnEnabled();
			}
		}

		public override void OnDisabled()
		{
			if(chargeDisplayEnergy && chargedAttacks[0].attack.ui.onlyShowWhenEnabled)
			{
				chargeDisplayEnergy.bar.gameObject.SetActive(false);
			}

			if(spriteToFlash)
			{
				spriteToFlash.color = new Color(1.0f, 1.0f, 1.0f);
				currentChargeTime = 0.0f;
			}

			if(initialAttack)
			{
				initialAttack.OnDisabled();
			}

			for(int i = 0; i < chargedAttacks.Count; i ++)
			{
				chargedAttacks[i].attack.OnDisabled();
			}
		}

		protected bool IsAttackAttempted()
		{
			bool isAttackAttempted = false;

			if(slots.actor.slots.input)
			{
				if((button == Attack.AttackImportance.Primary || button == Attack.AttackImportance.All) && slots.actor.slots.input.isAttackButtonDown)
				{
					isAttackAttempted = true;
				}
				else if((button == Attack.AttackImportance.Sub || button == Attack.AttackImportance.All) && slots.actor.slots.input.isSubAttackButtonDown)
				{
					isAttackAttempted = true;
				}
				else if((button == Attack.AttackImportance.Sub_2 || button == Attack.AttackImportance.All) && slots.actor.slots.input.isSubAttack_2ButtonDown)
				{
					isAttackAttempted = true;
				}
				else if((button == Attack.AttackImportance.Sub_3 || button == Attack.AttackImportance.All) && slots.actor.slots.input.isSubAttack_3ButtonDown)
				{
					isAttackAttempted = true;
				}
			}

			return isAttackAttempted;
		}

		protected void SetEnergyBar()
		{
			if(chargeDisplayEnergy != null)
			{
				float percentageFull = (float)(currentChargeTime / totalChargeTime);
				percentageFull = Mathf.Clamp01(percentageFull);
				float energyValue = percentageFull * chargeDisplayEnergy.max;
				chargeDisplayEnergy.SetValue((int)energyValue);
			}
		}

		protected void FlashSprite()
		{
			if(!spriteToFlash)
			{
				return;
			}

			if(currentChargeTime > 0.0f)
			{
				float flashDuration = 0.2f;
				float remainder = currentChargeTime % flashDuration;
				if(remainder > flashDuration / 2.0f)
				{
					spriteToFlash.color = new Color(0.5f, 0.5f, 0.5f);
				}
				else
				{
					spriteToFlash.color = new Color(1.0f, 1.0f, 1.0f);
				}
			}
			else
			{
				spriteToFlash.color = new Color(1.0f, 1.0f, 1.0f);
			}
		}
	}
}
