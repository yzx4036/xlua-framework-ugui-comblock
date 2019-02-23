/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	[SelectionBase]
	public class Water:MonoBehaviour
	{
		public AudioClip splashSound;
		public RexPool enterSplashPool;
		public RexPool exitSplashPool;

		protected float waveTop;

		void Awake()
		{
			waveTop = GetComponent<BoxCollider2D>().bounds.max.y;
		}

		protected void GenerateEnterSplash(Vector2 location, Transform actor)
		{
			if(enterSplashPool)
			{
				GameObject splash = enterSplashPool.Spawn().gameObject;
				splash.GetComponent<RexParticle>().Play();
				splash.transform.position = new Vector3(location.x, location.y, splash.transform.position.z);
			}

			if(CameraHelper.CameraContainsPoint(actor.position, 1.5f))
			{
				PlaySplashSound();
			}
		}

		protected void GenerateExitSplash(Vector2 location, Transform actor)
		{
			if(exitSplashPool)
			{
				GameObject splash = exitSplashPool.Spawn().gameObject;
				splash.GetComponent<RexParticle>().Play();
				splash.transform.position = new Vector3(location.x, location.y, splash.transform.position.z);
			}

			if(CameraHelper.CameraContainsPoint(actor.position, 1.5f))
			{
				PlaySplashSound();
			}
		}

		protected void PlaySplashSound()
		{
			AudioSource audio = GetComponent<AudioSource>();
			if(audio && splashSound)
			{
				audio.PlayOneShot(splashSound);
			}
		}

		protected void ProcessCollision(Collider2D col, RexObject.CollisionType collisionType)
		{
			float waveTop = GetComponent<BoxCollider2D>().bounds.max.y;
			RexActor actor = col.gameObject.GetComponent<RexActor>();
			if(actor != null)
			{
				if(collisionType == RexObject.CollisionType.Enter)
				{
					actor.NotifyOfWaterlineContact(collisionType);
					GenerateEnterSplash(new Vector2(col.gameObject.transform.position.x, waveTop), col.gameObject.transform);
				}
				else if(collisionType == RexObject.CollisionType.Exit)
				{
					actor.NotifyOfWaterlineContact(collisionType);
					if(actor.waterProperties.waterBodiesTouched <= 0)
					{
						GenerateExitSplash(new Vector2(col.gameObject.transform.position.x, waveTop), col.gameObject.transform);
					}
				}
			}
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			ProcessCollision(col, RexObject.CollisionType.Enter);
		}

		protected void OnTriggerExit2D(Collider2D col)
		{
			ProcessCollision(col, RexObject.CollisionType.Exit);
		}

		protected void OnDestroy()
		{
			splashSound = null;
			enterSplashPool = null;
			exitSplashPool = null;
		}
	}

}
