using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class PhysicsMover:MonoBehaviour
	{
		public virtual void NotifyOfObjectOnTop(RexPhysics _physicsObject){}
		public virtual void MovePhysics(RexPhysics _physicsObject){}
		public virtual void OnRemove(RexPhysics _physicsObject){}

		[HideInInspector]
		public int framesBeforeRemoval = 0;

		protected void PlaySound(AudioClip sound)
		{
			AudioSource audioSource = GetComponent<AudioSource>();
			if(audioSource != null && sound != null)
			{
				audioSource.PlayOneShot(sound);
			}
		}
	}
}
