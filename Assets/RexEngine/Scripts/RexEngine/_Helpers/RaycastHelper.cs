/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class RaycastHelper:MonoBehaviour 
{
	public class LedgeInfo
	{
		public bool didHit;
		public float hitY;
	}

	public class StairsInfo
	{
		public bool didHit;
		public Direction.Horizontal stairDirection;
	}

	void Awake() 
	{
		
	}

	public static bool IsNextToLedge(Direction.Horizontal _direction, Direction.Vertical _verticalDirection, BoxCollider2D _collider, float gravityScaleMultiplier = 1.0f, float rayLength = 0.1f)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("PassThroughBottom");

		rayLength = (_collider.size.y * 0.5f) + rayLength;

		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y + (_collider.offset.y * gravityScaleMultiplier));

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = (_direction == Direction.Horizontal.Left) ? rayOrigin.x - raycastSpacing : rayOrigin.x + raycastSpacing;

		raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
		if(raycastHits.fraction > 0) 
		{
			isConnected = true;
		}

		#if UNITY_EDITOR
		Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
		#endif

		return !isConnected;
	}

	public static LedgeInfo DetectLedgeOnWall(Direction.Horizontal _direction, Direction.Vertical _verticalDirection, BoxCollider2D _collider, float velocityToCheck, float offset = 0.25f)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain");

		float rayLength = _collider.size.y + Mathf.Abs(velocityToCheck) + offset;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x + (_collider.size.x * 0.5f) * (int)_direction, _collider.transform.position.y - (_collider.size.y * 0.5f * (int)_verticalDirection));
		rayOrigin.y = (rayDirection == Vector2.up) ? _collider.bounds.min.y - offset : _collider.bounds.max.y + offset;

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = (_direction == Direction.Horizontal.Left) ? rayOrigin.x - raycastSpacing : rayOrigin.x + raycastSpacing;

		raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
		if(raycastHits.fraction > 0) 
		{
			isConnected = true;
		}

		#if UNITY_EDITOR
		//Debug.DrawRay(rayOrigin, new Vector2(0.25f * (int)_direction, (int)rayDirection.y * rayLength), Color.red);
		#endif

		float hitY = (isConnected) ? raycastHits.point.y : 0.0f;
		LedgeInfo ledgeInfo = new LedgeInfo();
		ledgeInfo.didHit = isConnected;
		ledgeInfo.hitY = hitY;

		return ledgeInfo;
	}

	public static bool IsOnSurface(string surfaceTag, Direction.Vertical _verticalDirection, BoxCollider2D _collider, float gravityScaleMultiplier = 1.0f)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("PassThroughBottom");

		float rayLength = _collider.size.y * 0.5f + 0.25f;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y + (_collider.offset.y * gravityScaleMultiplier));

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = rayOrigin.x - raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			if(raycastHits.fraction > 0) 
			{
				if(raycastHits.collider.tag == surfaceTag)
				isConnected = true;
			}

			#if UNITY_EDITOR
			//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
			#endif

			rayOrigin.x += raycastSpacing;
		}

		return isConnected;
	}

	public static StairsInfo IsOnStairs(Direction.Vertical _verticalDirection, BoxCollider2D _collider, bool isOnSurface, float gravityScaleMultiplier = 1.0f)
	{
		StairsInfo stairsInfo = new StairsInfo();
		if(isOnSurface)
		{
			return stairsInfo;
		}

		RaycastHit2D raycastHits = new RaycastHit2D(); 
		string surfaceTag = "Stairs";
		int collisionLayerMask = 1 << LayerMask.NameToLayer(surfaceTag); 

		float rayLength = Mathf.Abs(_collider.GetComponent<RexPhysics>().properties.velocity.y) * PhysicsManager.Instance.fixedDeltaTime;
		if(rayLength < 0.25f)
		{
			rayLength = 0.25f;
		}

		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y - (_collider.size.y * 0.5f * gravityScaleMultiplier) + (_collider.offset.y * gravityScaleMultiplier));

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = rayOrigin.x - raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			if(raycastHits.fraction > 0) 
			{
				if(raycastHits.collider.tag == surfaceTag)
				{
					isConnected = true;
					stairsInfo.didHit = true;
					stairsInfo.stairDirection = (Direction.Horizontal)raycastHits.transform.localScale.x;
				}
			}

			#if UNITY_EDITOR
			//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
			#endif

			rayOrigin.x += raycastSpacing;
		}

		if(isConnected)
		{
			float colliderBottom = _collider.transform.position.y - (_collider.size.y * 0.5f * gravityScaleMultiplier) + (_collider.offset.y * gravityScaleMultiplier);
			rayOrigin.x = _collider.transform.position.x - raycastSpacing;
			rayOrigin.y = _collider.transform.position.y + (_collider.size.y * 0.5f * gravityScaleMultiplier) + (_collider.offset.y * gravityScaleMultiplier);
			rayLength += _collider.size.y;

			for(int i = 0; i < 2; i ++)
			{
				raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
				if(raycastHits.fraction > 0) 
				{
					if(raycastHits.collider.tag == surfaceTag)
					{
						if((colliderBottom < raycastHits.point.y && gravityScaleMultiplier > 0.0f) || (colliderBottom > raycastHits.point.y && gravityScaleMultiplier < 0.0f))
						{
							isConnected = false;
							stairsInfo.didHit = false;
						}
					}
				}

				#if UNITY_EDITOR
				//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
				#endif

				rayOrigin.x += raycastSpacing * 2.0f;
			}
		}

		return stairsInfo;
	}

	public static StairsInfo IsAtTopOfStairs(Direction.Vertical _verticalDirection, Direction.Horizontal _horizontalDirection, BoxCollider2D _collider, bool isOnSurface, float gravityScaleMultiplier = 1.0f)
	{
		StairsInfo stairsInfo = new StairsInfo();
		if(!isOnSurface)
		{
			return stairsInfo;
		}

		float standingBuffer = 1.0f;
		string surfaceTag = "Stairs";
		RaycastHit2D raycastHits = new RaycastHit2D(); 
		int collisionLayerMask = 1 << LayerMask.NameToLayer(surfaceTag); 

		float rayLength = _collider.GetComponent<RexPhysics>().properties.velocity.y * PhysicsManager.Instance.fixedDeltaTime + standingBuffer;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		//bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x + _collider.size.x * 0.5f * (int)_horizontalDirection, _collider.transform.position.y - (_collider.size.y * 0.5f * gravityScaleMultiplier) + (_collider.offset.y * gravityScaleMultiplier));
		raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
		if(raycastHits.fraction > 0) 
		{
			if(raycastHits.collider.tag == surfaceTag)
			{
				//isConnected = true;
				stairsInfo.didHit = true;
				stairsInfo.stairDirection = (Direction.Horizontal)raycastHits.transform.localScale.x;
			}
		}

		#if UNITY_EDITOR
		//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
		#endif

		return stairsInfo;
	}

	public static StairsInfo IsAtFootOfStairs(Direction.Vertical _verticalDirection, Direction.Horizontal _horizontalDirection, BoxCollider2D _collider, bool isOnSurface, float gravityScaleMultiplier = 1.0f)
	{
		StairsInfo stairsInfo = new StairsInfo();
		if(!isOnSurface)
		{
			return stairsInfo;
		}

		float standingBuffer = 0.25f;
		string surfaceTag = "Stairs";
		RaycastHit2D raycastHits = new RaycastHit2D(); 
		int collisionLayerMask = 1 << LayerMask.NameToLayer(surfaceTag); 

		float rayLength = _collider.GetComponent<RexPhysics>().properties.velocity.y * PhysicsManager.Instance.fixedDeltaTime + standingBuffer;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		//bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x + _collider.size.x * 0.5f * (int)_horizontalDirection, _collider.transform.position.y - (_collider.size.y * 0.5f * gravityScaleMultiplier) + (_collider.offset.y * gravityScaleMultiplier) + (standingBuffer * gravityScaleMultiplier));
		raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
		if(raycastHits.fraction > 0) 
		{
			if(raycastHits.collider.tag == surfaceTag)
			{
				//isConnected = true;
				stairsInfo.didHit = true;
				stairsInfo.stairDirection = (Direction.Horizontal)raycastHits.transform.localScale.x;
			}
		}

		#if UNITY_EDITOR
		//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
		#endif

		return stairsInfo;
	}


	public static bool IsUnderOverhang(Direction.Vertical _verticalDirection, Vector2 _colliderSize, Vector3 _colliderPosition)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain");
		float sideBuffer = 0.025f; //This pulls the raycasts slightly in from the sides of the collider; it prevents the player from crouching NEXT TO an overhang and being unable to get back up

		float rayLength = _colliderSize.y * 0.5f + 0.25f;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_colliderPosition.x, _colliderPosition.y);

		float raycastSpacing = _colliderSize.x * 0.5f - sideBuffer;
		rayOrigin.x = rayOrigin.x - raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			if(raycastHits.fraction > 0) 
			{
				isConnected = true;
			}

			#if UNITY_EDITOR
			//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
			#endif

			rayOrigin.x += raycastSpacing;
		}

		return isConnected;
	}

	public static Collider2D GetColliderForSurface(string surfaceTag, Direction.Vertical _verticalDirection, BoxCollider2D _collider, float gravityScaleMultiplier = 1.0f)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("PassThroughBottom");

		float rayLength = _collider.size.y * 0.5f + 0.25f;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		//bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y + (_collider.offset.y * gravityScaleMultiplier));

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = rayOrigin.x - raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			if(raycastHits.fraction > 0) 
			{
				if(raycastHits.collider.tag == surfaceTag)
				{
					//isConnected = true;
					return raycastHits.collider;
				}
			}

			#if UNITY_EDITOR
			//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
			#endif

			rayOrigin.x += raycastSpacing;
		}

		return null;
	}

	public static List<GameObject> GetCollidingObject(string objectTag, Direction.Vertical _verticalDirection, BoxCollider2D _collider, float gravityScaleMultiplier = 1.0f)
	{
		List<GameObject> collidingObjects = new List<GameObject>();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Terrain");

		float rayLength = _collider.size.y * 0.5f + 0.25f;
		Vector2 rayDirection = (_verticalDirection == Direction.Vertical.Up) ? Vector2.up : Vector2.down;
		//bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y + (_collider.offset.y * gravityScaleMultiplier));

		float raycastSpacing = _collider.size.x * 0.5f;
		rayOrigin.x = rayOrigin.x - raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			RaycastHit2D[] allRaycastHits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			for(int j = 0; j < allRaycastHits.Length; j ++)
			{
				if(allRaycastHits[j].fraction > 0) 
				{
					if(allRaycastHits[j].collider.tag == objectTag)
					{
						//isConnected = true;
						collidingObjects.Add(allRaycastHits[j].collider.gameObject);
					}
				}
			}

			#if UNITY_EDITOR
			//Debug.DrawRay(rayOrigin, new Vector2(0.0f, (int)_verticalDirection) * rayLength, Color.red);
			#endif

			rayOrigin.x += raycastSpacing;
		}

		return collidingObjects;
	}

	public static bool DropThroughFloorRaycast(Direction.Vertical _verticalDirection, BoxCollider2D _collider, float gravityScaleMultiplier = 1.0f)
	{
		RaycastHit2D raycastHits = new RaycastHit2D();
		int collisionLayerMask = 1 << LayerMask.NameToLayer("PassThroughBottom") | 1 << LayerMask.NameToLayer("Stairs");

		Vector3 rayDirection = (_verticalDirection == Direction.Vertical.Down) ? Vector3.down : Vector3.up;
		bool isConnected = false;

		Vector2 rayOrigin = new Vector2(_collider.transform.position.x, _collider.transform.position.y + (_collider.offset.y * gravityScaleMultiplier));
		float raycastSpacing = _collider.size.x * 0.5f;
		float rayLength = _collider.size.y * 0.5f + 0.05f;
		rayOrigin.x -= raycastSpacing;

		for(int i = 0; i < 3; i ++)
		{
			raycastHits = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, collisionLayerMask);
			if(raycastHits.fraction > 0) 
			{
				isConnected = true;
			}

			//Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

			rayOrigin.x += rayLength;
		}

		return isConnected;
	}
}
