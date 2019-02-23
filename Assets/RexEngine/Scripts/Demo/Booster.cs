/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class Booster:RexActor 
{
	public Attacks attacks;

	[System.Serializable]
	public class Attacks
	{
		public AttackSet subweaponAttackSet;
		public Attack flyingPeaShooterAttack;
		public ComboChain meleeAttack;
	}

	public AnimationClip victoryAnimation;
	public RexController flyingController;
	public RexPool growWingsPool;
	public AudioClip growWingsSound;

	void Start() 
	{
		DontDestroyOnLoad(gameObject);
	}

	public void OnBlueprintFound()
	{
		slots.controller.SetAxis(new Vector2(0, 0));
		slots.physicsObject.SetVelocityX(0.0f);
		slots.controller.isEnabled = false;
		slots.physicsObject.gravitySettings.usesGravity = true;
		slots.anim.Play(victoryAnimation.name, 0, 0);
	}

	public void SetToFlyingController()
	{
		slots.physicsObject.StopAllMovement();
		SetController(flyingController);
		GameObject growWingsParticle = growWingsPool.Spawn();
		growWingsParticle.transform.localPosition = new Vector3(0, 0, 0);
		growWingsParticle.GetComponent<RexParticle>().Play();
	}

	public void SetToRegularController()
	{
		SetController(waterProperties.landController);
	}

	public override void Reset()
	{
		SetToRegularController();

		if(!DataManager.Instance.hasUnlockedBounce)
		{
			if(slots.controller.GetComponent<BounceState>())
			{
				slots.controller.GetComponent<BounceState>().isEnabled = false;
			}
		}

		if(!DataManager.Instance.hasUnlockedProjectile)
		{
			if(attacks.subweaponAttackSet != null)
			{
				attacks.subweaponAttackSet.Disable();
			}
		}
		else
		{
			if(attacks.subweaponAttackSet != null)
			{
				attacks.subweaponAttackSet.Enable();
			}
		}

		if(!DataManager.Instance.hasUnlockedDoubleJump)
		{
			slots.controller.GetComponent<JumpState>().multipleJumpNumber = 1;
		}

		if(!DataManager.Instance.hasUnlockedWallCling)
		{
			if(slots.controller.GetComponent<WallClingState>())
			{
				slots.controller.GetComponent<WallClingState>().isEnabled = false;
			}
		}

		if(!DataManager.Instance.hasUnlockedFly)
		{
			slots.physicsObject.gravitySettings.usesGravity = true;

			if(attacks.meleeAttack)
			{
				attacks.meleeAttack.SetEnabled(true);
			}

			if(attacks.flyingPeaShooterAttack)
			{
				attacks.flyingPeaShooterAttack.Disable();
			}
		}
	}

	protected override void OnControllerChanged(RexController _newController, RexController previousController)
	{
		if(_newController == waterProperties.landController)
		{
			if(attacks.meleeAttack)
			{
				attacks.meleeAttack.SetEnabled(true);
			}

			if(previousController == waterProperties.waterController)
			{
				attacks.subweaponAttackSet.Enable();
			}

			GetComponent<BoxCollider2D>().size = new Vector2(1.0f, 1.83f);
		}
		else if(_newController == waterProperties.waterController)
		{
			attacks.meleeAttack.SetEnabled(false);
			attacks.subweaponAttackSet.Disable();
			GetComponent<BoxCollider2D>().size = new Vector2(2.3f, 1.83f);
		}
		else if(_newController == flyingController)
		{
			GetComponent<BoxCollider2D>().size = new Vector2(2.3f, 1.25f);
			attacks.flyingPeaShooterAttack.Enable();
			attacks.subweaponAttackSet.Disable();
			attacks.meleeAttack.SetEnabled(false);
			PlaySoundIfOnCamera(growWingsSound);
		}
	}
}
