using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class BreakingPlatform:PhysicsMover 
	{
		public float durationBeforeBreak = 0.0f;
		public bool requiresConstantContact = true;
		public List<Sprite> breakSprites;
		public SpriteRenderer spriteRenderer;
		public Animator anim;
		public Animations animations;
		public Sounds sounds;
		public bool willReform = false;
		public float durationBeforeReform = 1.0f;

		[HideInInspector]
		public bool hasPlayerOnTop;

		[System.Serializable]
		public class Animations
		{
			public AnimationClip defaultAnimation;
			public AnimationClip breakAnimation;
			public AnimationClip reformAnimation;
		}

		[System.Serializable]
		public class Sounds
		{
			public AudioClip crackSound;
			public AudioClip breakSound;
		}

		protected float timeTowardsBreak = 0.0f;
		protected bool hasEverHadPlayerOnTop;
		protected int currentBreakPhase = 0;
		protected float nextSpriteAppearance;
		protected bool hasBroken = false;
		protected Sprite originalSprite;

		protected List<RexPhysics> actorsOnTop = new List<RexPhysics>();

		void Awake() 
		{
			framesBeforeRemoval = 0;
			anim.enabled = false;
			originalSprite = spriteRenderer.sprite;

			if(breakSprites.Count > 0)
			{
				IncrementNextSpriteAppearance();
			}
		}

		void Update()
		{
			if(hasBroken)
			{
				return;
			}

			if(hasPlayerOnTop || (!requiresConstantContact && hasEverHadPlayerOnTop))
			{
				timeTowardsBreak += Time.deltaTime;
				if(timeTowardsBreak >= nextSpriteAppearance && breakSprites.Count > 0)
				{
					IncrementNextSpriteAppearance();
					PlaySound(sounds.crackSound);
					UpdateBreakSprite();
					currentBreakPhase ++;
				}

				if(timeTowardsBreak >= durationBeforeBreak)
				{
					StartCoroutine("BreakCoroutine");
				}
			}
		}

		void FixedUpdate() 
		{
		}

		public override void NotifyOfObjectOnTop(RexPhysics _physicsObject)
		{
			if(!actorsOnTop.Contains(_physicsObject))
			{
				actorsOnTop.Add(_physicsObject);
				hasEverHadPlayerOnTop = true;
			}

			if(actorsOnTop.Count > 0)
			{
				hasPlayerOnTop = true;
			}
		}

		public override void OnRemove(RexPhysics _physicsObject)
		{
			if(actorsOnTop.Contains(_physicsObject))
			{
				actorsOnTop.Remove(_physicsObject);
			}

			if(actorsOnTop.Count <= 0)
			{
				hasPlayerOnTop = false;
			}
		}

		protected IEnumerator BreakCoroutine()
		{
			hasBroken = true;

			GetComponent<BoxCollider2D>().enabled = false;

			PlaySound(sounds.breakSound);

			if(anim != null && animations.breakAnimation != null)
			{
				anim.enabled = true;
				anim.Play(animations.breakAnimation.name);

				yield return new WaitForSeconds(animations.breakAnimation.length);
			}
			else if(sounds.breakSound != null)
			{
				yield return new WaitForSeconds(sounds.breakSound.length);
			}

			if(willReform)
			{
				yield return new WaitForSeconds(durationBeforeReform);

				Reform();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		protected void IncrementNextSpriteAppearance()
		{
			float interval = (float)(durationBeforeBreak / (breakSprites.Count + 1));
			nextSpriteAppearance += interval;
		}

		protected void UpdateBreakSprite()
		{
			if(currentBreakPhase < breakSprites.Count)
			{
				spriteRenderer.sprite = breakSprites[currentBreakPhase];
			}
		}

		protected void Reform()
		{
			hasBroken = false;

			GetComponent<BoxCollider2D>().enabled = true;
			spriteRenderer.transform.localScale = Vector3.one;

			timeTowardsBreak = 0.0f;
			currentBreakPhase = 0;
			nextSpriteAppearance = 0;

			IncrementNextSpriteAppearance();

			StartCoroutine("ReformCoroutine");
		}

		protected IEnumerator ReformCoroutine()
		{
			if(animations.reformAnimation != null)
			{
				anim.Play(animations.reformAnimation.name, 0, 0);

				yield return new WaitForSeconds(animations.reformAnimation.length);
			}

			spriteRenderer.sprite = originalSprite;
			anim.enabled = false;
			spriteRenderer.transform.localScale = Vector3.one;
		}
	}
}
