/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class GravityChanger:MonoBehaviour 
	{
		public enum Type
		{
			Reverse,
			Normal
		}

		public Type type;
		public AudioSource audioSource;
		public AudioClip gravityChangeSound;

		void Awake() 
		{

		}

		void Start() 
		{

		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player" && !PhysicsManager.Instance.isSceneLoading) //The PhysicsManager check here ensures these don't get triggered mid-scene load, before the player has fully moved to the spawn point
			{
				if(type == Type.Reverse && PhysicsManager.Instance.gravityScale > 0.0f)
				{
					PhysicsManager.Instance.gravityScale = -1.0f;
					PlayGravityChangeSound();
				}
				else if(type == Type.Normal && PhysicsManager.Instance.gravityScale < 0.0f)
				{
					PhysicsManager.Instance.gravityScale = 1.0f;
					PlayGravityChangeSound();
				}
			}
		}

		protected void PlayGravityChangeSound()
		{
			if(audioSource)
			{
				audioSource.PlayOneShot(gravityChangeSound);
			}
		}
	}
}

