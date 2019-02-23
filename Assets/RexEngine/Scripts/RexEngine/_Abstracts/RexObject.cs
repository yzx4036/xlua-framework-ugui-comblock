/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	//The base RexEngine object class. Extended by RexActor.
	[SelectionBase]
	public class RexObject:MonoBehaviour 
	{
		//Primarily used to determine the side a collision happened on
		public enum Side
		{
			None,
			Left,
			Right,
			Top,
			Bottom
		}

		//Used to check against collision type
		public enum CollisionType
		{
			Enter,
			Stay,
			Exit
		}

		//This is for the most common things that need to be slotted on a RexObject, to keep them all in one easy place
		[System.Serializable]
		public class Slots
		{
			[Tooltip("The Animator used to display this actor's animations.")]
			public Animator anim;
			[Tooltip("The primary SpriteRenderer used by this actor.")]
			public SpriteRenderer spriteRenderer;
			[Tooltip("The RexController this actor uses to move.")]
			public RexController controller;
			[Tooltip("The RexPhysics component used to give this actor physics.")]
			public RexPhysics physicsObject;
			[Tooltip("The primary Collider2D used by this actor.")]
			public Collider2D collider;
			[Tooltip("The parent Transform holding this actor's SpriteRenderers, if applicable.")]
			public Transform spriteHolder;
			[Tooltip("The RexInput used to let a player control this actor, if applicable.")]
			public RexInput input;
			[Tooltip("The Jitter component used to make this actor's sprite jitter when required, if applicable.")]
			public Jitter jitter;
		}

		[Tooltip("This contains slots to hold the major components this actor uses to function. They can be automatically filled using the Auto-Fill Slots button at the bottom of this component's Inspector window.")]
		public Slots slots;

		[HideInInspector]
		public TimeStop timeStop;

		[System.Serializable]
		public class TimeStop
		{
			public bool isTimeStopped;
			public bool wasAnimEnabledPreStop;
			public bool wasAIEnabledPreStop;
			public bool werePhysicsEnabledPreStop;
		}

		[HideInInspector]
		public RexPool parentSpawnPool;

		[HideInInspector]
		public bool willDespawnOnSceneExit = true; //If the object despawns when it exits a scene

		[HideInInspector]
		public bool willDieInBottomlessPits = true; //If the object should be destroyed when it falls into a bottomless pit

		//Sets the position of the object, including notifying its RexPhysics and its RexController if necessary
		public void SetPosition(Vector2 position)
		{
			transform.position = position;

			if(slots.physicsObject)
			{
				slots.physicsObject.properties.position = position;
				slots.physicsObject.previousFrameProperties.position = position;
			}

			if(slots.controller)
			{
				slots.controller.aerialPeak = transform.position.y;
			}
		}

		//Plays a sound effect only if the object playing the sound effect is currently within the boundaries of the main camera
		public void PlaySoundIfOnCamera(AudioClip clip, float pitch = 1.0f, float volume = 1.0f, AudioSource source = null)
		{
			#if UNITY_EDITOR
			if(EditorPrefs.GetBool("AreSFXMuted"))
			{
				return;
			}
			#endif

			if(CameraHelper.CameraContainsPoint(transform.position, 2.5f))
			{
				AudioSource newSource = (source == null) ? GetComponent<AudioSource>() : source;
				if(newSource != null)
				{
					newSource.volume = volume;
					newSource.pitch = pitch;
					newSource.PlayOneShot(clip);
				}
			}
		}

		public void StopTime()
		{
			if(slots.anim)
			{
				if(!timeStop.isTimeStopped)
				{
					timeStop.wasAnimEnabledPreStop = slots.anim.enabled;
				}

				slots.anim.enabled = false;
			}

			if(slots.physicsObject)
			{
				if(!timeStop.isTimeStopped)
				{
					timeStop.werePhysicsEnabledPreStop = slots.physicsObject.isEnabled;
				}

				slots.physicsObject.isEnabled = false;
			}

			timeStop.isTimeStopped = true;
		}

		public void StartTime()
		{
			timeStop.isTimeStopped = false;
			if(slots.anim)
			{
				slots.anim.enabled = timeStop.wasAnimEnabledPreStop;
			}

			if(slots.physicsObject)
			{
				slots.physicsObject.isEnabled = timeStop.werePhysicsEnabledPreStop;
				slots.physicsObject.properties.isFalling = false;
			}
		}

		//Called when an object is moving out of a scene, but hasn't fully left; can be overidden to have unique effects here
		public virtual void OnSceneBoundaryCollisionInsideBuffer(){}

		//This is called when PhysicsManager.gravityScale is changed
		public virtual void OnGravityScaleChanged(float _gravityScale)
		{
			float scaleMultiplier = (_gravityScale >= 0.0f) ? 1.0f : -1.0f;
			transform.localScale = new Vector3(transform.localScale.x, scaleMultiplier, 1.0f);

			if(slots.controller)
			{
				slots.controller.AnimateGravityFlip();
			}

			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			if(collider)
			{
				if(Mathf.Abs(collider.offset.y) > 0.0f)
				{
					//This handles a specific edgecase where gravity reverses while the actor is crouching down in a one-tile-high passageway; it ensures that they retain the same position and don't leave the crouching state
					SetPosition(new Vector2(transform.position.x, transform.position.y + collider.offset.y * slots.controller.GravityScaleMultiplier()));
				}
			}
		}

		//This can be overidden by individual objects to have them do unique things when their RexPhysics component encounters different types of collisions
		public virtual void OnPhysicsCollision(Collider2D col, Side side, CollisionType type){}

		//This can be overidden by individual objects to have them do unique things when another RexPhysics component collides with it
		public virtual void NotifyOfCollisionWithPhysicsObject(Collider2D col, Side side, CollisionType type){}

		//This can be overidden by individual objects to clean them up in various ways
		public virtual void Clear()
		{
			if(parentSpawnPool)
			{
				parentSpawnPool.Despawn(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}