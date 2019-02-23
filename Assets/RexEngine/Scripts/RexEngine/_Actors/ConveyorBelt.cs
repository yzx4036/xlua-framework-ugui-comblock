using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ConveyorBelt:PhysicsMover
	{
		public float moveSpeed = 1.0f;
		public Animations animations;
		public Animators animators;

		[System.Serializable]
		public class Animators
		{
			public Animator left;
			public Animator right;
			public List<Animator> middle;
		}

		[System.Serializable]
		public class Animations
		{
			public AnimationClip left;
			public AnimationClip right;
			public AnimationClip middle;
		}

		void Awake() 
		{
			framesBeforeRemoval = 2;

			if(animators.left)
			{
				animators.left.Play(animations.left.name);
			}

			if(animators.right)
			{
				animators.right.Play(animations.right.name);
			}

			if(animators.middle != null)
			{
				for(int i = 0; i < animators.middle.Count; i ++)
				{
					animators.middle[i].Play(animations.middle.name);
				}
			}
		}

		public override void MovePhysics(RexPhysics _physicsObject)
		{
			_physicsObject.AddToExternalVelocity(new Vector2(moveSpeed, 0.0f));
		}
	}
}
