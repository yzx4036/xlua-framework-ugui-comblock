using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class AttackSet:MonoBehaviour 
	{
		public bool isEnabled = true;
		public Slots slots;
		public List<AttackBase> attacks;
		public AttackBase defaultAttack;
		[Tooltip("Whether or not you can toggle between attacks using the Misc_1 and Misc_2 buttons.")]
		public bool allowFastToggle;

		[System.Serializable]
		public class Slots
		{
			public RexActor actor;
		}

		protected int currentAttack = 0;

		void Start()
		{
			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].isEnabled = (attacks[i] == defaultAttack && isEnabled);
				if(attacks[i] == defaultAttack)
				{
					currentAttack = i;
				}

				if(!attacks[i].isEnabled)
				{
					attacks[i].Disable();
				}
			}
		}

		void Update()
		{
			if(slots.actor && isEnabled && allowFastToggle && !PauseManager.Instance.IsGamePaused())
			{
				if(slots.actor.slots.input.isMisc_1ButtonDownThisFrame)
				{
					ToggleLeft();
				}
				else if(slots.actor.slots.input.isMisc_2ButtonDownThisFrame)
				{
					ToggleRight();
				}
			}
		}

		public void EnableSpecificAttack(int attackIndex)
		{
			currentAttack = attackIndex;
			EnableSingleAttack();
		}

		public void ToggleRight()
		{
			if(!isEnabled)
			{
				return;
			}

			currentAttack ++;
			if(currentAttack >= attacks.Count)
			{
				currentAttack = 0;
			}

			EnableSingleAttack();
		}

		public void ToggleLeft()
		{
			if(!isEnabled)
			{
				return;
			}

			currentAttack --;
			if(currentAttack < 0)
			{
				currentAttack = attacks.Count - 1;
			}

			EnableSingleAttack();
		}

		public void Enable()
		{
			isEnabled = true;
			defaultAttack.Enable();
		}

		public void Disable()
		{
			isEnabled = false;
			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].Disable();
			}
		}

		protected void EnableSingleAttack()
		{
			for(int i = 0; i < attacks.Count; i ++)
			{
				if(attacks[i].isEnabled)
				{
					attacks[i].Disable();
				}
			}

			attacks[currentAttack].Enable();
		}
	}
}
