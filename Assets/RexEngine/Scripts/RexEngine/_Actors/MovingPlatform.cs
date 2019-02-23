/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	public class MovingPlatform:PhysicsMover 
	{
		public Vector2 moveSpeed = new Vector2(1.0f, 1.0f);
		public bool isMovementEnabled = true;
		public bool willStartWhenPlayerIsOnTop;
		public bool willTurnOnWallContact;
		public bool willTurnOnDistance;
		public Direction.Horizontal startingDirectionX = Direction.Horizontal.Right;
		public Direction.Vertical startingDirectionY = Direction.Vertical.Up;
		public RexPhysics physicsObject;
		public Transform maxMovePosition;
		public Transform minMovePosition;

		[HideInInspector]
		public Vector2 moveDistance;

		protected Vector2 minMovePositionVector;
		protected Vector2 maxMovePositionVector;

		[HideInInspector]
		public bool hasPlayerOnTop;

		protected bool isMoving;
		protected Direction.Horizontal directionX;
		protected Direction.Vertical directionY;
		protected Vector2 distanceMovedThisFrame;

		void Awake()
		{
			framesBeforeRemoval = 0;

			if(!physicsObject)
			{
				physicsObject = GetComponent<RexPhysics>();
			}

			if(minMovePosition)
			{
				minMovePositionVector = new Vector2(minMovePosition.position.x, minMovePosition.position.y);
			}

			if(maxMovePosition)
			{
				maxMovePositionVector = new Vector2(maxMovePosition.position.x, maxMovePosition.position.y);
			}

			if(willTurnOnDistance)
			{
				physicsObject.clamping.willClampX = true;
				physicsObject.clamping.min.x = minMovePositionVector.x;
				physicsObject.clamping.max.x = maxMovePositionVector.x;

				physicsObject.clamping.willClampY = true;
				physicsObject.clamping.min.y = minMovePositionVector.y;
				physicsObject.clamping.max.y = maxMovePositionVector.y;
			}
		}

		void Start()
		{
			directionX = startingDirectionX;
			directionY = startingDirectionY;
			if(!willStartWhenPlayerIsOnTop && isMovementEnabled)
			{
				StartMoving();
				physicsObject.SetVelocityY(moveSpeed.y * (int)directionY);
			}
		}

		void FixedUpdate()
		{
			if(isMoving && isMovementEnabled)
			{
				MoveHorizontal();
				MoveVertical();
			}

			if(willStartWhenPlayerIsOnTop && hasPlayerOnTop && !isMoving && isMovementEnabled)
			{
				StartMoving();
			}
		}

		public override void NotifyOfObjectOnTop(RexPhysics _physicsObject)
		{
			hasPlayerOnTop = true;
		}

		public override void MovePhysics(RexPhysics _physicsObject)
		{
			if(_physicsObject.IsOnSurface())
			{
				distanceMovedThisFrame = physicsObject.GetPositionChangeFromLastFrame();//new Vector2(physicsObject.properties.position.x - physicsObject.previousFrameProperties.position.x, physicsObject.properties.position.y - physicsObject.previousFrameProperties.position.y);
				_physicsObject.ApplyDirectTranslation(distanceMovedThisFrame);
			}
		}

		public Vector2 GetVelocity()
		{
			if(physicsObject)
			{
				return physicsObject.properties.velocity;
			}
			else
			{
				return new Vector2(0.0f, 0.0f);
			}
		}

		protected void StartMoving()
		{
			isMoving = true;
		}

		protected void MoveHorizontal()
		{
			if(moveSpeed.x == 0.0f)
			{
				return;
			}

			bool willTurn = false;
			if((physicsObject.DidHitEitherWallThisFrame() && willTurnOnWallContact))
			{
				willTurn = true;
			}
			else if(willTurnOnDistance && ((transform.position.x >= maxMovePositionVector.x && directionX == Direction.Horizontal.Right) || (transform.position.x <= minMovePositionVector.x && directionX == Direction.Horizontal.Left)))
			{
				willTurn = true;
			}

			if(willTurn)
			{
				directionX = (directionX == Direction.Horizontal.Left) ? Direction.Horizontal.Right : Direction.Horizontal.Left;
			}

			physicsObject.SetVelocityX(moveSpeed.x * (int)directionX);
		}

		protected void MoveVertical()
		{
			if(moveSpeed.y == 0.0f)
			{
				return;
			}

			bool willTurn = false;
			if((physicsObject.IsOnSurface() && willTurnOnWallContact))
			{
				willTurn = true;
			}
			else if(willTurnOnDistance && ((transform.position.y >= maxMovePositionVector.y && directionY == Direction.Vertical.Up) || (transform.position.y <= minMovePositionVector.y && directionY == Direction.Vertical.Down)))
			{
				willTurn = true;
			}

			if(willTurn)
			{
				directionY = (directionY == Direction.Vertical.Up) ? Direction.Vertical.Down : Direction.Vertical.Up;
			}

			physicsObject.SetVelocityY(moveSpeed.y * (int)directionY);
		}

		public void SetPosition(Vector2 position)
		{
			transform.position = position;

			if(physicsObject)
			{
				physicsObject.properties.position = position;
				physicsObject.previousFrameProperties.position = position;
			}
		}
	}

}
