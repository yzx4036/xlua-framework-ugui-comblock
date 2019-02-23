/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class EnemyAI:RexAI 
	{
		[Tooltip("Whether or not this AI routine is active.")]
		public bool isEnabled = true;

		[System.Serializable]
		public class Slots
		{
			//TEST
			[Tooltip("A slot for the RexController affected by this AI component.")]
			public RexController controller;
			[Tooltip("A slot for the RexPhysics affected by this AI component.")]
			public RexPhysics physicsObject;
			[Tooltip("A slot for the BoxCollider2D used by the RexController this AI component acts on.")]
			public BoxCollider2D boxCollider;
		}

		[System.Serializable]
		public class StartingMovement
		{
			[Tooltip("Whether this actor initially moves left or right.")]
			public Direction.Horizontal horizontal;
			[Tooltip("Whether this actor initially moves up or down, assuming vertical movement is enabled.")]
			public Direction.Vertical vertical;
			[Tooltip("If True, this actor will attempt to face and move towards the player on initialization, regardless of their above Startming Movement settings.")]
			public bool willFacePlayerAtStart = true;
			[Tooltip("Who doesn't love medusa heads?")]
			public bool moveInWave;
		}

		[System.Serializable]
		public class ChangeDirection
		{
			[Tooltip("If True, this actor will periodically change its movement direction according to the below settings.")]
			public bool willChangeDirectionIntervals = false;
			[Tooltip("The number of frames between the actor changing its movement direction.")]
			public int framesBetweenChange = 128;
			[Tooltip("The number of frames the actor will pause movement for between changing movement directions.")]
			public int pauseFrames = 128;
			[Tooltip("If True, the actor's new horizontal movement direction will be random upon changing directions.")]
			public bool randomizeX;
			[Tooltip("If True, the actor's new vertical movement direction will be random upon changing directions.")]
			public bool randomizeY;
			[Tooltip("If this number is greater than 0, it will limit their direction changes to that number. If this number is 0, they can change direction indefinitely.")]
			public int flipsAllowed = 0;

			[HideInInspector]
			public int currentFlip = 0;

			[HideInInspector]
			public int currentFrameBetweenChanges = 0;

			[HideInInspector]
			public int currentPauseFrame = 0;

			[HideInInspector]
			public Vector2 nextDirection;
		}

		[System.Serializable]
		public class MoveTowardsTransform
		{
			[Tooltip("Whether or not moving towards a Transform is enabled.")]
			public bool isEnabled = true;
			[Tooltip("If this is slotted, this actor will attempt to move towards the Transform slotted here. Is Enabled must be checked below for this to work, as well as Will Move Towards X and/or Will Move Towards Y.")]
			public Transform transformToMoveTowards;
			[Tooltip("If True, this actor will attempt to move towards the player.")]
			public bool usePlayerAsTarget;
			[Tooltip("If True, this actor will attempt to move towards the target's X position.")]
			public bool willMoveTowardsX;
			[Tooltip("If True, this actor will attempt to move towards the target's Y position.")]
			public bool willMoveTowardsY;
			[Tooltip("When moving towards a Transform, this actor will attempt to maintain this amount of space between them.")]
			public float buffer = 0.5f;
		}

		[System.Serializable]
		public class Turn
		{
			[Tooltip("If True, this actor will turn around when it runs into a wall.")]
			public bool onWallContact = true;
			[Tooltip("If True, this actor will change its vertical movement direction when it contacts the ceiling or floor.")]
			public bool onCeilingFloorContact = false;
			[Tooltip("If True, this actor will turn around when it encounters a ledge at its feet.")]
			public bool onLedgeContact = true;
		}

		[System.Serializable]
		public class Jump
		{
			[HideInInspector]
			public JumpState jumpState;
			[Tooltip("If True, this actor will jump when it encounters a ledge at its feet. It must have an attached JumpState component for this to work.")]
			public bool onLedges;
			[Tooltip("If True, this actor will automatically jump at the interval specified in Frames Between Jumps. It must have an attached JumpState component for this to work.")]
			public bool willAutoJumpAtIntervals = true;
			[Tooltip("If Will Auto Jump At Intervals is True, this value governs the number of frames between jump attempts.")]
			public int framesBetweenJumps = 128;

			[HideInInspector]
			public int currentFrameBetweenJumps;
		}

		[System.Serializable]
		public class EnableNearActor
		{
			[Tooltip("If True, this EnemyAI component will be disabled until it's within the set proximity to Transform To Enable Near.")]
			public bool onlyEnableWhenClose = false;
			[Tooltip("If this EnemyAI component has Is Enabled set to False initially, and Transform To Enable Near is slotted, this EnemyAI component will become active when in proximity to the Transform To Enable Near.")]
			public Transform transformToEnableNear;
			[Tooltip("If Transform To Enable Near is slotted, this EnemyAI component will become active when it's within this distance of Transform To Enable Near.")]
			public Vector2 distanceForEnable = new Vector2(10, 10);
			[Tooltip("If True, the player will be used as the Transform To Enable Near target.")]
			public bool usePlayerAsTarget;
		}

		[Tooltip("Slots for common components this EnemyAI needs to function.")]
		public Slots slots;
		[Tooltip("Options for how this actor moves when first initialized.")]
		public StartingMovement startingMovement;
		[Tooltip("Options for when this actor changes direction.")]
		public Turn turn;
		[Tooltip("Options for when this actor jumps.")]
		public Jump jump;
		[Tooltip("Options for letting this actor move towards another target.")]
		public MoveTowardsTransform moveTowards;
		[Tooltip("Options for only enabling this actor when it's nearby to another actor.")]
		public EnableNearActor enableNearActor;
		[Tooltip("Options for letting this actor change its movement direction periodically.")]
		public ChangeDirection changeDirection;
		[Tooltip("Options for this actor using attacks.")]
		public Attacks attacks;

		protected float fixedTime;
		protected RexActor player;

		[System.Serializable]
		public class Attacks
		{
			[Tooltip("A slot for the Attack component this actor will use to attack.")]
			public Attack attackToPerform;
			[Tooltip("If True, this actor will automatically attack at a set interval.")]
			public bool willAutoAttackAtIntervals = true;
			[Tooltip("If Will Auto Attack At Intervals is True, this is the minimum number of frames between its attacks triggering. The actual duration will be randomized between Min Frames Between Attacks and Max Frames Between Attacks.")]
			public int minFramesBetweenAttacks;
			[Tooltip("If Will Auto Attack At Intervals is True, this is the maximum number of frames between its attacks triggering. The actual duration will be randomized between Min Frames Between Attacks and Max Frames Between Attacks.")]
			public int maxFramesBetweenAttacks;

			[HideInInspector]
			public int currentFrameBetweenAttacks;
		}

		void Awake() 
		{
			if(jump.jumpState == null)
			{
				jump.jumpState = GetComponent<JumpState>();
			}
		}

		void Start() 
		{
			player = GameManager.Instance.player;

			if(enableNearActor.usePlayerAsTarget)
			{
				enableNearActor.transformToEnableNear = player.transform;
			}

			if(moveTowards.isEnabled && (moveTowards.willMoveTowardsX || moveTowards.willMoveTowardsY))
			{
				if(moveTowards.usePlayerAsTarget)
				{
					moveTowards.transformToMoveTowards = player.transform;
				}
			}

			changeDirection.currentFrameBetweenChanges = changeDirection.framesBetweenChange;
			changeDirection.currentPauseFrame = changeDirection.pauseFrames;

			changeDirection.currentFlip = changeDirection.flipsAllowed;

			if(startingMovement.willFacePlayerAtStart)
			{
				FacePlayer();
				if(player) //If we turn to face something, make sure our startingDirection is set to the direction we face
				{
					if(startingMovement.horizontal != Direction.Horizontal.Neutral)
					{
						if(player.transform.position.x < transform.position.x)
						{
							startingMovement.horizontal = Direction.Horizontal.Left;
						}
						else
						{
							startingMovement.horizontal = Direction.Horizontal.Right;
						}
					}

					if(startingMovement.vertical != Direction.Vertical.Neutral)
					{
						if(player.transform.position.y < transform.position.y)
						{
							startingMovement.vertical = Direction.Vertical.Down;
						}
						else
						{
							startingMovement.vertical = Direction.Vertical.Up;
						}
					}

					if(moveTowards.usePlayerAsTarget)
					{
						moveTowards.transformToMoveTowards = player.transform;
					}
				}
			}

			attacks.currentFrameBetweenAttacks = RexMath.RandomInt(attacks.minFramesBetweenAttacks, attacks.maxFramesBetweenAttacks);
			jump.currentFrameBetweenJumps = jump.framesBetweenJumps;

			if(enableNearActor.onlyEnableWhenClose)
			{
				isEnabled = false;
			}

			if(!isEnabled)
			{
				return;
			}

			if(slots.controller)
			{
				slots.controller.SetAxis(new Vector2((int)startingMovement.horizontal, (int)startingMovement.vertical));
			}
		}

		void FixedUpdate()
		{
			if(slots.controller.slots.actor.timeStop.isTimeStopped)
			{
				return;
			}

			fixedTime += Time.fixedDeltaTime;

			if(!isEnabled)
			{
				if(enableNearActor.transformToEnableNear != null)
				{
					CheckActorDistance(enableNearActor.transformToEnableNear);
				}

				if(!isEnabled)
				{
					return;
				}
			}

			if(!(slots.controller && slots.controller.StateID() != DeathState.idString && !slots.controller.isKnockbackActive && Time.timeScale > 0.0f))
			{
				return;
			}

			if(!slots.controller || !slots.physicsObject)
			{
				return;
			}

			if(moveTowards.transformToMoveTowards != null)
			{
				MoveTowards(moveTowards.transformToMoveTowards);
			}

			//If this has a target to move towards, don't turn around on wall or ledge contact
			if(!(moveTowards.transformToMoveTowards != null && (moveTowards.willMoveTowardsX || moveTowards.willMoveTowardsY)))
			{
				CheckForLedges();
				TurnOnWallContact();
			}

			if(jump.willAutoJumpAtIntervals)
			{
				CheckForAutoJumps();
			}

			if(changeDirection.willChangeDirectionIntervals)
			{
				CheckForDirectionChange();
			}

			if(startingMovement.moveInWave)
			{
				slots.controller.SetAxis(new Vector2(slots.controller.axis.x, Mathf.Sin(fixedTime)));
			}

			if(attacks.willAutoAttackAtIntervals)
			{
				CheckForAttacks();
			}
		}

		public void OnNewStateAdded(RexState _state)
		{
			if(_state.id == JumpState.idString && !jump.jumpState)
			{
				jump.jumpState = _state as JumpState;
			}
		}

		public void FacePlayer()
		{
			if(player != null && slots.controller)
			{
				if(player.transform.position.x < transform.position.x)
				{
					slots.controller.FaceDirection(Direction.Horizontal.Left);
				}
				else
				{
					slots.controller.FaceDirection(Direction.Horizontal.Right);
				}
			}
		}

		protected void CheckForLedges()
		{
			if(jump.onLedges || turn.onLedgeContact)
			{
				if(slots.controller.StateID() == LadderState.idString)
				{
					return;
				}

				if(slots.controller.axis.x != 0.0f && slots.physicsObject.IsOnSurface() && RaycastHelper.IsNextToLedge((Direction.Horizontal)slots.controller.axis.x, (Direction.Vertical)(slots.controller.GravityScaleMultiplier() * -1.0f), slots.boxCollider))
				{
					if(jump.onLedges && !slots.physicsObject.IsAgainstEitherWall())
					{
						if(jump.jumpState)
						{
							jump.jumpState.Begin();
						}
					}
					else if(turn.onLedgeContact && !slots.physicsObject.IsAgainstEitherWall())
					{
						slots.controller.SetAxis(new Vector2(slots.controller.axis.x * -1.0f, slots.controller.axis.y));
					}
				}
			}
		}

		protected void CheckForAutoJumps()
		{
			if(jump.jumpState == null)
			{
				jump.jumpState = GetComponent<JumpState>();
			}

			if(jump.jumpState != null)
			{
				jump.currentFrameBetweenJumps --;
				if(jump.currentFrameBetweenJumps <= 0)
				{
					jump.currentFrameBetweenJumps = jump.framesBetweenJumps;
					jump.jumpState.Begin();
				}
			}
		}

		protected void CheckForDirectionChange()
		{
			if(changeDirection.flipsAllowed > 0 && changeDirection.currentFlip > changeDirection.flipsAllowed)
			{
				return;
			}

			changeDirection.currentFrameBetweenChanges --;
			if(changeDirection.currentFrameBetweenChanges <= 0)
			{
				if(changeDirection.currentPauseFrame == changeDirection.pauseFrames)
				{
					changeDirection.nextDirection.x = slots.controller.axis.x * -1.0f;
					changeDirection.nextDirection.y = slots.controller.axis.y * -1.0f;

					if(changeDirection.randomizeX)
					{
						changeDirection.nextDirection.x = (RexMath.RandomInt(1, 10) >= 6) ? -1.0f : 1.0f;
					}

					if(changeDirection.randomizeY)
					{
						changeDirection.nextDirection.y = (RexMath.RandomInt(1, 10) >= 6) ? -1.0f : 1.0f;
					}

					slots.controller.SetAxis(Vector2.zero);
					slots.controller.SetStateToDefault();
				}

				changeDirection.currentPauseFrame --;

				if(changeDirection.currentPauseFrame <= 0)
				{
					changeDirection.currentFrameBetweenChanges = changeDirection.framesBetweenChange;
					changeDirection.currentFlip ++;
					changeDirection.currentPauseFrame = changeDirection.pauseFrames;
					slots.controller.SetAxis(changeDirection.nextDirection);
				}
			}
		}

		protected void CheckForAttacks()
		{
			if(attacks.attackToPerform != null)
			{
				attacks.currentFrameBetweenAttacks --;
				if(attacks.currentFrameBetweenAttacks <= 0)
				{
					attacks.currentFrameBetweenAttacks = RexMath.RandomInt(attacks.minFramesBetweenAttacks, attacks.maxFramesBetweenAttacks);
					attacks.attackToPerform.Begin();
				}
			}
		}

		protected void TurnOnWallContact()
		{
			if(slots.controller)
			{
				if(slots.controller.StateID() == LadderState.idString)
				{
					return;
				}

				if(turn.onWallContact && ((slots.controller.axis.x < 0.0f && slots.physicsObject.properties.isAgainstLeftWall) || (slots.controller.axis.x > 0.0f && slots.physicsObject.properties.isAgainstRightWall)))
				{
					slots.controller.SetAxis(new Vector2(slots.controller.axis.x * -1.0f, slots.controller.axis.y));
				}
				else if(turn.onCeilingFloorContact && ((slots.controller.axis.y < 0.0f && slots.physicsObject.properties.isGrounded) || (slots.controller.axis.y > 0.0f && slots.physicsObject.properties.isAgainstCeiling)))
				{
					slots.controller.SetAxis(new Vector2(slots.controller.axis.x, slots.controller.axis.y * -1.0f));
				}
			}
		}

		protected void CheckActorDistance(Transform actorTransform)
		{
			Vector2 distance = new Vector2();
			distance.x = Mathf.Abs(transform.position.x - actorTransform.position.x);
			distance.y = Mathf.Abs(transform.position.y - actorTransform.position.y);

			if(distance.x <= enableNearActor.distanceForEnable.x && distance.y <= enableNearActor.distanceForEnable.y)
			{
				RexActor actor = enableNearActor.transformToEnableNear.GetComponent<RexActor>();
				if(actor != null && actor.isBeingLoadedIntoNewScene)
				{
					return;
				}

				isEnabled = true;
				if(slots.controller)
				{
					slots.controller.SetAxis(new Vector2((int)startingMovement.horizontal, (int)startingMovement.vertical));
				}
			}
		}

		protected void MoveTowards(Transform _transform)
		{
			if(moveTowards.willMoveTowardsX && _transform != null)
			{
				if(_transform)
				{
					if(_transform.position.x > transform.position.x + moveTowards.buffer)
					{
						slots.controller.SetAxis(new Vector2(1.0f, slots.controller.axis.y));
					}
					else if(_transform.position.x < transform.position.x - moveTowards.buffer)
					{
						slots.controller.SetAxis(new Vector2(-1.0f, slots.controller.axis.y));
					}
					else
					{
						slots.controller.SetAxis(new Vector2(0.0f, slots.controller.axis.y));
					}

					if((_transform.position.x > transform.position.x && slots.physicsObject.CheckForWallContact(Direction.Horizontal.Right)) || (_transform.position.x < transform.position.x && slots.physicsObject.CheckForWallContact(Direction.Horizontal.Left)))
					{
						slots.controller.SetAxis(new Vector2(0.0f, slots.controller.axis.y));
					}
				}
			}

			if(moveTowards.willMoveTowardsY && _transform != null)
			{
				//float adjustedBuffer = (slots.controller.StateID() == LadderState.idString) ? -0.5f : moveTowards.buffer;
				if(player)
				{
					LadderState ladderState = slots.controller.GetComponent<LadderState>();
					if(slots.controller.StateID() == LadderState.idString)
					{
						if(_transform.position.y > transform.position.y + moveTowards.buffer || (ladderState.GetDistanceFromTop() < 1.5f && slots.controller.axis.y > 0.0f))
						{
							slots.controller.SetAxis(new Vector2(slots.controller.axis.x, 1.0f));
						}
						else if(_transform.position.y < transform.position.y - moveTowards.buffer || (ladderState.GetDistanceFromBottom() < 1.5f && slots.controller.axis.y < 0.0f))
						{
							slots.controller.SetAxis(new Vector2(slots.controller.axis.x, -1.0f));
						}
						else
						{
							if(ladderState.GetDistanceFromTop() > 1.5f && ladderState.GetDistanceFromBottom() > 1.5f)
							{
								slots.controller.SetAxis(new Vector2(slots.controller.axis.x, 0.0f));
							}
						}
					}
					else
					{
						if(_transform.position.y > transform.position.y + moveTowards.buffer)
						{
							slots.controller.SetAxis(new Vector2(slots.controller.axis.x, 1.0f));
						}
						else if(_transform.position.y < transform.position.y - moveTowards.buffer)
						{
							slots.controller.SetAxis(new Vector2(slots.controller.axis.x, -1.0f));
						}
						else
						{
							slots.controller.SetAxis(new Vector2(slots.controller.axis.x, 0.0f));
						}
					}
				}
			}
		}
	}
}
