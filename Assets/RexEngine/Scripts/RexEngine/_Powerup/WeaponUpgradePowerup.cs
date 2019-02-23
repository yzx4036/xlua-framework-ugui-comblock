using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class WeaponUpgradePowerup:Powerup 
	{
		public string nameOfAttackToUpgrade = "";
		public Attack upgradeAttackPrefab;
		public Vector3 newAttackLocalPosition;
		public bool willUseContactDelayOnCollect;

		[HideInInspector]
		public Attack originalAttack; //The original attack on the actor that this replaces

		protected Attack newAttack;

		void Awake() 
		{
			idString = "WeaponUpgrade_" + upgradeAttackPrefab.name;
		}

		public override void RemoveEffect(RexActor actor)
		{
			bool willDestroy = false;
			if(originalAttack != null)
			{
				originalAttack.isEnabled = true;
				newAttack.isEnabled = false;
				willDestroy = true;
			}

			if(newAttack.enableType != Attack.EnableType.Permanent)
			{
				willDestroy = true;
			}

			base.RemoveEffect(actor);

			if(willDestroy && newAttack.gameObject != null)
			{
				Destroy(newAttack.gameObject);
			}
		}

		protected override void TriggerEffect(RexActor actor)
		{
			bool doesActorHaveAttackToUpgrade = false;
			Attack[] attacks = actor.GetComponentsInChildren<Attack>();
			for(int i = 0; i < attacks.Length; i ++)
			{
				string attackName = attacks[i].name.Split('(')[0];
				if(nameOfAttackToUpgrade == attackName)
				{
					doesActorHaveAttackToUpgrade = true;
					originalAttack = attacks[i];
				}
			}

			if(doesActorHaveAttackToUpgrade || nameOfAttackToUpgrade == "")
			{
				GameObject newAttackGameObject = Instantiate(upgradeAttackPrefab).gameObject;
				newAttack = newAttackGameObject.GetComponent<Attack>();

				if(originalAttack)
				{
					originalAttack.isEnabled = false;
				}

				newAttack.isEnabled = true;
				newAttack.slots.actor = actor;

				Transform transformToParentTo = actor.transform;
				if(actor.transform.Find("Attacks"))
				{
					transformToParentTo = actor.transform.Find("Attacks");
				}

				newAttack.transform.parent = transformToParentTo;
				newAttack.transform.localPosition = newAttackLocalPosition;

				if(newAttack.enableType == Attack.EnableType.Unique)
				for(int i = 0; i < attacks.Length; i ++)
				{
					bool isReplacingUniqueAttack = (attacks[i].enableType == Attack.EnableType.Unique && attacks[i].input.button == newAttack.input.button);
					if(isReplacingUniqueAttack)
					{
						attacks[i].isEnabled = false;
						//Destroy(attacks[i].gameObject);
					}
				}
			}

			if(willUseContactDelayOnCollect)
			{
				StartCoroutine("ContactDelayCoroutine");
			}

		}

		protected IEnumerator ContactDelayCoroutine()
		{
			float addedTime = 1.0f;
			float startingTime = Time.realtimeSinceStartup;
			float currentTime = 0.0f;

			Time.timeScale = 0.0f;

			while(currentTime < addedTime)
			{
				if(!PauseManager.Instance.IsGamePaused())
				{
					currentTime = Time.realtimeSinceStartup - startingTime;
				}

				Time.timeScale = 0.0f;
				yield return new WaitForEndOfFrame();
			}

			Time.timeScale = 1.0f;
		}
	}
}
