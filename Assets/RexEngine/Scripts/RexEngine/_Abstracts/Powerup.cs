/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	public class Powerup:RexObject
	{
		public enum StackType
		{
			OverwriteOld,
			IgnoreNew
		}

		public enum AffectType
		{
			CollidingActor,
			OnscreenEnemies,
			AllEnemies
		}

		[System.Serializable]
		public class Sounds
		{
			public AudioClip collectSound;
		}

		[HideInInspector]
		public string idString;

		public enum Equation
		{
			Increment,
			Decrement
		}

		[Tooltip("A slot for the AudioClip that plays when the powerup is collected.")]
		public Sounds sounds;
		[Tooltip("If True, this powerup's GameObject will be destroyed as soon as it is collected.")]
		public bool willDestroyOnCollision = true;
		[Tooltip("If True, enemies can collect this powerup in addition to players.")]
		public bool canBeCollectedByEnemies;
		[Tooltip("If slotted, this RexParticle will play when the powerup is collected.")]
		public RexParticle collectParticle;
		[Tooltip("Options for what happens when you collect multiple powerups of this same type while a previous powerup of this same type is already active. If Overwrite Old is set, the new instance of the powerup will overwrite the old one. If Ignore New is set, the old instance of the powerup will continue operating and the newly-collected instance will be ignored.")]
		public StackType stackType;
		[Tooltip("Lets you set whether this powerup's effect applies to: Colliding Actor, or only the actor that collects it; Onscreen Enemies, or every enemy onscreen at the time the powerup is collected; or All Enemies, which affects all enemies in the scene.")]
		public AffectType affect;

		[HideInInspector]
		public List<RexActor> affectedObjects;

		public virtual void RemoveEffect(RexActor actor)
		{
			RemoveAffectedActor(actor);
		}

		public void RemoveFromAllAffected()
		{
			if(affectedObjects != null)
			{
				for(int i = affectedObjects.Count - 1; i >= 0; i --)
				{
					if(affectedObjects[i] != null)
					{
						RemoveEffect(affectedObjects[i]);
					}
					else
					{
						affectedObjects.RemoveAt(i);
					}
				}
			}

			if(affectedObjects.Count <= 0)
			{
				Destroy(gameObject);
			}
		}

		protected virtual void TriggerEffect(RexActor actor){}

		protected virtual void OnCollisionProcessed(){}

		protected virtual void AnimateIn(){} //Animates the entire powerup in; not specific to any actor
		protected virtual void AnimateInForActor(RexActor actor){} //Only called if this is the first instance of this powerup on an actor at once (as opposed to a stacking one)
		protected virtual void AnimateOutForActor(RexActor actor){}

		protected Powerup GetPowerupFromActor(RexActor _actor)
		{
			Powerup powerup = null;
			for(int i = _actor.activePowerups.Count; i >= 0; i --)
			{
				if(_actor.activePowerups[i] != null)
				{
					if(_actor.activePowerups[i].idString == idString)
					{
						powerup = _actor.activePowerups[i];
						break;
					}
				}
			}

			return powerup;
		}

		protected void AddToAffectedActors(RexActor actor)
		{
			if(affectedObjects == null)
			{
				affectedObjects = new List<RexActor>();
			}

			bool isAlreadyAdded = false;
			for(int i = 0; i < affectedObjects.Count; i ++)
			{
				if(affectedObjects[i] == actor)
				{
					isAlreadyAdded = true;
				}
			}

			if(!isAlreadyAdded)
			{
				affectedObjects.Add(actor);
				actor.AddToActivePowerups(this);
			}
		}

		protected void RemoveAffectedActor(RexActor actor)
		{
			if(affectedObjects.Contains(actor))
			{
				actor.activePowerups.Remove(this);
				AnimateOutForActor(actor);
				affectedObjects.Remove(actor);
			}

			for(int i = affectedObjects.Count - 1; i >= 0; i --)
			{
				if(affectedObjects[i] == null)
				{
					affectedObjects.RemoveAt(i);
				}
			}

			if(affectedObjects.Count <= 0 && !willDestroyOnCollision)
			{
				Destroy(gameObject);
			}
		}

		protected void RemovePreviousPowerup(RexActor _actor)
		{
			Powerup powerup = null;
			for(int i = _actor.activePowerups.Count - 1; i >= 0; i --)
			{
				if(_actor.activePowerups[i] != null)
				{
					if(_actor.activePowerups[i].idString == idString)
					{
						powerup = _actor.activePowerups[i];
						break;
					}
				}
			}

			if(powerup != null)
			{
				powerup.RemoveAffectedActor(_actor);
			}
		}

		protected bool DetermineIfActorHasPowerupActive(List<Powerup> _activePowerups)
		{
			bool isPowerupActive = false;

			if(_activePowerups == null)
			{
				return false;
			}

			for(int i = _activePowerups.Count - 1; i >= 0; i --)
			{
				if(_activePowerups[i] != null)
				{
					if(_activePowerups[i].idString == idString)
					{
						isPowerupActive = true;
						break;
					}
				}
			}

			return isPowerupActive;
		}

		protected void AddTargetAndActivate(RexActor actor)
		{
			bool isFirstInstanceOfPowerupOnActor = true;
			if(DetermineIfActorHasPowerupActive((actor != null && actor.activePowerups != null) ? actor.activePowerups : null))
			{
				if(stackType == StackType.OverwriteOld)
				{
					RemovePreviousPowerup(actor);
					isFirstInstanceOfPowerupOnActor = false;
				}
				else
				{
					return;
				}

			}

			AddToAffectedActors(actor);
			TriggerEffect(actor);

			if(isFirstInstanceOfPowerupOnActor)
			{
				AnimateInForActor(actor);
			}
		}

		protected List<RexActor> GetActorsToAffect(RexActor collidingActor)
		{
			List<RexActor> actorsToAffect = new List<RexActor>();
			if(affect == AffectType.OnscreenEnemies || affect == AffectType.AllEnemies)
			{
				GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
				for(int i = 0; i < enemies.Length; i ++)
				{
					Enemy enemy = enemies[i].GetComponent<Enemy>();
					if(enemy != null && (affect != AffectType.OnscreenEnemies || CameraHelper.CameraContainsPoint(enemy.transform.position)))
					{
						actorsToAffect.Add(enemy);
					}
				}
			}
			else if(affect == AffectType.CollidingActor)
			{
				actorsToAffect.Add(collidingActor);
			}

			return actorsToAffect;
		}

		protected virtual void Kill()
		{
			GetComponent<Collider2D>().enabled = false;

			if(slots.spriteRenderer)
			{
				slots.spriteRenderer.enabled = false;
			}

			if(sounds.collectSound != null)
			{
				PlaySoundIfOnCamera(sounds.collectSound);
			}

			if(collectParticle)
			{
				collectParticle.Play();
			}
		}

		protected IEnumerator KillCoroutine()
		{
			ObjectDisabler objectDisabler = GetComponent<ObjectDisabler>();
			if(objectDisabler != null)
			{
				objectDisabler.MarkForDisable();	
			}

			Kill();

			if(willDestroyOnCollision)
			{
				yield return new WaitForSeconds(1.5f);

				Destroy(gameObject);
			}
			else
			{
				DontDestroyOnLoad(gameObject);
				ParentHelper.Parent(gameObject, ParentHelper.ParentObject.Powerups);
			}
		}

		protected void ProcessCollision(Collider2D col)
		{
			if(col.gameObject.tag == "Player" || (col.gameObject.tag == "Enemy" && canBeCollectedByEnemies))
			{
				RexActor actor = col.GetComponent<RexActor>();
				if(actor != null)
				{
					List<RexActor> actorsToAffect = GetActorsToAffect(actor);
					for(int i = 0; i < actorsToAffect.Count; i ++)
					{
						AddTargetAndActivate(actorsToAffect[i]);
					}

					OnCollisionProcessed();
					AnimateIn();

					StartCoroutine("KillCoroutine");
				}
			}
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			ProcessCollision(col);
		}
	}

}