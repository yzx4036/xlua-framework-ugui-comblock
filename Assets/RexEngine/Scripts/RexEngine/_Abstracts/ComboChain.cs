using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ComboChain:AttackBase 
	{
		public List<Attack> attacks;
		public int framesBeforeTimeout = 64;

		protected Attack rootAttack;
		protected int currentAttack;

		void Start() 
		{
			rootAttack = attacks[0];

			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].comboChain = this;
				attacks[i].isEnabled = (i > 0) ? false : true;
				if(!isEnabled)
				{
					attacks[i].isEnabled = false;
				}
			}
		}

		public void NotifyOfAttackBegin(Attack attack)
		{
			if(isEnabled)
			{
				StopCoroutine("ComboCoroutine");
				StartCoroutine("ComboCoroutine");

				StopCoroutine("ComboTimeoutCoroutine");
				StartCoroutine("ComboTimeoutCoroutine");
			}
		}

		public void SetEnabled(bool _isEnabled)
		{
			isEnabled = _isEnabled;
			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].isEnabled = (_isEnabled && i == 0) ? true : false;
			}
		}

		public override void OnEnabled()
		{
			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].OnEnabled();
			}
		}

		public override void OnDisabled()
		{
			for(int i = 0; i < attacks.Count; i ++)
			{
				attacks[i].OnDisabled();
			}
		}

		protected IEnumerator ComboCoroutine()
		{
			yield return new WaitForFixedUpdate();

			while(!attacks[currentAttack].GetIsCooldownComplete())
			{
				while(attacks[currentAttack].slots.actor.timeStop.isTimeStopped) yield return null;
				yield return new WaitForFixedUpdate();
			}

			attacks[currentAttack].isEnabled = false;

			currentAttack ++;
			if(currentAttack >= attacks.Count)
			{
				currentAttack = 0;
			}

			attacks[currentAttack].isEnabled = true;
		}

		protected IEnumerator ComboTimeoutCoroutine()
		{
			int currentTimeoutFrame = framesBeforeTimeout;

			while(currentTimeoutFrame > 0)
			{
				currentTimeoutFrame --;

				while(attacks[currentAttack].slots.actor.timeStop.isTimeStopped) yield return null;
				yield return new WaitForFixedUpdate();
			}

			attacks[currentAttack].isEnabled = false;

			currentAttack = 0;

			if(isEnabled)
			{
				rootAttack.isEnabled = true;
			}
		}
	}
}
