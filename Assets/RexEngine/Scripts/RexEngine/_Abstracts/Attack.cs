/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class Attack:AttackBase 
	{
		[System.Serializable]
		public class AttackAnimations
		{
			public AnimationClip standing;
			public AnimationClip moving;
			public AnimationClip dashing;
			public AnimationClip jumping;
			public AnimationClip climbing;
			public AnimationClip crouching;
			public AnimationClip wallClinging;
			public AnimationClip stairClimbing;
			public AnimationClip groundPounding;
			public AnimationClip flutterJumping;
			public AnimationClip gliding;
		}

		//If any of these are True, transitioning into that event will cancel this attack
		[System.Serializable]
		public class CanceledBy
		{
			public bool onMove;
			public bool onJump;
			public bool onTurn;
			public bool onKnockback;
			public bool onDash;
			public bool onCrouch;
			public bool onWallCling;
			public bool onStairClimb;
			public bool onGroundPound;
			public bool onFlutterJump;
			public bool onGlide;
		}

		//What additional actions are canceled if you begin this attack during them?
		[System.Serializable]
		public class Cancels
		{
			public bool dash;
			public bool crouch;
			public bool wallClinging;
			public bool groundPound;
			public bool flutterJump;
			public bool glide;
		}

		[System.Serializable]
		public class ProjectileProperties
		{
			public RexPool rexPool; //The RexPool that spawns the projectile
			public int limitInstances; //The max number of this projectile you can have active at once
			public bool willAutoCreateProjectile = true; //If a projectile is slotted, setting this to "true" means it will be auto-spawned as soon as the attack begins. Otherwise, it will need to be spawned manually.
			public bool isAimable = false; //Whether or not 8-way aiming with the d-pad is enabled
		}

		[System.Serializable]
		public class Combo
		{
			public Attack nextAttack;
			public bool isFirstInChain;
		}

		//Whether the attack is triggered with the primary or secondary attack button
		public enum AttackImportance
		{
			Primary,
			Sub,
			Sub_2,
			Sub_3,
			All
		}

		public enum AttackDirection
		{
			Ahead,
			Behind
		}

		public enum EnableType
		{
			Permanent,
			UntilDeath,
			Unique
		}

		//If any of these are True, you can do them while the attack is active
		[System.Serializable]
		public class ActionsAllowedDuringAttack
		{
			public bool groundMoving;
			public bool airMoving;
			public bool jumping;
			public bool turning;
			public bool attacking;
			public bool dashing;
			public bool climbing;
			public bool crouching;
			public bool wallClinging;
			public bool stairClimbing;
			public bool groundPounding;
			public bool flutterJumping;
			public bool gliding;
		}

		//What other states can you use this attack from? 
		[System.Serializable]
		public class CanInitiateFrom
		{
			public bool standing;
			public bool moving;
			public bool jumping;
			public bool dashing;
			public bool climbing;
			public bool crouching;
			public bool wallClinging;
			public bool stairClimbing;
			public bool groundPounding;
			public bool flutterJumping;
			public bool gliding;
		}

		[System.Serializable]
		public class Slots
		{
			public RexActor actor;
			public Animator animator;
			public AudioSource audio;
			public SpriteRenderer spriteRenderer;
			public BoxCollider2D boxCollider;
			public Energy mp;
		}

		[System.Serializable]
		public class AttackUI
		{
			public GameObject uiPrefab;
			public bool onlyShowWhenEnabled = true;
		}

		[System.Serializable]
		public class AttackInput
		{
			public AttackImportance button; //if an Input is attached, this governs whether the attach is called on the Attack or the SubAttack button
			public bool requireVerticalPress = false;
			public Direction.Vertical verticalPressRequired = Direction.Vertical.Neutral;
			public bool requireFreshButtonPress = true; //If True, the attack will only initiate  when the attack button is first pressed; if false, you can hold the button to continuously repeat the attack
		}

		public Slots slots;
		public AttackUI ui;
		public ProjectileProperties projectile;
		public int energyCost = 0;
		public AttackAnimations actorAnimations;
		public AttackAnimations attackAnimations;
		public AudioClip audioClip;
		public ActionsAllowedDuringAttack actionsAllowedDuringAttack;
		public CanceledBy canceledBy;
		public Cancels cancels;
		public CanInitiateFrom canInitiateFrom;
		public AttackInput input;
		public float cooldownTime = 0.2f; //The duration from when you perform the attack to when you're allowed to perform it again
		public bool willAutoEnableCollider = true; //If False, you need to manually enable the collider for this attack when it is executed
		public bool willSyncMoveAnimation = true;
		public float crouchOffset = 0.0f;
		public EnableType enableType;

		[HideInInspector]
		public ComboChain comboChain;

		[HideInInspector]
		public AttackDirection attackDirection;

		protected float currentCooldownTime;
		protected bool isCooldownComplete = true;
		protected GameObject uiGameObject;

		protected Vector3 nonCrouchingPosition;

		public enum ActionType
		{
			GroundMoving,
			AirMoving,
			Jumping,
			Turning,
			Dashing,
			Climbing,
			Crouching,
			WallClinging,
			StairClimbing,
			GroundPounding,
			FlutterJumping,
			Gliding
		}

		[HideInInspector]
		public bool isAttackActive;

		void Awake()
		{
			Reset();
		}

		void Start()
		{
			nonCrouchingPosition = transform.localPosition;
			if(slots.mp == null)
			{
				slots.mp = slots.actor.mpProperties.mp;
			}

			if(slots.mp && slots.mp != slots.actor.mpProperties.mp && ui.onlyShowWhenEnabled && !isEnabled)
			{
				slots.mp.gameObject.SetActive(false); 
			}

			if(ui.uiPrefab != null)
			{
				uiGameObject = (GameObject)Instantiate(ui.uiPrefab);
				ParentHelper.Parent(uiGameObject, ParentHelper.ParentObject.UI);

				if(ui.onlyShowWhenEnabled && !isEnabled)
				{
					uiGameObject.SetActive(false);
				}
			}
		}

		void Update()
		{
			//Check to see if we're attempting to Begin this attack
			if(Time.timeScale > 0.0f && slots.actor && isEnabled && !slots.actor.timeStop.isTimeStopped)
			{
				if(slots.actor.slots.input)
				{
					if(IsAttackAttempted())
					{
						Begin();
					}
				}
			}
		}

		void FixedUpdate()
		{
			CheckCooldown();

			if(Time.timeScale > 0.0f && slots.actor && !slots.actor.timeStop.isTimeStopped)
			{
				if(isAttackActive)
				{
					SetCrouchPosition();
				}
			}
		}

		#region public methods

		//Forces the attack to begin, even if we'd normally be prevented from initiating it. Use with caution.
		public void ForceBegin()
		{
			Reset();

			if(comboChain)
			{
				comboChain.NotifyOfAttackBegin(this);
			}

			if(slots.actor.slots.controller.StateID() == WallClingState.idString)
			{
				if(slots.actor.slots.controller.direction.horizontal == Direction.Horizontal.Right)
				{
					transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				}
				else
				{
					transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				}
			}

			SetCrouchPosition();

			if(slots.mp != null)
			{
				slots.mp.Decrement(energyCost);
			}

			StartCoroutine("AttackCoroutine");
		}

		//Called to start the attack after checking if we can initiate it first
		public void Begin()
		{
			if(CanInitiate())
			{
				ForceBegin();
			}
		}

		//Stops the attack
		public void Cancel()
		{
			Reset();
		}

		//Checks to see if we can Begin the attack or if something is stopping us
		public bool CanInitiate()
		{
			string stateID = slots.actor.slots.controller.StateID();
			bool canInitiate = true;
			if(!isCooldownComplete)
			{
				canInitiate = false;
			}
			else if(slots.mp != null && slots.mp.current < energyCost)
			{
				canInitiate = false;
			}
			else if(projectile.limitInstances > 0 && projectile.rexPool != null && projectile.rexPool.ActiveObjects() >= projectile.limitInstances)
			{
				canInitiate = false;
			}
			else if((isAttackActive || slots.actor.currentAttack != null) && !actionsAllowedDuringAttack.attacking)
			{
				canInitiate = false;
			}
			else if(!isEnabled)
			{
				canInitiate = false;
			}
			else if(slots.actor.slots.controller && (slots.actor.slots.controller.isKnockbackActive || slots.actor.slots.controller.isStunned || slots.actor.slots.controller.StateID() == DeathState.idString))
			{
				canInitiate = false;
			}
			else if(((stateID == DashState.idString && !canInitiateFrom.dashing) || (stateID == LadderState.idString && !canInitiateFrom.climbing) || (stateID == DefaultState.idString || stateID == LandingState.idString) && !canInitiateFrom.standing) || (stateID == MovingState.idString && !canInitiateFrom.moving) || ((stateID == JumpState.idString || stateID == FallingState.idString) && !canInitiateFrom.jumping) || (stateID == WallClingState.idString && !canInitiateFrom.wallClinging) || (stateID == StairClimbingState.idString && !canInitiateFrom.stairClimbing) || (stateID == GroundPoundState.idString && !canInitiateFrom.groundPounding) || (stateID == FlutterJumpState.idString && !canInitiateFrom.flutterJumping)|| (stateID == GlideState.idString && !canInitiateFrom.gliding))
			{
				canInitiate = false;
			}
			else if(stateID == CrouchState.idString && !canInitiateFrom.crouching)
			{
				canInitiate = false;
			}
			else if(slots.actor.slots.controller.isOverridingAnimationInProgress)
			{
				canInitiate = false;
			}

			return canInitiate;
		}

		//Checks to see if the attack is interruptable with other actions
		public bool CanInterrupt(ActionType _actionType)
		{
			bool canInterrupt = true;

			if((_actionType == ActionType.Dashing && !actionsAllowedDuringAttack.dashing) || (_actionType == ActionType.GroundMoving && !actionsAllowedDuringAttack.groundMoving) || (_actionType == ActionType.AirMoving && !actionsAllowedDuringAttack.airMoving) || (_actionType == ActionType.Jumping && !actionsAllowedDuringAttack.jumping) || (_actionType == ActionType.Turning && !actionsAllowedDuringAttack.turning) || (_actionType == ActionType.Climbing && !actionsAllowedDuringAttack.climbing) || (_actionType == ActionType.Crouching && !actionsAllowedDuringAttack.crouching) || (_actionType == ActionType.WallClinging && !actionsAllowedDuringAttack.wallClinging) || (_actionType == ActionType.StairClimbing && !actionsAllowedDuringAttack.stairClimbing) || (_actionType == ActionType.GroundPounding && !actionsAllowedDuringAttack.groundPounding) || (_actionType == ActionType.FlutterJumping && !actionsAllowedDuringAttack.flutterJumping) || (_actionType == ActionType.Gliding && !actionsAllowedDuringAttack.gliding))
			{
				canInterrupt = false;
			}

			return canInterrupt;
		}

		//If a projectile is slotted, this spawns it
		public virtual void CreateProjectile()
		{
			Direction.Horizontal direction = (slots.actor.slots.controller) ? slots.actor.slots.controller.direction.horizontal : Direction.Horizontal.Right;
			if(attackDirection == AttackDirection.Behind)
			{
				direction = (Direction.Horizontal)((int)direction * -1.0f);
			}

			Projectile newProjectile = projectile.rexPool.Spawn().GetComponent<Projectile>();
			Direction.Horizontal startingHorizontalDirection = direction;
			Direction.Vertical startingVerticalDirection = Direction.Vertical.Up;

			if(projectile.isAimable)
			{
				if(Mathf.Abs(slots.actor.slots.input.verticalAxis) == 1.0f && slots.actor.slots.input.horizontalAxis == 0.0f)
				{
					startingHorizontalDirection = Direction.Horizontal.Neutral;
				}

				startingVerticalDirection = (Direction.Vertical)(slots.actor.slots.input.verticalAxis * PhysicsManager.Instance.gravityScale);
			}

			newProjectile.isAimable = projectile.isAimable;
			newProjectile.Fire(new Vector2(slots.actor.transform.position.x + transform.localPosition.x * (int)direction, transform.position.y), startingHorizontalDirection, startingVerticalDirection, slots.actor, projectile.rexPool);
		}

		//This gets the animation clip that the actor using this attack should play based on this attack and the current State of the Controller
		public AnimationClip GetActorAnimationClip()
		{
			string stateID = slots.actor.slots.controller.StateID();
			AnimationClip animationClip = null;
			if(stateID == DefaultState.idString || stateID == LandingState.idString)
			{
				animationClip = actorAnimations.standing;
			}
			else if(stateID == MovingState.idString)
			{
				animationClip = actorAnimations.moving;
			}
			else if(stateID == JumpState.idString || stateID == FallingState.idString)
			{
				animationClip = actorAnimations.jumping;
			}
			else if(stateID == DashState.idString)
			{
				animationClip = actorAnimations.dashing;
			}
			else if(stateID == LadderState.idString)
			{
				animationClip = actorAnimations.climbing;
			}
			else if(stateID == CrouchState.idString)
			{
				animationClip = actorAnimations.crouching;
			}
			else if(stateID == WallClingState.idString)
			{
				animationClip = actorAnimations.wallClinging;
			}
			else if(stateID == StairClimbingState.idString)
			{
				animationClip = actorAnimations.stairClimbing;
			}
			else if(stateID == GroundPoundState.idString)
			{
				animationClip = actorAnimations.groundPounding;
			}
			else if(stateID == FlutterJumpState.idString)
			{
				animationClip = actorAnimations.flutterJumping;
			}
			else if(stateID == GlideState.idString)
			{
				animationClip = actorAnimations.gliding;
			}

			return animationClip;
		}

		public bool GetIsCooldownComplete()
		{
			return isCooldownComplete;
		}

		//Cancels and resets the attack properties
		public void Reset()
		{			
			StopCoroutine("AttackCoroutine");

			if(slots.boxCollider != null)
			{
				slots.boxCollider.enabled = false;
			}

			if(slots.spriteRenderer != null)
			{
				slots.spriteRenderer.enabled = false;
			}

			if(slots.actor != null && slots.actor.currentAttack == this)
			{
				slots.actor.currentAttack = null;
			}

			isAttackActive = false;
		}

		public override void OnEnabled()
		{
			if(slots.mp && slots.mp != slots.actor.mpProperties.mp && ui.onlyShowWhenEnabled)
			{
				slots.mp.bar.gameObject.SetActive(true); 
			}

			if(uiGameObject && ui.onlyShowWhenEnabled)
			{
				uiGameObject.SetActive(true);
			}
		}

		public override void OnDisabled()
		{
			if(slots.mp && slots.mp != slots.actor.mpProperties.mp && ui.onlyShowWhenEnabled)
			{
				slots.mp.bar.gameObject.SetActive(false); 
			}

			if(uiGameObject && ui.onlyShowWhenEnabled)
			{
				uiGameObject.SetActive(false);
			}
		}

		#endregion

		#region private methods

		protected void CheckCooldown()
		{
			if(!isCooldownComplete)
			{
				currentCooldownTime += Time.fixedDeltaTime;
				if(currentCooldownTime >= cooldownTime)
				{
					isCooldownComplete = true;
					currentCooldownTime = 0;
				}
			}
		}

		//This gets the animation clip that THIS ATTACK should play based on this attack and the current State of the Controller
		protected AnimationClip GetAttackAnimationClip()
		{
			string stateID = slots.actor.slots.controller.StateID();
			AnimationClip animationClip = null;
			if(stateID == DefaultState.idString || stateID == LandingState.idString)
			{
				animationClip = attackAnimations.standing;
			}
			else if(stateID == MovingState.idString)
			{
				animationClip = attackAnimations.moving;
			}
			else if(stateID == LadderState.idString)
			{
				animationClip = attackAnimations.climbing;
			}
			else if(stateID == DashState.idString)
			{
				animationClip = attackAnimations.dashing;
			}
			else if(stateID == JumpState.idString || stateID == FallingState.idString)
			{
				animationClip = attackAnimations.jumping;
			}
			else if(stateID == CrouchState.idString)
			{
				animationClip = attackAnimations.crouching;
			}
			else if(stateID == WallClingState.idString)
			{
				animationClip = attackAnimations.wallClinging;
			}
			else if(stateID == StairClimbingState.idString)
			{
				animationClip = attackAnimations.stairClimbing;
			}
			else if(stateID == GroundPoundState.idString)
			{
				animationClip = attackAnimations.groundPounding;
			}
			else if(stateID == FlutterJumpState.idString)
			{
				animationClip = attackAnimations.flutterJumping;
			}
			else if(stateID == GlideState.idString)
			{
				animationClip = attackAnimations.gliding;
			}

			return animationClip;
		}

		protected virtual bool IsAttackAttempted()
		{
			bool isAttackAttempted = false;

			if(slots.actor.slots.input)
			{
				if((input.button == AttackImportance.Primary || input.button == AttackImportance.All) && ((slots.actor.slots.input.isAttackButtonDown && !input.requireFreshButtonPress) || slots.actor.slots.input.isAttackButtonDownThisFrame))
				{
					isAttackAttempted = true;
				}
				else if((input.button == AttackImportance.Sub || input.button == AttackImportance.All) && ((slots.actor.slots.input.isSubAttackButtonDown && !input.requireFreshButtonPress) || slots.actor.slots.input.isSubAttackButtonDownThisFrame))
				{
					isAttackAttempted = true;
				}
				else if((input.button == AttackImportance.Sub_2 || input.button == AttackImportance.All) && ((slots.actor.slots.input.isSubAttack_2ButtonDown && !input.requireFreshButtonPress) || slots.actor.slots.input.isSubAttack_2ButtonDownThisFrame))
				{
					isAttackAttempted = true;
				}
				else if((input.button == AttackImportance.Sub_3 || input.button == AttackImportance.All) && ((slots.actor.slots.input.isSubAttack_3ButtonDown && !input.requireFreshButtonPress) || slots.actor.slots.input.isSubAttack_3ButtonDownThisFrame))
				{
					isAttackAttempted = true;
				}
			}

			if(input.requireVerticalPress)
			{
				if((input.verticalPressRequired == Direction.Vertical.Up && slots.actor.slots.input.verticalAxis != 1.0f) || (input.verticalPressRequired == Direction.Vertical.Down && slots.actor.slots.input.verticalAxis != -1.0f) || (input.verticalPressRequired == Direction.Vertical.Neutral && slots.actor.slots.input.verticalAxis != 0.0f))
				{
					isAttackAttempted = false;
				}
			}

			return isAttackAttempted;
		}

		//Don't call this directly; this will be auto-called after a successful call to Begin()
		protected IEnumerator AttackCoroutine()
		{
			isAttackActive = true;
			slots.actor.currentAttack = this;
			currentCooldownTime = 0;
			isCooldownComplete = false;

			if(slots.audio && audioClip)
			{
				slots.actor.PlaySoundIfOnCamera(audioClip, slots.audio.pitch, 1.0f, slots.audio);
			}

			if(slots.boxCollider != null && willAutoEnableCollider)
			{
				slots.boxCollider.enabled = true;
			}

			if(slots.spriteRenderer != null)
			{
				slots.spriteRenderer.enabled = true;
			}

			attackDirection = AttackDirection.Ahead;
			if(slots.actor.slots.controller.StateID() == WallClingState.idString)
			{
				WallClingState wallClingState = slots.actor.slots.controller.currentState.GetComponent<WallClingState>();
				if(wallClingState && wallClingState.attacksReverseOnWall)
				{
					attackDirection = AttackDirection.Behind;
				}
			}

			transform.localScale = (attackDirection == AttackDirection.Ahead) ? new Vector3(1.0f, 1.0f, 1.0f) : new Vector3(-1.0f, 1.0f, 1.0f);

			AnimationClip attackAnimationClip = GetAttackAnimationClip();
			if(attackAnimationClip != null && slots.actor.slots.anim)
			{
				slots.animator.Play(attackAnimationClip.name, 0, 0.0f);
			}

			AnimationClip actorAnimationClip = GetActorAnimationClip();
			if(actorAnimationClip != null && slots.actor.slots.anim)
			{
				float timeToSyncTo = (slots.actor.slots.controller.StateID() == MovingState.idString) ? (float)slots.actor.slots.anim.GetCurrentAnimatorStateInfo(0).normalizedTime : 0.0f; //If the slots.actor is moving, sync the move-attack cycle to where their move cycle was

				if(willSyncMoveAnimation)
				{
					timeToSyncTo = 0.0f;
				}

				slots.actor.slots.anim.Play(actorAnimationClip.name, 0, timeToSyncTo);
			}

			if(projectile.rexPool != null && projectile.willAutoCreateProjectile)
			{
				CreateProjectile();
			}

			float durationWithoutAnimation = 0.5f;
			float duration = (attackAnimationClip != null) ? attackAnimationClip.length : durationWithoutAnimation;
			if(attackAnimationClip == null && actorAnimationClip != null)
			{
				duration = actorAnimationClip.length;
			}

			while(slots.actor.timeStop.isTimeStopped) yield return null;
			yield return new WaitForSeconds(duration);

			isAttackActive = false;

			if(slots.actor != null)
			{
				slots.actor.OnAttackComplete(this);
			}

			Reset();
		}

		protected void SetCrouchPosition()
		{
			if(slots.actor.slots.controller.StateID() == CrouchState.idString)
			{
				transform.localPosition = new Vector3(nonCrouchingPosition.x, nonCrouchingPosition.y - crouchOffset, nonCrouchingPosition.z);
			}
			else
			{
				transform.localPosition = nonCrouchingPosition;
			}
		}
		#endregion
	}	
}
