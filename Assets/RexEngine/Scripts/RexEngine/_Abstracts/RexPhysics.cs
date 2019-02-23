/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace RexEngine
{
	public class RexPhysics:MonoBehaviour 
	{
		[System.Serializable]
		public class Gravity
		{
			[Tooltip("An individual setting for the gravity of this object. Higher numbers means this has more weight and falls faster..")]
			public float gravity = 1.0f;
			[Tooltip("The highest speed this can achieve while falling.")]
			public float maxFallSpeed = 15.0f;
			[Tooltip("Whether or not this object is pulled by gravity each frame.")]
			public bool usesGravity = true;

			[HideInInspector] //A lower gravity scale means objects jump higher and fall slower. A negative gravity scale means inverse gravity.
			public float gravityScale = 1.0f; //Don't set this directly; instead, set GravityScale on the PhysicsManager. 
		}

		[System.Serializable] //Properties are meant to be ReadOnly; you can set them via various public methods below
		public class Properties
		{
			public Vector2 velocity;
			public Vector2 externalVelocity;
			public Vector2 velocityCap;
			public Vector2 acceleration;
			public Vector2 deceleration;
			public Vector2 externalAcceleration;
			public Vector2 externalDeceleration;

			public bool isGrounded;
			public bool isAgainstCeiling;
			public bool isAgainstLeftWall;
			public bool isAgainstRightWall;

			public bool isFalling;
			public Vector2 position;
			public string surfaceTag;
			public string wallTag;
			public float slopeAngle;
		}

		[System.Serializable]
		public class RaycastAmounts
		{
			[Tooltip("The amount of raycasts this object uses for horizontal movement and collisions. Higher numbers are more precise but have more performance overhead.")]
			public int horizontal = 5;
			[Tooltip("The amount of raycasts this object uses for vertical movement and collisions. Higher numbers are more precise but have more performance overhead.")]
			public int vertical = 3;
			[Tooltip("If True, this object will not perform physics movements or calculations when out of the main camera view. Setting this to True helps with performance.")]
			public bool disablePhysicsWhenOffCamera = true;
			[Tooltip("Rarely, actors can clip through objects at the intersection of multiple colliders -- for example, when falling to the bottom of a Ladder whose collider touches the ground. Setting this to True acts as a safeguard against those situations, at the expense of slightly more performance overhead. It's recommended to set this to True for the player, and False for most enemies or other objects.")]
			public bool enableRedundantVerticalCollisions;
			[Tooltip("If True, this object's raycasts will be visualized in Scene view when played in the Unity Editor.")]
			public bool drawDebug;
		}

		[System.Serializable]
		public class Slopes
		{
			[Tooltip("Setting this to True allows this object to be more precise when moving on sloped terrain. It requires a slight amount of performance overhead.")]
			public bool enableDetailedSlopeCollisions;
			[Tooltip("The amount of raycasts this object uses for collisions on sloped terrain. Higher numbers are more precise but -- yup, you guessed it -- have more performance overhead.")]
			public int slopeRaycasts = 3;
			[Tooltip("The distance between slope raycasts.")]
			public float slopeRaycastIncrement = 0.1f;
			[Tooltip("The length of slope raycats.")]
			public float slopeRaycastDistance = 1.5f;
		}

		[System.Serializable]
		public class Advanced
		{
			[Tooltip("Settings for how this object uses raycasts.")]
			public RaycastAmounts raycastAmounts = new RaycastAmounts();
			[Tooltip("Settings for how this object interacts with sloped terrain.")]
			public Slopes slopes = new Slopes();
		}

		[Tooltip("If False, this object's RexPhysics will be disabled, and it won't move.")]
		public bool isEnabled = true;
		[Tooltip("If True, this object will not collide with terrain. In addition to letting it pass through walls, this can improve performance.")]
		public bool willIgnoreTerrain;
		[Tooltip("If a RexObject is slotted here, it will receive notifications for various events from this RexPhysics component, including when it collides with terrain and what side the collision took place on.")]
		public RexObject rexObject; 
		[Tooltip("Settings for how this object interacts with gravity.")]
		public Gravity gravitySettings;
		[Tooltip("If True, this object will not move horizontally.")]
		public bool freezeMovementX;
		[Tooltip("If True, this object will not move vertically.")]
		public bool freezeMovementY;
		[Tooltip("If True, this object will snap to the closest floor terrain during its Awake() method.")]
		public bool willSnapToFloorOnStart;
		[Tooltip("Advanced settings for how this object uses raycasts and interacts with sloped terrain.")]
		public Advanced advanced = new Advanced();

		//[HideInInspector]
		public Properties properties;

		[HideInInspector]
		public Properties previousFrameProperties;

		[HideInInspector]
		public List<PhysicsMoverInfo> physicsMovers;

		public Clamping clamping;

		[System.Serializable]
		public class PhysicsMoverInfo
		{
			public PhysicsMover physicsMover;
			public int framesBeforeRemoval;
		}

		[System.Serializable]
		public class Clamping
		{
			public bool willClampX = false;
			public bool willClampY = false;
			public Vector2 max;
			public Vector2 min;
		}

		private float slopeDetectionMargin = 1.0f; //Used in detection of slopes
		private BoxCollider2D boxCollider;
		private Vector2 singleFrameVelocityAddition;
		private bool isXMovementFrozenForSingleFrame;
		private bool isYMovementFrozenForSingleFrame;
		private bool isGravityFrozenForSingleFrame;
		private float slopeAngleBuffer = 10.0f;
		private int collisionLayerMask; //The Layers this can collide with
		private int collisionLayerMaskDown; //Use a separate mask for Down collisions to enable one-way platforms
		private Rect rectangle; //Internal use only
		private bool didHitSlopeThisFrame; //Internal use only
		private Direction.Horizontal bufferedHorizontalDirection; //Internal use only
		private	float slopeOffsetThisFrame;

		private AccelerationType accelerationType;
		private Vector2 attemptedVelocity;
		private VelocityType velocityXType;
		private VelocityType velocityYType;
		private Vector2 directTranslationThisFrame;

		public enum VelocityType
		{
			Unlocked,
			Locked
		}

		public enum AccelerationType
		{
			None,
			Decelerating,
			Accelerating
		}

		void Awake()
		{
			previousFrameProperties.position = transform.position;
			properties.position = transform.position;
			collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain");
			collisionLayerMaskDown = collisionLayerMask;
			FlagsHelper.Set(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer("PassThroughBottom"));

			if(gameObject.tag == "Player")
			{
				AddToCollisions("Boundaries");
			}

			if(rexObject == null)
			{
				rexObject = GetComponent<RexObject>();
			}

			physicsMovers = new List<PhysicsMoverInfo>();
		}

		void Start()
		{
			boxCollider = GetComponent<BoxCollider2D>();
			slopeDetectionMargin = boxCollider.size.y * 0.5f;

			if(willIgnoreTerrain)
			{
				RemoveFromCollisions("Terrain");
			}
		}

		//Registers this object with the PhysicsManager, which will allow it to move every frame
		void OnEnable()
		{
			PhysicsManager.Instance.RegisterPhysicsObject(this);
			properties.position = transform.position;
			previousFrameProperties.position = transform.position;
			SyncGravityScale();
		}

		void OnDisable()
		{
			PhysicsManager.Instance.UnregisterPhysicsObject(this);
		}

		#region public methods
		public void Reset()
		{
			previousFrameProperties.isGrounded = false;
			previousFrameProperties.isAgainstCeiling = false;
			previousFrameProperties.isAgainstLeftWall = false;
			previousFrameProperties.isAgainstRightWall = false;
			previousFrameProperties.isFalling = false;

			properties.isGrounded = false;
			properties.isAgainstCeiling = false;
			properties.isAgainstLeftWall = false;
			properties.isAgainstRightWall = false;
			properties.isFalling = false;
		}

		public void StopAllMovement()
		{
			properties.acceleration = Vector2.zero;
			SetVelocityX(0.0f);
			SetVelocityY(0.0f);
		}

		//Attempt to move by this velocity, taking acceleration and deceleration into account.
		public void MoveX(float _velocity)
		{
			velocityXType = VelocityType.Unlocked;
			attemptedVelocity.x = _velocity;
			if(_velocity != 0.0f)
			{
				properties.velocityCap.x = _velocity;
			}
		}

		//Attempt to move by this velocity, taking acceleration and deceleration into account
		public void MoveY(float _velocity)
		{
			velocityYType = VelocityType.Unlocked;
			attemptedVelocity.y = _velocity;
			if(_velocity != 0.0f)
			{
				properties.velocityCap.y = _velocity;
			}
		}

		//Directly sets the X velocity
		public void SetVelocityX(float newVelocity)
		{
			//MoveX(newVelocity);
			velocityXType = VelocityType.Locked;
			properties.velocity = new Vector2(newVelocity, properties.velocity.y);
		}	

		//Directly sets the Y velocity
		public void SetVelocityY(float newVelocity)
		{
			//MoveY(newVelocity);
			velocityYType = VelocityType.Locked;
			properties.velocity = new Vector2(properties.velocity.x, newVelocity);
		}

		//Add to velocity for a single frame
		public void ApplyForce(Vector2 _force)
		{
			singleFrameVelocityAddition = new Vector2(singleFrameVelocityAddition.x + _force.x, singleFrameVelocityAddition.y + _force.y);
		}

		public void AddToExternalVelocity(Vector2 _velocityToAdd)
		{
			properties.externalVelocity = new Vector2(properties.externalVelocity.x + _velocityToAdd.x, properties.externalVelocity.y + _velocityToAdd.y);
		}

		public void FreezeXMovementForSingleFrame()
		{
			isXMovementFrozenForSingleFrame = true;
		}

		public void FreezeYMovementForSingleFrame()
		{
			isYMovementFrozenForSingleFrame = true;
		}

		public void FreezeGravityForSingleFrame()
		{
			isGravityFrozenForSingleFrame = true;
		}

		public void ClearSingleFrameVelocity()
		{
			singleFrameVelocityAddition = Vector2.zero;
		}

		//Used for accleration. This is the highest speed the object can accelerate to
		public void SetAccelerationCapX(float velocityCapX)
		{
			properties.velocityCap = new Vector2(velocityCapX, properties.velocityCap.y);
		}

		public void SetAccelerationCapY(float velocityCapY)
		{
			properties.velocityCap = new Vector2(properties.velocityCap.x, velocityCapY);
		}

		public void ApplyDirectTranslation(Vector2 directTranslation)
		{
			directTranslationThisFrame += directTranslation;
		}

		//Adds a new Layer this object can collide with
		public void AddToCollisions(string layerName)
		{
			FlagsHelper.Set(ref collisionLayerMask, 1 << LayerMask.NameToLayer(layerName));
			FlagsHelper.Set(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer(layerName));

			//Handle Terrain and PassThroughBottom at the same time for ease of use
			if(layerName == "Terrain")
			{
				FlagsHelper.Set(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer("PassThroughBottom"));
			}
		}

		//Removes a Layer that this object could collide with
		public void RemoveFromCollisions(string layerName)
		{
			FlagsHelper.Unset(ref collisionLayerMask, 1 << LayerMask.NameToLayer(layerName));
			FlagsHelper.Unset(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer(layerName));

			//Handle Terrain and PassThroughBottom at the same time for ease of use
			if(layerName == "Terrain")
			{
				FlagsHelper.Unset(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer("PassThroughBottom"));
			}
		}

		//Disables this object from colliding with one-way platforms
		public void DisableOneWayPlatforms()
		{
			FlagsHelper.Unset(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer("PassThroughBottom"));
		}

		//Enables collision with one-way platforms
		public void EnableOneWayPlatforms()
		{
			FlagsHelper.Set(ref collisionLayerMaskDown, 1 << LayerMask.NameToLayer("PassThroughBottom"));
		}

		public void SyncGravityScale() //Called automatically by PhysicsManager when its gravityScale is changed
		{
			float newGravityScale = PhysicsManager.Instance.gravityScale;
			if(newGravityScale != gravitySettings.gravityScale && rexObject != null)
			{
				rexObject.OnGravityScaleChanged(newGravityScale);
			}

			gravitySettings.gravityScale = newGravityScale;
		}

		public float GravityScale()
		{
			return gravitySettings.gravityScale;
		}

		public string GetSurfaceTag()
		{
			if(IsOnSurface())
			{
				return properties.surfaceTag;
			}
			else
			{
				return null;
			}
		}

		public bool IsOnSurface() //Returns True if on the floor in normal gravity, or on the ceiling in reverse gravity
		{
			return (properties.isGrounded && gravitySettings.gravityScale > 0.0f) || (properties.isAgainstCeiling && gravitySettings.gravityScale < 0.0f);
		}

		public bool DidLandThisFrame()
		{
			return ((properties.isGrounded && !previousFrameProperties.isGrounded && gravitySettings.gravityScale > 0.0f) || (properties.isAgainstCeiling && !previousFrameProperties.isAgainstCeiling && gravitySettings.gravityScale <= 0.0f));
		}

		public bool DidHitCeilingThisFrame()
		{
			return ((properties.isGrounded && !previousFrameProperties.isGrounded && gravitySettings.gravityScale < 0.0f) || (properties.isAgainstCeiling && !previousFrameProperties.isAgainstCeiling && gravitySettings.gravityScale >= 0.0f));
			//return (properties.isAgainstCeiling && !previousFrameProperties.isAgainstCeiling);
		}

		public bool DidHitLeftWallThisFrame()
		{
			return (properties.isAgainstLeftWall && !previousFrameProperties.isAgainstLeftWall);
		}

		public bool DidHitRightWallThisFrame()
		{
			return (properties.isAgainstRightWall && !previousFrameProperties.isAgainstRightWall);
		}

		public bool DidHitEitherWallThisFrame()
		{
			return (DidHitLeftWallThisFrame() || DidHitRightWallThisFrame());
		}

		public bool IsAgainstEitherWall()
		{
			return (properties.isAgainstLeftWall || properties.isAgainstRightWall);
		}

		//Sets our current properties to our previous properties
		public void ResetFlags()
		{
			previousFrameProperties.velocity = properties.velocity;
			previousFrameProperties.externalVelocity = properties.externalVelocity;
			previousFrameProperties.velocityCap = properties.velocityCap;
			previousFrameProperties.acceleration = properties.acceleration;
			previousFrameProperties.deceleration = properties.deceleration;

			previousFrameProperties.isGrounded = properties.isGrounded;
			previousFrameProperties.isAgainstCeiling = properties.isAgainstCeiling;
			previousFrameProperties.isAgainstLeftWall = properties.isAgainstLeftWall;
			previousFrameProperties.isAgainstRightWall = properties.isAgainstRightWall;
			previousFrameProperties.isFalling = properties.isFalling;

			previousFrameProperties.position = properties.position;
		}

		//Checks in a direction to see if we hit a wall
		public bool CheckForWallContact(Direction.Horizontal _direction)
		{
			float margin = 0.01f;
			Vector2 direction = new Vector2((int)_direction, 0.0f);
			float rayLength = rectangle.width / 2 + margin;
			/*RaycastHit2D raycastHit = */DoHorizontalRaycasts(1.0f * (int)direction.x, false, direction, rayLength);
			bool didCollide = (direction == Vector2.right && properties.isAgainstRightWall) || (direction == Vector2.left && properties.isAgainstLeftWall);

			return didCollide;
		}

		//Checks to see if we hit either the floor or the ceiling
		public bool CheckForCeilingFloorContact(Direction.Vertical _direction, bool willDetectWithMargin = false)
		{
			float edgeBuffer = 0.025f;
			Vector2 startPoint = new Vector2(rectangle.xMin + edgeBuffer, rectangle.center.y);
			Vector2 endPoint = new Vector2(rectangle.xMax - edgeBuffer, rectangle.center.y);

			RaycastHit2D[] raycastHits = new RaycastHit2D[advanced.raycastAmounts.vertical];

			float margin = (willDetectWithMargin) ? 0.25f : 0.001f;
			float rayLength = rectangle.height / 2 + margin;
			//bool didCollide = false;

			for(int i = 0; i < advanced.raycastAmounts.vertical; i ++) 
			{
				float raycastSpacing = (float)i / (float)(advanced.raycastAmounts.vertical - 1);
				Vector2 rayOrigin = Vector2.Lerp(startPoint, endPoint, raycastSpacing);
				Vector2 rayDirection = (_direction == Direction.Vertical.Down) ? -Vector2.up : Vector2.up;
				int mask = (_direction == Direction.Vertical.Down) ? collisionLayerMaskDown : collisionLayerMask;
				raycastHits[i] = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, mask);
				if(raycastHits[i].fraction > 0) 
				{
					//didCollide = true;
					if(_direction == Direction.Vertical.Up)
					{
						properties.isAgainstCeiling = true;

						if(gravitySettings.gravityScale < 0.0f)
						{
							properties.surfaceTag = raycastHits[i].collider.tag;
						}

						return true;
					}
					else if(_direction == Direction.Vertical.Down)
					{
						properties.isGrounded = true;

						if(gravitySettings.gravityScale >= 0.0f)
						{
							properties.surfaceTag = raycastHits[i].collider.tag;
						}

						willSnapToFloorOnStart = false;
						return true;
					}

					break;
				}
			}

			return false;
		}

		//Immediately anchors this object to the nearest floor
		public void AnchorToFloor()
		{
			rectangle = GetColliderRect(properties.position);
			Direction.Vertical direction = (gravitySettings.gravityScale >= 0.0f) ? Direction.Vertical.Down : Direction.Vertical.Up;
			Vector2 startPoint = new Vector2(rectangle.xMin, rectangle.center.y);
			Vector2 endPoint = new Vector2(rectangle.xMax, rectangle.center.y);
			float rayLength = rectangle.height / 2 + 200.0f;

			RaycastHit2D raycastHit = DoVerticalRaycasts(startPoint, endPoint, rayLength, direction, advanced.raycastAmounts.vertical, collisionLayerMaskDown);
			if(raycastHit != null && raycastHit.fraction > 0.0f)
			{
				properties.position.y += (int)direction * (raycastHit.fraction * rayLength - rectangle.height / 2);
			}
		}

		public Vector2 GetPositionChangeFromLastFrame()
		{
			return new Vector2(properties.position.x - previousFrameProperties.position.x, properties.position.y - previousFrameProperties.position.y);
		}

		//The meat of RexPhysics. Moves the object and handles colliding with surfaces
		public void StepPhysics()
		{
			rectangle = GetColliderRect(properties.position);
			bool isOnCamera = CameraHelper.CameraContainsPoint(transform.position, 6.0f);
			bool willStepPhysics = (!isOnCamera && advanced.raycastAmounts.disablePhysicsWhenOffCamera) ? false : true;

			if(willStepPhysics)
			{
				ApplyPhysicsMovers();
				rectangle = GetColliderRect(properties.position);

				if(directTranslationThisFrame.x != 0.0f)
				{
					CheckHorizontalCollisions(directTranslationThisFrame.x, true);
					rectangle = GetColliderRect(properties.position);
				}

				if(directTranslationThisFrame.y != 0.0f)
				{
					CheckVerticalCollisions(directTranslationThisFrame.y, true);
					rectangle = GetColliderRect(properties.position);
				}

				accelerationType = AccelerationType.None;
				if(Mathf.Abs(attemptedVelocity.x) > 0.0f && (Mathf.Abs(properties.acceleration.x) > 0.0f || Mathf.Abs(properties.externalAcceleration.x) > 0.0f))
				{
					accelerationType = AccelerationType.Accelerating;
				}
				else if((Mathf.Abs(attemptedVelocity.x) == 0.0f && (Mathf.Abs(properties.deceleration.x) > 0.0f || Mathf.Abs(properties.externalDeceleration.x) > 0.0f)) && Mathf.Abs(properties.velocity.x) > 0.0f)
				{
					accelerationType = AccelerationType.Decelerating;
				}

				if(accelerationType == AccelerationType.Accelerating)
				{
					ApplyAccelerationX();
				}
				else if(accelerationType == AccelerationType.Decelerating)
				{
					ApplyDecelerationX();
				}
				else if(velocityXType != VelocityType.Locked)
				{
					properties.velocity.x = attemptedVelocity.x;
				}

				CheckHorizontalCollisions((properties.velocity.x + properties.externalVelocity.x + singleFrameVelocityAddition.x) * PhysicsManager.Instance.fixedDeltaTime);

				ApplyHorizontalVelocity();

				if(willSnapToFloorOnStart && advanced.raycastAmounts != null)
				{
					AnchorToFloor();
					willSnapToFloorOnStart = false;
				}
				else
				{
					if(gravitySettings.usesGravity && !isGravityFrozenForSingleFrame)
					{
						ApplyGravity();
					}
					else
					{
						accelerationType = AccelerationType.None;
						if(Mathf.Abs(attemptedVelocity.y) > 0.0f && (Mathf.Abs(properties.acceleration.y) > 0.0f || Mathf.Abs(properties.externalAcceleration.y) > 0.0f))
						{
							accelerationType = AccelerationType.Accelerating;
						}
						else if((Mathf.Abs(attemptedVelocity.y) == 0.0f && (Mathf.Abs(properties.deceleration.y) > 0.0f || Mathf.Abs(properties.externalDeceleration.y) > 0.0f)) && Mathf.Abs(properties.velocity.y) > 0.0f)
						{
							accelerationType = AccelerationType.Decelerating;
						}

						if(accelerationType == AccelerationType.Accelerating)
						{
							ApplyAccelerationY();
						}
						else if(accelerationType == AccelerationType.Decelerating)
						{
							ApplyDecelerationY();
						}
						else if(velocityYType != VelocityType.Locked)
						{
							properties.velocity.y = attemptedVelocity.y;
						}
					}

					CheckVerticalCollisions((properties.velocity.y + properties.externalVelocity.y) * PhysicsManager.Instance.fixedDeltaTime);

					ApplyVerticalVelocity();
				}
			}

			if(!IsOnSurface())
			{
				properties.slopeAngle = 0;
			}

			properties.externalVelocity = Vector2.zero;
			singleFrameVelocityAddition = Vector2.zero;
			directTranslationThisFrame = Vector2.zero;
			didHitSlopeThisFrame = false;
			slopeOffsetThisFrame = 0.0f;
			attemptedVelocity = Vector2.zero;

			isXMovementFrozenForSingleFrame = false;
			isYMovementFrozenForSingleFrame = false;
			isGravityFrozenForSingleFrame = false;
		}

		#endregion

		#region private methods

		private void ResetCollider()
		{
			BoxCollider2D tempcollider = GetComponent<BoxCollider2D>();
			if(tempcollider != null)
			{
				boxCollider = GetComponent<BoxCollider2D>();
			}

			slopeDetectionMargin = boxCollider.size.y * 0.5f;
		}

		//Called internally; applies acceleration for this frame
		private void ApplyAccelerationX()
		{
			float adjustedAcceleration = GetAdjustedAcceleration(properties.acceleration.x, properties.externalAcceleration.x);
			float newVelocityX = properties.velocity.x + adjustedAcceleration;

			if(properties.acceleration.x != 0.0f || properties.externalAcceleration.x != 0.0f)
			{
				newVelocityX = properties.velocity.x + adjustedAcceleration;
			}
			else
			{
				newVelocityX = attemptedVelocity.x;
			}

			bool usesController = false; 
			Direction.Horizontal controllerDirection = Direction.Horizontal.Right;
			if(rexObject && rexObject.slots.controller)
			{
				usesController = true;
				controllerDirection = rexObject.slots.controller.direction.horizontal;
			}

			if((usesController && controllerDirection == Direction.Horizontal.Right) || properties.velocityCap.x > 0)
			{
				if(Mathf.Abs(newVelocityX) > Mathf.Abs(properties.velocityCap.x))
				{
					newVelocityX = properties.velocityCap.x;
				}
			}
			else if((usesController && controllerDirection == Direction.Horizontal.Left) || properties.velocityCap.x < 0)
			{
				if(Mathf.Abs(newVelocityX) > Mathf.Abs(properties.velocityCap.x))
				{
					newVelocityX = properties.velocityCap.x;
				}
			}

			properties.velocity = new Vector2(newVelocityX, properties.velocity.y);
		}

		//Called internally; applies acceleration for this frame
		private void ApplyAccelerationY()
		{
			float adjustedAcceleration = GetAdjustedAcceleration(properties.acceleration.y, properties.externalAcceleration.y);
			float newVelocityY = properties.velocity.y + adjustedAcceleration;

			if(properties.velocityCap.y > 0)
			{
				if(Mathf.Abs(newVelocityY) > Mathf.Abs(properties.velocityCap.y))
				{
					newVelocityY = properties.velocityCap.y;
				}
			}
			else if(properties.velocityCap.y < 0)
			{
				if(Mathf.Abs(newVelocityY) > Mathf.Abs(properties.velocityCap.y))
				{
					newVelocityY = properties.velocityCap.y;
				}
			}

			properties.velocity = new Vector2(properties.velocity.x, newVelocityY);
		}

		//Called internally; applies deceleration for this frame
		private void ApplyDecelerationX()
		{
			float newVelocityX = 0;
			float adjustedDeceleration = GetAdjustedAcceleration(properties.deceleration.x, properties.externalDeceleration.x);

			if(properties.velocity.x > 0)
			{
				newVelocityX = properties.velocity.x - adjustedDeceleration;
				if(newVelocityX < 0)
				{
					newVelocityX = 0;
				}
			}
			else if(properties.velocity.x < 0)
			{
				newVelocityX = properties.velocity.x + adjustedDeceleration;
				if(newVelocityX > 0)
				{
					newVelocityX = 0;
				}
			}

			SetVelocityX(newVelocityX);
		}

		//Called internally; applies deceleration for this frame
		private void ApplyDecelerationY()
		{
			float newVelocityY = 0;
			float adjustedDeceleration = GetAdjustedAcceleration(properties.deceleration.y, properties.externalDeceleration.y);

			if(properties.velocity.y > 0)
			{
				newVelocityY = properties.velocity.y - adjustedDeceleration;
				if(newVelocityY < 0)
				{
					newVelocityY = 0;
				}
			}
			else if(properties.velocity.y < 0)
			{
				newVelocityY = properties.velocity.y + adjustedDeceleration;
				if(newVelocityY > 0)
				{
					newVelocityY = 0;
				}
			}

			SetVelocityY(newVelocityY);
		}

		private float GetAdjustedAcceleration(float _deceleration, float _externalDeceleration)
		{
			float adjustedDeceleration = 0.0f;
			if(_deceleration != 0.0f)
			{
				adjustedDeceleration = _deceleration;
			}
			else if(_externalDeceleration != 0.0f)
			{
				adjustedDeceleration = _externalDeceleration;
			}

			if(_deceleration != 0.0f && _externalDeceleration != 0.0f) //Use whichever deceleration is smaller for the greatest impact
			{
				adjustedDeceleration = (Mathf.Abs(_deceleration) < Mathf.Abs(_externalDeceleration)) ? _deceleration : _externalDeceleration;
			}

			return adjustedDeceleration;
		}

		//Called internally; applies gravity this frame
		private void ApplyGravity()
		{
			if(Mathf.Abs(singleFrameVelocityAddition.y) > 0.0f)
			{
				properties.velocity = new Vector2(properties.velocity.x, singleFrameVelocityAddition.y - (gravitySettings.gravity * gravitySettings.gravityScale));
			}
			else
			{
				float currentFallSpeed = (gravitySettings.gravityScale >= 0.0f) ? Mathf.Max(-gravitySettings.maxFallSpeed, properties.velocity.y - (gravitySettings.gravity * gravitySettings.gravityScale)) : Mathf.Min(gravitySettings.maxFallSpeed, properties.velocity.y - (gravitySettings.gravity * gravitySettings.gravityScale));
				properties.velocity = new Vector2(properties.velocity.x, currentFallSpeed);
			}

			if((properties.velocity.y < 0 && gravitySettings.gravityScale > 0 && !properties.isGrounded) || (properties.velocity.y > 0 && gravitySettings.gravityScale < 0 && !properties.isAgainstCeiling)) 
			{
				properties.isFalling = true;
			}
		}

		//Checks our horizontal collisions, and either stops us from moving or enables us to continue moving
		private void CheckHorizontalCollisions(float velocityToCheck, bool isTranslatingDirectly = false)
		{
			properties.isAgainstLeftWall = false;
			properties.isAgainstRightWall = false;
			rectangle = GetColliderRect(properties.position);
			Vector2 direction = (velocityToCheck > 0) ? Vector2.right : -Vector2.right;

			if(willIgnoreTerrain)
			{
				return;
			}

			if(velocityToCheck != 0) 
			{        
				bufferedHorizontalDirection = direction.x > 0 ? Direction.Horizontal.Right : Direction.Horizontal.Left;

				float rayLength = rectangle.width / 2 + Mathf.Abs(velocityToCheck);
				RaycastHit2D raycastHit = DoHorizontalRaycasts(velocityToCheck, isTranslatingDirectly, direction, rayLength);

				bool didCollide = (direction == Vector2.right && properties.isAgainstRightWall) || (velocityToCheck < 0.0f && properties.isAgainstLeftWall);

				if(didCollide)
				{
					properties.position.x = properties.position.x + (direction.x * (raycastHit.fraction * rayLength - rectangle.width / 2.0f));
					properties.velocity.x = properties.externalVelocity.x = 0.0f;

					RexObject.Side side = (direction == Vector2.right) ? RexObject.Side.Right : RexObject.Side.Left;
					if(side == RexObject.Side.Left && DidHitLeftWallThisFrame() && velocityToCheck < 0.0f)
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Left, RexObject.CollisionType.Enter);
					}
					else if(side == RexObject.Side.Right && DidHitRightWallThisFrame() && velocityToCheck > 0.0f)
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Right, RexObject.CollisionType.Enter);
					}
				}

				if(!didCollide && isTranslatingDirectly)
				{
					properties.position = new Vector2(properties.position.x + velocityToCheck, properties.position.y);
				}
			}
		}

		private RaycastHit2D DoHorizontalRaycasts(float velocityToCheck, bool isTranslatingDirectly, Vector2 direction, float rayLength)
		{
			//edgebuffer prevents unintended horizontal collisions with a MovingPlatform you're riding on top of
			float edgeBuffer = (isTranslatingDirectly) ? 0.05f : 0.005f;
			Vector2 startPoint = new Vector2(rectangle.center.x, rectangle.center.y - rectangle.height * 0.5f + edgeBuffer);
			Vector2 endPoint = new Vector2(rectangle.center.x, rectangle.center.y + rectangle.height * 0.5f - edgeBuffer);

			RaycastHit2D[] raycastHits = new RaycastHit2D[advanced.raycastAmounts.horizontal];
			float fraction = 0.0f;

			//bool didCollide = false;
			float farthestFraction = 0.0f;
			int farthestRaycast = 0;

			for(int i = 0; i < advanced.raycastAmounts.horizontal; i ++) 
			{
				float raycastSpacing = (float)i / (float)(advanced.raycastAmounts.horizontal - 1);
				Vector2 rayOrigin = (advanced.raycastAmounts.horizontal > 1) ?  Vector2.Lerp(startPoint, endPoint, raycastSpacing) : rectangle.center;

				if(advanced.slopes.enableDetailedSlopeCollisions)
				{
					if(i == 1)
					{
						rayOrigin = Vector2.Lerp(startPoint, endPoint, ((float)(i - 1) / (float)(advanced.raycastAmounts.horizontal - 1)) + 0.005f);
					}
					else if(i == advanced.raycastAmounts.horizontal - 2)
					{
						rayOrigin = Vector2.Lerp(startPoint, endPoint, ((float)(i + 1) / (float)(advanced.raycastAmounts.horizontal - 1)) - 0.005f);
					}
				}

				raycastHits[i] = Physics2D.Raycast(rayOrigin, direction, rayLength, collisionLayerMask);

				if(advanced.raycastAmounts.drawDebug){Debug.DrawRay(rayOrigin, direction * rayLength, Color.red);}

				if(raycastHits[i].fraction > 0) 
				{
					if(fraction > 0 || advanced.raycastAmounts.horizontal < 2) 
					{
						float slopeAngle = (advanced.raycastAmounts.horizontal > 1) ? Vector2.Angle(raycastHits[i].point - raycastHits[i - 1].point, Vector2.right) : 90.0f;
						if(Mathf.Abs(slopeAngle - 90) < slopeAngleBuffer) //If the slope is too steep, treat it as a wall
						{
							if(direction == Vector2.right)
							{
								properties.isAgainstRightWall = true;
								properties.wallTag = raycastHits[i].collider.tag;
							}
							else if(velocityToCheck < 0.0f)
							{
								properties.isAgainstLeftWall = true;
								properties.wallTag = raycastHits[i].collider.tag;
							}

							if(raycastHits[i].fraction > farthestFraction)
							{
								farthestFraction = raycastHits[i].fraction;
								farthestRaycast = i;
							}
						}
						else
						{
							didHitSlopeThisFrame = true;
						}
					}

					fraction = raycastHits[i].fraction;
				}
			}
				
			return raycastHits[farthestRaycast];
		}

		//Checks our vertical collisions, and either stops us from moving or enables us to continue moving
		private void CheckVerticalCollisions(float velocityToCheck, bool isTranslatingDirectly = false)
		{
			rectangle = GetColliderRect(properties.position);
			Direction.Vertical direction = (velocityToCheck <= 0.0f) ? Direction.Vertical.Down : Direction.Vertical.Up;

			if(willIgnoreTerrain || (velocityToCheck == 0.0f && !advanced.slopes.enableDetailedSlopeCollisions))
			{
				return;
			}

			//The edge buffer prevents you from standing on a ledge with the very corner of your hitrectangle
			//This fixes problems where you jump straight up *next to* a ledge and the end up resting on the ledge itself on the way down
			float edgeBuffer = 0.001f;
			Vector2 startPoint = new Vector2(rectangle.xMin + edgeBuffer, rectangle.center.y);
			Vector2 endPoint = new Vector2(rectangle.xMax - edgeBuffer, rectangle.center.y);

			float rayLength = rectangle.height / 2.0f + ((((properties.isGrounded && direction == Direction.Vertical.Down) || (properties.isAgainstCeiling && direction == Direction.Vertical.Up)) && velocityToCheck != 0.0f) ? slopeDetectionMargin : Mathf.Abs(velocityToCheck));
			bool didCollide = false;
			RaycastHit2D raycastHit;
			if(didHitSlopeThisFrame && advanced.slopes.enableDetailedSlopeCollisions)
			{
				raycastHit = ApplySlopeOffset(-1.0f);
				didCollide = (raycastHit != null && raycastHit.fraction > 0.0f);

				if(!didCollide)
				{
					raycastHit = ApplySlopeOffset(1.0f);
					didCollide = (raycastHit != null && raycastHit.fraction > 0.0f);
				}
			}

			raycastHit = DoVerticalRaycasts(startPoint, endPoint, rayLength, direction, advanced.raycastAmounts.vertical, collisionLayerMaskDown);
			didCollide = (raycastHit != null && raycastHit.fraction > 0.0f);

			//This prevents you from walking off a one-way platform, holding left or right to be back inside it as you fall, and clipping back up on top of it
			if(didCollide && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("PassThroughBottom"))
			{
				if(!isTranslatingDirectly)
				{
					didCollide = IsRaycastHitOutsideCollider(raycastHit);
				}
			}

			//In rare instances, if the actor is already on top of a PassThroughBottom collider when they land on the ground, the above raycast can fail to detect the terrain; this is a failsafe
			if(!didCollide && advanced.raycastAmounts.enableRedundantVerticalCollisions)
			{
				int adjustedVerticalRaycasts = Mathf.CeilToInt(advanced.raycastAmounts.vertical * 0.5f);
				raycastHit = DoVerticalRaycasts(startPoint, endPoint, rayLength, direction, adjustedVerticalRaycasts, collisionLayerMask);
				didCollide = (raycastHit != null && raycastHit.fraction > 0.0f);
			}

			if(didCollide) 
			{
				properties.velocity.y = properties.externalVelocity.y = 0.0f;

				if(direction == Direction.Vertical.Down)
				{
					properties.isGrounded = true; 
					if(raycastHit && raycastHit.collider != null)
					{
						if(gravitySettings.gravityScale >= 0.0f)
						{
							properties.surfaceTag = raycastHit.collider.tag;
						}
					}

					willSnapToFloorOnStart = false;
					properties.isFalling = false;
					properties.isAgainstCeiling = false;

					if(advanced.slopes.enableDetailedSlopeCollisions)
					{
						properties.slopeAngle = GetSlopeAngle(direction, rayLength);
					}

					if(slopeOffsetThisFrame == 0.0f) //Don't do the below again if this already was repositioned to a slope this frame
					{
						Vector2 slopeMoveDistance = Vector2.down * (raycastHit.fraction * rayLength - rectangle.height / 2);
						if(IsStuckToSlope(velocityToCheck)) //This prevents you from sticking to downward-facing slopes if your downward velocity is stopped
						{
							properties.position += slopeMoveDistance;
						}
					}
				}
				else
				{
					properties.isAgainstCeiling = true;
					if(raycastHit && raycastHit.collider != null)
					{
						if(gravitySettings.gravityScale < 0.0f)
						{
							properties.surfaceTag = raycastHit.collider.tag;
						}
					}

					if(gravitySettings.gravityScale > 0.0f) //Hit your head on the ceiling; stop your jump
					{
						properties.isFalling = true;
					}
					else
					{
						properties.isFalling = false;
					}

					properties.position += Vector2.up * (raycastHit.fraction * rayLength - rectangle.height / 2);
				}

				if(DidLandThisFrame())
				{
					if(gravitySettings.gravityScale >= 0.0f)
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Bottom, RexObject.CollisionType.Enter);
					}
					else
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Top, RexObject.CollisionType.Enter);
					}
				}

				if(DidHitCeilingThisFrame())
				{
					if(gravitySettings.gravityScale >= 0.0f)
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Top, RexObject.CollisionType.Enter);
					}
					else
					{
						NotifyOfCollision(raycastHit.collider, RexObject.Side.Bottom, RexObject.CollisionType.Enter);
					}
				}
			} 
			else 
			{
				properties.isAgainstCeiling = false;
				properties.isGrounded = false;

				if(isTranslatingDirectly)
				{
					properties.position = new Vector2(properties.position.x, properties.position.y + velocityToCheck);
				}
			}
		}

		private bool IsRaycastHitOutsideCollider(RaycastHit2D raycastHit)
		{
			bool isOutsideCollider = true;
			float buffer = 0.1f;
			if((gravitySettings.gravityScale > 0.0f && rectangle.yMin < raycastHit.collider.bounds.max.y - buffer) || (gravitySettings.gravityScale <= 0.0f && rectangle.yMax > raycastHit.collider.bounds.min.y + buffer))
			{
				isOutsideCollider = false;
			}

			return isOutsideCollider;
		}

		private RaycastHit2D DoVerticalRaycasts(Vector2 startPoint, Vector2 endPoint, float rayLength, Direction.Vertical direction, int numberOfRaycasts, int mask)
		{
			RaycastHit2D[] raycastHits = new RaycastHit2D[numberOfRaycasts];
			float closestFraction = Mathf.Infinity;
			int closestRaycast = 0;

			for(int i = 0; i < numberOfRaycasts; i ++) 
			{
				float raycastSpacing = (float)i / (float)(advanced.raycastAmounts.vertical - 1);
				Vector2 rayOrigin = (advanced.raycastAmounts.vertical > 1) ? Vector2.Lerp(startPoint, endPoint, raycastSpacing) : rectangle.center;
				Vector2 rayDirection = (direction == Direction.Vertical.Down) ? -Vector2.up : Vector2.up;

				if(advanced.raycastAmounts.drawDebug){Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);}

				raycastHits[i] = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, mask);
				if(raycastHits[i].fraction > 0.0f) 
				{
					if(raycastHits[i].fraction < closestFraction) 
					{
						closestRaycast = i;
						closestFraction = raycastHits[i].fraction;

						PhysicsMover _physicsMover = (raycastHits[i] && raycastHits[i].collider != null && raycastHits[i].collider.tag == "PhysicsMover") ? raycastHits[i].collider.GetComponent<PhysicsMover>() : null;
						if(_physicsMover != null)
						{
							if(IsRaycastHitOutsideCollider(raycastHits[i]))
							{
								AddPhysicsMover(_physicsMover);
							}
						}
					}
				}
			}

			return raycastHits[closestRaycast];
		}

		private RaycastHit2D ApplySlopeOffset(float _verticalDirection)
		{
			RaycastHit2D newRaycastHit = new RaycastHit2D();
			int totalRays = advanced.slopes.slopeRaycasts;
			float increment = advanced.slopes.slopeRaycastIncrement;
			float newRayLength = advanced.slopes.slopeRaycastDistance; //TODO: Needs to change size depending on horizontal speed
			float verticalDirection = _verticalDirection;
			for(int i = 0; i < totalRays; i ++)
			{
				int horizontalDirection = bufferedHorizontalDirection == Direction.Horizontal.Right ? 1 : -1;
				float rectangleCorner = (verticalDirection == -1.0f) ? rectangle.min.y : rectangle.max.y;
				Vector2 newRaycastOrigin = new Vector2(properties.position.x - (0.001f * horizontalDirection) + (rectangle.size.x / 2 * horizontalDirection) - (i * increment * horizontalDirection), rectangleCorner + (newRayLength * -verticalDirection));

				newRaycastHit = Physics2D.Raycast(newRaycastOrigin, new Vector2(0.0f, verticalDirection), newRayLength, collisionLayerMask);

				if(advanced.raycastAmounts.drawDebug){Debug.DrawRay(newRaycastOrigin, new Vector2(0.0f, verticalDirection) * newRayLength, Color.blue);}

				if(newRaycastHit.fraction > 0)
				{
					slopeOffsetThisFrame = properties.position.y - (newRaycastHit.point.y + (rectangle.size.y / 2 * -verticalDirection));
					properties.position.y = newRaycastHit.point.y - boxCollider.offset.y + (rectangle.size.y / 2 * -verticalDirection);

					return newRaycastHit;
				}
			}

			return newRaycastHit;
		}

		private float GetSlopeAngle(Direction.Vertical direction, float rayLength)
		{
			float slopeAngle = 0.0f;
			Vector2 rayDirection = (direction == Direction.Vertical.Down) ? -Vector2.up : Vector2.up;
			RaycastHit2D[] slopeRaycastHits = new RaycastHit2D[4];
			int mask = ((direction == Direction.Vertical.Down && gravitySettings.gravityScale > 0.0f) || (direction == Direction.Vertical.Up && gravitySettings.gravityScale <= 0.0f)) ? collisionLayerMaskDown : collisionLayerMask;
			float margin = 0.01f;
			float edgeBuffer = 0.001f;
			Vector2 startPoint = new Vector2(rectangle.xMin + edgeBuffer, rectangle.center.y);
			Vector2 endPoint = new Vector2(rectangle.xMax - edgeBuffer, rectangle.center.y);
			Vector2 rayOrigin = startPoint;

			for(int i = 0; i < 4; i ++) 
			{
				if(i == 1)
				{
					rayOrigin = new Vector2(startPoint.x + margin, startPoint.y);
				}
				else if(i == 2)
				{
					rayOrigin = new Vector2(endPoint.x - margin, endPoint.y);
				}
				else if(i == 3)
				{
					rayOrigin = endPoint;
				}

				if(advanced.raycastAmounts.drawDebug){Debug.DrawRay(rayOrigin, rayDirection * rayLength * 0.6f, Color.red);}

				slopeRaycastHits[i] = Physics2D.Raycast(rayOrigin, rayDirection, rayLength * 0.6f, mask);
				if(slopeRaycastHits[i].fraction > 0) 
				{
					if(i > 0)
					{
						slopeAngle = RexMath.AngleFromPoint(slopeRaycastHits[i - 1].point, slopeRaycastHits[i].point);
					}
				}
			}

			//bool isOnFlatGround = false;
			for(int i = 0; i < slopeRaycastHits.Length; i ++)
			{
				if(i > 0 && slopeRaycastHits[i].fraction == slopeRaycastHits[i - 1].fraction && slopeRaycastHits[i].fraction > 0.0f)
				{
					//isOnFlatGround = true;
					slopeAngle = 0.0f;
					break;
				}
			}

			return slopeAngle;
		}

		private bool IsStuckToSlope(float velocityToCheck)
		{
			bool isStuckToSlope = true;
			if(velocityToCheck >= 0.0f)
			{
				Direction.Horizontal horizontalMovement = (properties.position.x >= transform.position.x) ? Direction.Horizontal.Right : Direction.Horizontal.Left;
				if(properties.slopeAngle > 0.0f && properties.slopeAngle < 90.0f) //45 degrees up-right slope
				{
					if(horizontalMovement == Direction.Horizontal.Left || properties.position.x == transform.position.x)
					{
						isStuckToSlope = false;
					}
				}
				else if(properties.slopeAngle < 0.0f && properties.slopeAngle > -90.0f) //-45 degrees up-left slope
				{
					if(horizontalMovement == Direction.Horizontal.Right)
					{
						isStuckToSlope = false;
					}
				}
			}

			return isStuckToSlope;
		}

		private void AddPhysicsMover(PhysicsMover _physicsMover)
		{
			int physicsMoverIndex = 0;
			bool isPhysicsMoverAlreadyInList = false;
			for(int i = 0; i < physicsMovers.Count; i ++)
			{
				if(physicsMovers[i].physicsMover == _physicsMover)
				{
					isPhysicsMoverAlreadyInList = true;
					physicsMoverIndex = i;
					break;
				}
			}

			if(!isPhysicsMoverAlreadyInList)
			{
				//Debug.Log("Add Physics Mover");
				PhysicsMoverInfo physicsMoverInfo = new PhysicsMoverInfo();
				physicsMoverInfo.physicsMover = _physicsMover;
				physicsMoverInfo.framesBeforeRemoval = _physicsMover.framesBeforeRemoval;
				physicsMovers.Add(physicsMoverInfo);
				_physicsMover.NotifyOfObjectOnTop(this);
			}
			else
			{
				physicsMovers[physicsMoverIndex].framesBeforeRemoval =  _physicsMover.framesBeforeRemoval;
			}
		}

		//If we have a rexObject slotted, this will notify it of any collisions and enable it to act accordingly
		private void NotifyOfCollision(Collider2D col, RexObject.Side side, RexObject.CollisionType collisionType)
		{
			if(col == null)
			{
				return;
			}

			if(rexObject != null)
			{
				rexObject.OnPhysicsCollision(col, side, collisionType);
			}

			RexObject otherObject = col.GetComponent<RexObject>();
			if(otherObject != null)
			{
				RexObject.Side otherSide;
				if(side == RexObject.Side.Bottom)
				{
					otherSide = RexObject.Side.Top;
				}
				else if(side == RexObject.Side.Top)
				{
					otherSide = RexObject.Side.Bottom;
				}
				else if(side == RexObject.Side.Left)
				{
					otherSide = RexObject.Side.Right;
				}
				else
				{
					otherSide = RexObject.Side.Left;
				}

				otherObject.NotifyOfCollisionWithPhysicsObject(boxCollider, otherSide, collisionType);
			}
		}

		private void ApplyPhysicsMovers()
		{
			for(int i = physicsMovers.Count - 1; i >= 0; i --)
			{
				if(physicsMovers[i].physicsMover == null || physicsMovers[i].framesBeforeRemoval < 0)
				{
					physicsMovers[i].physicsMover.OnRemove(this);
					physicsMovers.RemoveAt(i);
				}
				else
				{
					physicsMovers[i].physicsMover.MovePhysics(this);
					physicsMovers[i].framesBeforeRemoval --;
				}
			}
		}

		//At the end of a frame, moves us the appropriate amount on the X axis
		private void ApplyHorizontalVelocity()
		{
			if(!freezeMovementX && !isXMovementFrozenForSingleFrame)
			{
				properties.position = new Vector2(properties.position.x + ((properties.velocity.x + properties.externalVelocity.x + singleFrameVelocityAddition.x) * PhysicsManager.Instance.fixedDeltaTime), properties.position.y);

				if(clamping.willClampX)
				{
					if(properties.position.x > clamping.max.x)
					{
						properties.position.x = clamping.max.x;
					}
					else if(properties.position.x < clamping.min.x)
					{
						properties.position.x = clamping.min.x;
					}
				}
			}
		}

		//At the end of a frame, moves us the appropriate amount on the Y axis
		private void ApplyVerticalVelocity()
		{
			if(!freezeMovementY && !isYMovementFrozenForSingleFrame)
			{
				properties.position = new Vector2(properties.position.x, properties.position.y + ((properties.velocity.y + properties.externalVelocity.y) * PhysicsManager.Instance.fixedDeltaTime));

				if(clamping.willClampY)
				{
					if(properties.position.y > clamping.max.y)
					{
						properties.position.y = clamping.max.y;
					}
					else if(properties.position.y < clamping.min.y)
					{
						properties.position.y = clamping.min.y;
					}
				}
			}
		}

		//Used internally for figuring out collisions
		private Rect GetColliderRect(Vector3 position)
		{
			if(boxCollider != null)
			{
				float scaleMultiplierX = (boxCollider.transform.localScale.x <= 0.0f) ? -1.0f : 1.0f;
				float scaleMultiplierY = (boxCollider.transform.localScale.y <= 0.0f) ? -1.0f : 1.0f;
				return new Rect(position.x + (boxCollider.offset.x * scaleMultiplierX) - boxCollider.size.x / 2, position.y + (boxCollider.offset.y * scaleMultiplierY) - boxCollider.size.y / 2, boxCollider.size.x, boxCollider.size.y);
			}
			else
			{
				return new Rect(0, 0, 1, 1);
			}
		}

		#endregion
	}

}
