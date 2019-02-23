/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class NewAbilityButton:RexActor 
	{
		public enum MechanicType
		{
			Bounce,
			Projectile,
			DoubleJump,
			Flying,
			WallCling
		}

		public MechanicType mechanicType;
		public Sprite pressedSprite;
		public AudioSource audioSource;
		public AudioClip pressSound;

		protected bool hasActivated;
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
				case MechanicType.Bounce:
					AddBounce();
					break;
				case MechanicType.Projectile:
					AddProjectileAttack();
					break;
				case MechanicType.DoubleJump:
					AddDoubleJump();
					break;
				case MechanicType.Flying:
					AddFlying();
					break;
				case MechanicType.WallCling:
					AddWallCling();
					break;
				default:
					AddBounce();
					break;
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
			player.GetComponent<Booster>().attacks.subweaponAttackSet.Enable();
		}

		protected void AddWallCling()
		{
			DataManager.Instance.hasUnlockedWallCling = true;
			player.slots.controller.GetComponent<WallClingState>().isEnabled = true;
		}

		protected void AddFlying()
		{
			DataManager.Instance.hasUnlockedFly = true;
			StartCoroutine("AddFlyingCoroutine");
		}

		//This is to let the player finish out their bounce before gravity is disabled; it just feels good
		protected IEnumerator AddFlyingCoroutine()
		{
			yield return new WaitForSeconds(0.525f);

			player.GetComponent<Booster>().SetToFlyingController();
			player.slots.physicsObject.gravitySettings.usesGravity = false;
			ScreenShake.Instance.Shake();
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

				hasActivated = true;
				ScreenShake.Instance.Shake();
				AddMechanic();
				GetComponent<BoxCollider2D>().enabled = false;
				StartCoroutine("DisplayTextCoroutine");
				slots.spriteRenderer.sprite = pressedSprite;
			}
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
				case MechanicType.Bounce:
					description = "Rex Engine makes it easy to give yourself new powers with the press of a button! Now you can bounce on top of enemies! Bam!";
					break;
				case MechanicType.Projectile:
					description = "Now you can fire bullets by pressing 'T'! You can also fire charged shots by holding T for several seconds before releasing it!";
					break;
				case MechanicType.DoubleJump:
					description = "Now you can double-jump by pressing the Jump key again in midair!";
					break;
				case MechanicType.Flying:
					description = "A double-jump isn't enough to get up this shaft, but with Rex Engine, we can make ourselves fly! Now you can move up and down in midair!";
					break;
				case MechanicType.WallCling:
					description = "Now you can cling to walls and wall-jump! There are even options to climb up and down walls and hang from ledges!";
					break;
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
