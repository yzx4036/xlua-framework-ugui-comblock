/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class AbilityToggleButton:RexActor 
	{
		public enum MechanicType
		{
			Weapon,
			FixedJump,
			Acceleration
		}

		public enum WeaponType
		{
			Melee,
			Projectile
		}

		public MechanicType mechanicType;
		public Sprite pressedSprite;
		public Sprite originalSprite;
		public AudioSource audioSource;
		public AudioClip pressSound;

		protected bool hasActivated;
		protected WeaponType currentWeaponType;
		protected bool isJumpFixed;
		protected bool isAccelerationEnabled;
		protected RexActor player;

		void Awake() 
		{

		}

		void Start()
		{
			player = GameManager.Instance.player;
		}

		protected void AddMechanic()
		{
			Debug.Log("Adding: " + mechanicType);
			switch(mechanicType)
			{
				case MechanicType.Weapon:
					ToggleWeapon();
					break;
				case MechanicType.FixedJump:
					ToggleFixedJump();
					break;
				case MechanicType.Acceleration:
					ToggleAcceleration();
					break;
				default:
					AddBounce();
					break;
			}
		}

		protected void ToggleWeapon()
		{
			if(currentWeaponType == WeaponType.Melee)
			{
				currentWeaponType = WeaponType.Projectile;
				player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<Attack>().isEnabled = true;
				player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<ChargeAttack>().button = Attack.AttackImportance.Primary;
				player.transform.Find("Attacks").Find("Melee").GetComponent<Attack>().isEnabled = false;
			}
			else if(currentWeaponType == WeaponType.Projectile)
			{
				currentWeaponType = WeaponType.Melee;
				player.transform.Find("Attacks").Find("Melee").GetComponent<Attack>().isEnabled = true;
				player.transform.Find("Attacks").Find("Melee").GetComponent<Attack>().input.button = Attack.AttackImportance.Primary;
				player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<ChargeAttack>().isEnabled = false;
			}
		}

		protected void ToggleFixedJump()
		{
			if(!isJumpFixed)
			{
				isJumpFixed = true;
				player.slots.controller.GetComponent<JumpState>().freezeHorizontalMovement = true;
			}
			else if(isJumpFixed)
			{
				isJumpFixed = false;
				player.slots.controller.GetComponent<JumpState>().freezeHorizontalMovement = false;
			}
		}

		protected void ToggleAcceleration()
		{
			if(!isAccelerationEnabled)
			{
				isAccelerationEnabled = true;
				player.slots.controller.GetComponent<MovingState>().movementProperties.acceleration = 0.25f;
				player.slots.controller.GetComponent<MovingState>().movementProperties.deceleration = 0.25f;
			}
			else if(isAccelerationEnabled)
			{
				isAccelerationEnabled = false;
				player.slots.controller.GetComponent<MovingState>().movementProperties.acceleration = 0.0f;
				player.slots.controller.GetComponent<MovingState>().movementProperties.deceleration = 0.0f;
			}
		}

		protected void AddDoubleJump()
		{
			DataManager.Instance.hasUnlockedDoubleJump = true;
			player.slots.controller.GetComponent<JumpState>().multipleJumpNumber = 2;
		}

		protected void AddProjectileAttack()
		{
			DataManager.Instance.hasUnlockedProjectile = true;
			player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<ChargeAttack>().isEnabled = true;
		}

		protected void AddFlying()
		{
			DataManager.Instance.hasUnlockedFly = true;
			StartCoroutine("AddFlyingCoroutine");
		}

		//This is to let the player finish out their bounce before gravity is disabled; it just feels good
		protected IEnumerator AddFlyingCoroutine()
		{
			player.slots.controller.GetComponent<JumpState>().type = JumpState.JumpType.None;
			player.slots.controller.GetComponent<MovingState>().canMoveVertically = true;

			player.transform.Find("Attacks").Find("Melee").GetComponent<Attack>().isEnabled = false;
			player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<ChargeAttack>().isEnabled = true;
			player.transform.Find("Attacks").Find("PeaShooter_Charge").GetComponent<ChargeAttack>().button = Attack.AttackImportance.Primary;

			yield return new WaitForSeconds(0.35f);

			player.slots.physicsObject.gravitySettings.usesGravity = false;
			player.slots.controller.GetComponent<BounceState>().isEnabled = false;
		}

		protected void AddBounce()
		{
			DataManager.Instance.hasUnlockedBounce = true;
			player.slots.controller.GetComponent<BounceState>().isEnabled = true;
		}

		public override void OnBouncedOn(Collider2D col = null)
		{
			if(!hasActivated)
			{
				if(audioSource && pressSound)
				{
					audioSource.PlayOneShot(pressSound);
				}

				ScreenShake.Instance.Shake();
				AddMechanic();
				StartCoroutine("DisplayTextCoroutine");
				StartCoroutine("BounceCoroutine");
			}
		}

		protected IEnumerator BounceCoroutine()
		{
			slots.spriteRenderer.sprite = pressedSprite;
			hasActivated = true;
			GetComponent<BoxCollider2D>().enabled = false;
			yield return new WaitForSeconds(2.5f);

			hasActivated = false;
			GetComponent<BoxCollider2D>().enabled = true;
			slots.spriteRenderer.sprite = originalSprite;
		}

		protected IEnumerator DisplayTextCoroutine()
		{
			yield return new WaitForSeconds(0.5f);

			List<DialogueManager.Page> pages = new List<DialogueManager.Page>();
			DialogueManager.Page page = new DialogueManager.Page();
			page.text = GetAbilityDescription();
			pages.Add(page);

			DialogueManager.Instance.Show(pages);
		}

		protected string GetAbilityDescription()
		{
			string description = "";
			switch(mechanicType)
			{
				case MechanicType.Weapon:
					string weaponName = (currentWeaponType == WeaponType.Melee) ? "Wrench Slash" : "Pistol";
					description = "Weapon changed to: " + weaponName + "!";
					break;
				case MechanicType.FixedJump:
					description = (isJumpFixed) ? "Jump is now horizontally fixed!" : "Jump is no longer horizontally fixed!";
					break;
				case MechanicType.Acceleration:
					description = (isAccelerationEnabled) ? "Movement now has acceleration and deceleration!" : "Movement no longer has acceleration or \ndeceleration!";
					break;
				/*case MechanicType.Flying:
					description = "A double-jump isn't enough to get up \nthis shaft, but with RexEngine, we can \nmake ourselves fly! Now you can \nmove up and down in midair!";
					break;*/
				default:
					description = "Rex Engine makes it easy to give yourself new powers with the press of a button! Now you can bounce on top of enemies! Bam!";
					break;
			}

			return description;
		}

		protected new void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player")
			{
				player.slots.controller.GetComponent<BounceState>().isEnabled = true;
			}
		}
	}
}
