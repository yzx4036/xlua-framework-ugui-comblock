/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	[SelectionBase]
	public class Projectile:RexObject
	{
		[System.Serializable]
		public class DestroyOnHit
		{
			public bool player;
			public bool enemy;
		}

		public Direction.Vertical startingVerticalDirection;

		[System.Serializable]
		public class Animations
		{
			public AnimationClip defaultAnimation;
			public AnimationClip spawnAnimation;
			public AnimationClip deathAnimation;
		}

		[System.Serializable]
		public class Sounds
		{
			public AudioClip fireSound;
			public AudioClip deathSound;
			public AudioClip reflectSound;
		}

		[System.Serializable]
		public class Reflection
		{
			public bool willReflectOnReflectors = true; //Set to True if this will be reflected when it hits Reflector objects
			public bool willReflectAtAngles = true; //If this is True, this will reflect at various angles based on how far it is from the center of a Reflector it collides with
			public bool willShakeScreenOnReflectionDeath = true;
			public DestroyOnHit reflectedDestroyOnHit;
			public bool willDamagePlayerOnReflect;
			public bool willDamageEnemiesOnReflect;
		}

		public DestroyOnHit willDestroyOnHit;
		public Reflection reflection;
		public Animations animations;
		public Sounds sounds;
		public Vector2 movementSpeed;
		public ContactDamage contactDamage;
		public bool willDestroyWhenOffscreen = true;
		public bool willDestroyWhenSceneChanges = true;
		public bool willShakeScreenOnDeath;
		public float rotationSpeed; //Setting this higher than 0 lets the projectile sprite rotate each frame it's being fired

		public CollisionSide ricochetOnTerrainCollision;
		public CollisionSide destroyOnTerrainCollision;

		public float acceleration;

		public DirectionChange directionChange;
		public int horizontalFlipsAllowed;
		public int verticalFlipsAllowed;

		public int framesBeforeDespawn;

		[HideInInspector]
		public RexActor spawningActor; //A reference to the RexActor that spawned this, if any

		[HideInInspector]
		public Attack spawningAttack; //A reference to the Attack that spawned this, if any

		[System.Serializable]
		public class DirectionChange
		{
			public FramesBeforeDirectionChange framesBeforeDirectionChange;
			public bool swapOnActorFlip;
		}

		protected Direction direction;
		protected int framesMovedInCurrentHorizontalDirection = 0;
		protected int framesMovedInCurrentVerticalDirection = 0;

		[System.Serializable]
		public class CollisionSide
		{
			public bool onCeiling;
			public bool onFloor;
			public bool onLeft;
			public bool onRight;
		}

		protected bool isFiring;
		protected bool isDead;
		protected bool isBeingReflected;

		//These values let the projectile reset to its original state if it's used and respawned; they are set automatically in Awake()
		protected Vector2 originalMovementSpeed;
		protected bool originalDamagePlayer;
		protected bool originalDamageEnemies;

		protected Direction previousDirection;

		protected int remainingFramesBeforeDespawn;

		protected int remainingHorizontalFlips;
		protected int remainingVerticalFlips;

		protected Direction.Horizontal firingDirection;

		[HideInInspector]
		public bool isAimable = true; //Whether or not this projectile was fired using 8-way aiming; set by the Attack that fires it, if any

		[HideInInspector]
		public DestroyOnHit originalDestroyOnHit;

		[System.Serializable]
		public class FramesBeforeDirectionChange
		{
			public int up;
			public int down;
			public int left;
			public int right;
		}

		void Awake()
		{
			gameObject.tag = "Projectile";
			gameObject.layer = LayerMask.NameToLayer("Default");

			SaveOriginalValues();

			if(!willDestroyWhenSceneChanges)
			{
				DontDestroyOnLoad(gameObject);
			}

			direction = new Direction();
			previousDirection = new Direction();
		}

		void Update()
		{
			if(slots.spriteRenderer != null && isFiring)
			{
				slots.spriteRenderer.transform.localEulerAngles = new Vector3(slots.spriteRenderer.transform.localEulerAngles.x, slots.spriteRenderer.transform.localEulerAngles.y, slots.spriteRenderer.transform.localEulerAngles.z + rotationSpeed * slots.physicsObject.properties.velocity.x);
			}

			if(willDestroyWhenOffscreen)
			{
				float buffer = 2.5f;
				if(!CameraHelper.CameraContainsPoint(transform.position, buffer) && !isDead)
				{
					Clear();
				}
			}
		}

		void FixedUpdate() 
		{
			if(isFiring)
			{
				UpdateMovement();
			}
		}

		#region public methods

		public void Fire(Vector2 _startingPosition, Direction.Horizontal _horizontal, Direction.Vertical _vertical, RexActor _spawningActor, RexPool _parentSpawnPool = null)
		{
			if(!willDestroyWhenSceneChanges)
			{
				DontDestroyOnLoad(gameObject);
			}

			isFiring = true;
			isBeingReflected = false;
			movementSpeed = originalMovementSpeed;
			spawningActor = _spawningActor;

			if(sounds.fireSound)
			{
				GetComponent<AudioSource>().PlayOneShot(sounds.fireSound);
			}

			if(_parentSpawnPool != null)
			{
				parentSpawnPool = _parentSpawnPool;
			}

			SetPosition(_startingPosition);
			SetHorizontalDirection(_horizontal);
			SetVerticalDirection((Direction.Vertical)((int)startingVerticalDirection * (int)_vertical * PhysicsManager.Instance.gravityScale));

			firingDirection = _horizontal;

			slots.physicsObject.properties.acceleration = Vector2.zero;

			float startingXSpeed = (acceleration == 0.0f) ? movementSpeed.x : 0.0f;
			float startingYSpeed = (acceleration == 0.0f) ? movementSpeed.y : 0.0f;

			if(isAimable)
			{
				if(direction.horizontal != Direction.Horizontal.Neutral && direction.vertical != Direction.Vertical.Neutral)
				{
					startingXSpeed *= 0.707f;
					startingYSpeed = startingXSpeed;
				}
				else
				{
					if(startingXSpeed == 0.0f)
					{
						startingXSpeed = startingYSpeed;
					}
					else if(startingYSpeed == 0.0f)
					{
						startingYSpeed = startingXSpeed;
					}
				}
			}

			slots.physicsObject.SetVelocityX(startingXSpeed * (int)direction.horizontal);
			slots.physicsObject.SetAccelerationCapX(movementSpeed.x * (int)direction.horizontal);

			slots.physicsObject.SetVelocityY(startingYSpeed * (int)direction.vertical);
			slots.physicsObject.SetAccelerationCapY(movementSpeed.y * (int)direction.vertical);
		}

		public override void Clear()
		{
			StopAllCoroutines();

			gameObject.SetActive(false);

			if(parentSpawnPool)
			{
				transform.parent = parentSpawnPool.transform;
				parentSpawnPool.Despawn(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void OnSpawned()
		{
			slots.physicsObject.isEnabled = true;

			willDespawnOnSceneExit = true;
			isFiring = false;
			isDead = false;
			isBeingReflected = false;
			spawningActor = null;
			spawningAttack = null;

			framesMovedInCurrentHorizontalDirection = 0;
			framesMovedInCurrentVerticalDirection = 0;

			remainingFramesBeforeDespawn = framesBeforeDespawn;

			remainingHorizontalFlips = horizontalFlipsAllowed;
			remainingVerticalFlips = verticalFlipsAllowed;

			ResetToOriginalValues();

			GetComponent<Collider2D>().enabled = true;

			direction.horizontal = Direction.Horizontal.Neutral;
			direction.vertical = Direction.Vertical.Neutral;

			slots.physicsObject.Reset();

			StopAllCoroutines();
			StartCoroutine("SpawnCoroutine");

			ParentHelper.Parent(gameObject, ParentHelper.ParentObject.Projectiles);
		}

		#endregion

		#region private methods

		protected void SaveOriginalValues()
		{
			originalMovementSpeed = movementSpeed;

			ContactDamage contactDamage = GetComponent<ContactDamage>();
			if(contactDamage != null)
			{
				originalDamagePlayer = contactDamage.willDamagePlayer;
				originalDamageEnemies = contactDamage.willDamageEnemies;
			}

			originalDestroyOnHit.enemy = willDestroyOnHit.enemy;
			originalDestroyOnHit.player = willDestroyOnHit.player;
		}

		protected void ResetToOriginalValues()
		{

			ContactDamage contactDamage = GetComponent<ContactDamage>();
			if(contactDamage != null)
			{
				contactDamage.willDamagePlayer = originalDamagePlayer;
				contactDamage.willDamageEnemies = originalDamageEnemies;
			}

			willDestroyOnHit.enemy = originalDestroyOnHit.enemy;
			willDestroyOnHit.player = originalDestroyOnHit.player;
		}

		protected IEnumerator SpawnCoroutine()
		{
			if(animations.spawnAnimation != null)
			{
				slots.anim.Play(animations.spawnAnimation.name);
				while(timeStop.isTimeStopped) yield return null;
				yield return new WaitForSeconds(animations.spawnAnimation.length);
			}

			if(animations.defaultAnimation != null)
			{
				slots.anim.Play(animations.defaultAnimation.name);
			}

			yield return null;
		}

		protected virtual void UpdateMovement()
		{
			Vector2 adjustedMovementSpeed = movementSpeed;
			if(isAimable)
			{
				if(direction.horizontal != Direction.Horizontal.Neutral && direction.vertical != Direction.Vertical.Neutral)
				{
					adjustedMovementSpeed.x *= 0.707f;
				}

				adjustedMovementSpeed.y = adjustedMovementSpeed.x;
			}

			if(Mathf.Abs(acceleration) > 0.0f)
			{
				slots.physicsObject.MoveX(adjustedMovementSpeed.x * (int)direction.horizontal);
				//slots.physicsObject.properties.velocityCap = new Vector2(adjustedMovementSpeed.x * (int)direction.horizontal, slots.physicsObject.properties.velocityCap.y);
				slots.physicsObject.properties.acceleration.x = acceleration * (int)direction.horizontal;
			}
			else
			{
				slots.physicsObject.properties.velocityCap = new Vector2((int)direction.horizontal * adjustedMovementSpeed.x, slots.physicsObject.properties.velocityCap.y);
			}

			if(Mathf.Abs(acceleration) > 0.0f)
			{
				slots.physicsObject.MoveY(adjustedMovementSpeed.y * (int)direction.vertical);

				//slots.physicsObject.properties.velocityCap = new Vector2(slots.physicsObject.properties.velocityCap.x, adjustedMovementSpeed.y * (int)direction.vertical);
				slots.physicsObject.properties.acceleration.y = acceleration * (int)direction.vertical;
			}
			else
			{
				slots.physicsObject.properties.velocityCap = new Vector2(slots.physicsObject.properties.velocityCap.x, (int)direction.vertical * adjustedMovementSpeed.y);
			}

			CheckForDespawn();

			if(!isAimable)
			{
				CheckProjectileDirection();
			}
		}

		protected void CheckForDespawn()
		{
			if(framesBeforeDespawn > 0)
			{
				remainingFramesBeforeDespawn --;
				if(remainingFramesBeforeDespawn <= 0)
				{
					OnDeath();
				}
			}
		}

		protected void CheckProjectileDirection()
		{
			int horizontalFramesBeforeFlip = (direction.horizontal == Direction.Horizontal.Right) ? directionChange.framesBeforeDirectionChange.right : directionChange.framesBeforeDirectionChange.left;
			int verticalFramesBeforeFlip = ((PhysicsManager.Instance.gravityScale >= 0.0f && direction.vertical == Direction.Vertical.Up) || (PhysicsManager.Instance.gravityScale < 0.0f && direction.vertical == Direction.Vertical.Down)) ? directionChange.framesBeforeDirectionChange.up : directionChange.framesBeforeDirectionChange.down;

			if(firingDirection == Direction.Horizontal.Left)
			{
				horizontalFramesBeforeFlip = (direction.horizontal == Direction.Horizontal.Left) ? directionChange.framesBeforeDirectionChange.right : directionChange.framesBeforeDirectionChange.left;
			}

			if(direction.horizontal == Direction.Horizontal.Right && horizontalFramesBeforeFlip > 0 && framesMovedInCurrentHorizontalDirection >= horizontalFramesBeforeFlip && (remainingHorizontalFlips > 0 || horizontalFlipsAllowed == 0))
			{
				FlipHorizontal();
				if(acceleration == 0.0f)
				{
					slots.physicsObject.SetVelocityX((int)direction.horizontal * Mathf.Abs(slots.physicsObject.properties.velocity.x));
				}

				remainingHorizontalFlips --;
				framesMovedInCurrentHorizontalDirection = 0;
			}
			else if(direction.horizontal == Direction.Horizontal.Left && horizontalFramesBeforeFlip > 0 && framesMovedInCurrentHorizontalDirection >= horizontalFramesBeforeFlip && (remainingHorizontalFlips > 0 || horizontalFlipsAllowed == 0))
			{
				FlipHorizontal();
				if(acceleration == 0.0f)
				{
					slots.physicsObject.SetVelocityX((int)direction.horizontal * Mathf.Abs(slots.physicsObject.properties.velocity.x));
				}

				remainingHorizontalFlips --;
				framesMovedInCurrentHorizontalDirection = 0;
			}

			if(previousDirection.horizontal == direction.horizontal)
			{
				framesMovedInCurrentHorizontalDirection ++;
			}
			else
			{
				framesMovedInCurrentHorizontalDirection = 0;
			}

			previousDirection.horizontal = direction.horizontal;

			if(direction.vertical == Direction.Vertical.Up && verticalFramesBeforeFlip > 0 && framesMovedInCurrentVerticalDirection >= verticalFramesBeforeFlip && (remainingVerticalFlips > 0 || verticalFlipsAllowed == 0))
			{
				FlipVertical();
				if(acceleration == 0.0f)
				{
					slots.physicsObject.SetVelocityY((int)direction.vertical * Mathf.Abs(slots.physicsObject.properties.velocity.y));
				}

				//slots.physicsObject.SetVelocityY((int)direction.vertical * movementSpeed.y);
				remainingVerticalFlips --;
				framesMovedInCurrentVerticalDirection = 0;
			}
			else if(direction.vertical == Direction.Vertical.Down && verticalFramesBeforeFlip > 0 && framesMovedInCurrentVerticalDirection >= verticalFramesBeforeFlip && (remainingVerticalFlips > 0 || verticalFlipsAllowed == 0))
			{
				FlipVertical();

				if(acceleration == 0.0f)
				{				
					slots.physicsObject.SetVelocityY((int)direction.vertical * Mathf.Abs(slots.physicsObject.properties.velocity.y));
				}

				//slots.physicsObject.SetVelocityY((int)direction.vertical * movementSpeed.y);
				remainingVerticalFlips --;
				framesMovedInCurrentVerticalDirection = 0;
			}

			if(previousDirection.vertical == direction.vertical)
			{
				framesMovedInCurrentVerticalDirection ++;
			}
			else
			{
				framesMovedInCurrentVerticalDirection = 0;
			}

			previousDirection.vertical = direction.vertical;
		}

		protected virtual void OnDeath(Side side = Side.None)
		{
			isDead = true;
			isFiring = false;

			slots.physicsObject.isEnabled = false;

			if(willShakeScreenOnDeath || (reflection.willShakeScreenOnReflectionDeath && isBeingReflected))
			{
				ScreenShake.Instance.Shake();
			}

			if(sounds.deathSound)
			{
				GetComponent<AudioSource>().PlayOneShot(sounds.deathSound);
			}

			StopAllCoroutines();
			StartCoroutine("DeathCoroutine");
		}

		protected IEnumerator DeathCoroutine()
		{
			yield return new WaitForSeconds(0.001f); //This prevents the collider from being disabled before other attached scripts like ContactDamage can take effect

			GetComponent<BoxCollider2D>().enabled = false;

			if(animations.deathAnimation != null)
			{
				slots.anim.Play(animations.deathAnimation.name);
				while(timeStop.isTimeStopped) yield return null;
				yield return new WaitForSeconds(animations.deathAnimation.length);
			}

			Clear();
		}

		protected void Reflect(Reflector reflector)
		{
			if(sounds.reflectSound)
			{
				GetComponent<AudioSource>().PlayOneShot(sounds.reflectSound);
			}

			Vector2 distanceFromCenter = new Vector2();
			distanceFromCenter.x = (transform.position.x - reflector.GetComponent<BoxCollider2D>().bounds.min.x) / (reflector.GetComponent<BoxCollider2D>().bounds.max.x - reflector.GetComponent<BoxCollider2D>().bounds.min.x) - 0.5f;
			distanceFromCenter.y = (transform.position.y - reflector.GetComponent<BoxCollider2D>().bounds.min.y) / (reflector.GetComponent<BoxCollider2D>().bounds.max.y - reflector.GetComponent<BoxCollider2D>().bounds.min.y) - 0.5f;

			if(distanceFromCenter.x > 0.5f)
			{
				distanceFromCenter.x = 0.5f;
			}
			else if(distanceFromCenter.x < -0.5f)
			{
				distanceFromCenter.x = -0.5f;
			}

			if(distanceFromCenter.y > 0.5f)
			{
				distanceFromCenter.y = 0.5f;
			}
			else if(distanceFromCenter.y < -0.5f)
			{
				distanceFromCenter.y= -0.5f;
			}

			movementSpeed.x = 0.0f;
			movementSpeed.y = 0.0f;

			float speed = 40.0f;
			if(reflector.orientation == Reflector.Orientation.Right || reflector.orientation == Reflector.Orientation.Left)
			{
				movementSpeed.x = speed;
				if(transform.position.x < reflector.transform.position.x) //Bullet is to the left of reflector
				{
					SetHorizontalDirection(Direction.Horizontal.Left);
				}
				else //Bullet is to the right of reflector
				{
					SetHorizontalDirection(Direction.Horizontal.Right);
				}

				if(reflection.willReflectAtAngles)
				{
					movementSpeed.x *= 1.0f - Mathf.Abs(distanceFromCenter.y);
					movementSpeed.y = speed * distanceFromCenter.y;
				}
			}
			else if(reflector.orientation == Reflector.Orientation.Up || reflector.orientation == Reflector.Orientation.Down)
			{
				SetHorizontalDirection(Direction.Horizontal.Right);

				if(transform.position.y > reflector.transform.position.y)
				{
					movementSpeed.y = speed;
					transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				}
				else
				{
					movementSpeed.y = -speed;
					transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
				}

				if(reflection.willReflectAtAngles)
				{
					movementSpeed.y *= 1.0f - Mathf.Abs(distanceFromCenter.x);
					movementSpeed.x = speed * distanceFromCenter.x;
				}
			}

			slots.physicsObject.SetVelocityY(movementSpeed.y);
			ContactDamage contactDamage = GetComponent<ContactDamage>();
			if(contactDamage != null)
			{
				contactDamage.willDamagePlayer = reflection.willDamagePlayerOnReflect;
				contactDamage.willDamageEnemies = reflection.willDamageEnemiesOnReflect;
			}

			willDestroyOnHit.enemy = reflection.reflectedDestroyOnHit.enemy;
			willDestroyOnHit.player = reflection.reflectedDestroyOnHit.player;

			slots.physicsObject.SetVelocityX((int)direction.horizontal * Mathf.Abs(movementSpeed.x));
			slots.physicsObject.SetVelocityY((int)direction.vertical * Mathf.Abs(movementSpeed.y));
		}

		protected void SetHorizontalDirection(Direction.Horizontal _direction)
		{
			direction.horizontal = _direction;
			if(direction.horizontal == Direction.Horizontal.Left)
			{
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
			else if(direction.horizontal == Direction.Horizontal.Right)
			{
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
		}

		protected void SetVerticalDirection(Direction.Vertical _direction)
		{
			direction.vertical = _direction;
			if(direction.vertical == Direction.Vertical.Down)
			{
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
			else if(direction.vertical == Direction.Vertical.Up)
			{
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
		}

		protected void FlipHorizontal()
		{
			if(direction.horizontal == Direction.Horizontal.Left)
			{
				SetHorizontalDirection(Direction.Horizontal.Right);
			}
			else if(direction.horizontal == Direction.Horizontal.Right)
			{
				SetHorizontalDirection(Direction.Horizontal.Left);
			}
		}

		protected void FlipVertical()
		{
			if(direction.vertical == Direction.Vertical.Down)
			{
				SetVerticalDirection(Direction.Vertical.Up);
			}
			else if(direction.vertical == Direction.Vertical.Up)
			{
				SetVerticalDirection(Direction.Vertical.Down);
			}
		}

		public override void OnPhysicsCollision(Collider2D col, Side side, CollisionType type)
		{
			if(side == Side.Right)
			{
				if(destroyOnTerrainCollision.onRight)
				{
					if(!isDead)
					{
						OnDeath();
					}
				}
				else if(ricochetOnTerrainCollision.onRight)
				{
					FlipHorizontal();
					framesMovedInCurrentHorizontalDirection = 0;
					if(acceleration == 0.0f)
					{
						slots.physicsObject.SetVelocityX((int)direction.horizontal * Mathf.Abs(movementSpeed.x));
					}
				}
			}
			else if(side == Side.Left)
			{
				if(destroyOnTerrainCollision.onLeft)
				{
					if(!isDead)
					{
						OnDeath();
					}
				}
				else if(ricochetOnTerrainCollision.onLeft)
				{
					FlipHorizontal();
					framesMovedInCurrentHorizontalDirection = 0;
					if(acceleration == 0.0f)
					{
						slots.physicsObject.SetVelocityX((int)direction.horizontal * Mathf.Abs(movementSpeed.x));
					}
				}
			}
			else if(side == Side.Top)
			{
				if(destroyOnTerrainCollision.onCeiling && PhysicsManager.Instance.gravityScale > 0.0f || destroyOnTerrainCollision.onFloor && PhysicsManager.Instance.gravityScale <= 0.0f)
				{
					if(!isDead)
					{
						OnDeath();
					}
				}
				else if(ricochetOnTerrainCollision.onCeiling && PhysicsManager.Instance.gravityScale > 0.0f || ricochetOnTerrainCollision.onFloor && PhysicsManager.Instance.gravityScale <= 0.0f)
				{
					FlipVertical();
					framesMovedInCurrentVerticalDirection = 0;
					if(acceleration == 0.0f)
					{
						slots.physicsObject.SetVelocityY((int)direction.vertical * Mathf.Abs(movementSpeed.y));
					}
				}
			}
			else if(side == Side.Bottom)
			{
				if(destroyOnTerrainCollision.onFloor && PhysicsManager.Instance.gravityScale > 0.0f || destroyOnTerrainCollision.onCeiling && PhysicsManager.Instance.gravityScale <= 0.0f)
				{
					if(!isDead)
					{
						OnDeath();
					}
				}
				else if(ricochetOnTerrainCollision.onFloor && PhysicsManager.Instance.gravityScale > 0.0f || ricochetOnTerrainCollision.onFloor && PhysicsManager.Instance.gravityScale <= 0.0f)
				{
					FlipVertical();
					framesMovedInCurrentVerticalDirection = 0;
					if(acceleration == 0.0f)
					{
						slots.physicsObject.SetVelocityY((int)direction.vertical * Mathf.Abs(movementSpeed.y));
					}
				}
			}
		}

		protected void ProcessCollision(Collider2D col)
		{
			if((col.tag == "Player" && willDestroyOnHit.player) || (col.tag == "Enemy" && willDestroyOnHit.enemy))
			{
				if(!isDead)
				{
					RexActor targetActor = col.gameObject.GetComponent<RexActor>();
					if(targetActor != null && spawningActor != null && targetActor == spawningActor) //Don't collide if this is the same actor that spawned it
					{
						return;
					}

					OnDeath();
				}
			}
			else if(col.gameObject.tag == "Reflector")
			{
				
				if(!isBeingReflected && reflection.willReflectOnReflectors)
				{
					Reflector reflector = col.GetComponent<Reflector>();
					if(!(reflector && spawningActor &&  reflector.actor == spawningActor))
					{
						Reflect(reflector);
						isBeingReflected = true;
						spawningActor = null;
					}
				}
				else if(!reflection.willReflectOnReflectors)
				{
					OnDeath();
				}
			}
		}

		protected void OnCollisionEnter2D(Collision2D col)
		{
			ProcessCollision(col.collider);
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			ProcessCollision(col);
		}

		#endregion

		void OnEnable()
		{
			OnSpawned();
		}

		void OnDisable()
		{

		}

		void OnDestroy()
		{
			if(parentSpawnPool != null)
			{
				parentSpawnPool.Despawn(gameObject);
			}
		}
	}
}