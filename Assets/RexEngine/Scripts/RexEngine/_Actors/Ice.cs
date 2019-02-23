using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class Ice:PhysicsMover
	{
		public float acceleration = 0.25f;
		public float deceleration = 0.1f;

		void Awake() 
		{
			framesBeforeRemoval = 0;
		}

		public override void MovePhysics(RexPhysics _physicsObject)
		{
			float inputDirection = 1.0f;
			if(_physicsObject && _physicsObject.rexObject && _physicsObject.rexObject.slots.controller)
			{
				inputDirection = (int)_physicsObject.rexObject.slots.controller.direction.horizontal;
				if(_physicsObject.rexObject.slots.controller.StateID() == DashState.idString)
				{
					return;
				}
			}

			_physicsObject.properties.externalAcceleration.x = acceleration * inputDirection;
			_physicsObject.properties.externalDeceleration.x = deceleration;
		}

		public override void OnRemove(RexPhysics _physicsObject)
		{
			if(_physicsObject.physicsMovers.Count <= 1)
			{
				_physicsObject.properties.externalAcceleration.x = 0.0f;
				_physicsObject.properties.externalDeceleration.x = 0.0f;

				if(_physicsObject.properties.deceleration.x == 0.0f)
				{
					_physicsObject.SetVelocityX(0.0f);
				}
			}
		}
	}
}
