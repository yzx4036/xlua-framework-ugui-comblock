using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	[SelectionBase]
	public class Door:RexObject 
	{
		[System.Serializable]
		public class ActorAnimations
		{
			public string entryAnimationName = "enter_door";
			public string exitAnimationName = "exit_door";
		}

		[System.Serializable]
		public class DoorAnimations
		{
			public AnimationClip openAnimation;
			public AnimationClip closeAnimation;
		}

		public bool willOpenOnTouch;
		public Direction.Vertical pressDirectionVertical = Direction.Vertical.Up;
		public Direction.Horizontal pressDirectionHorizontal;
		public RexObject.Side actorSide;

		public ActorAnimations actorAnimations;
		public DoorAnimations doorAnimations;

		protected List<ActorWithSide> actorsTouched;

		public class ActorWithSide
		{
			public RexActor actor;
			public RexObject.Side side;
		}

		protected bool hasOpened;

		void Awake() 
		{
			actorsTouched = new List<ActorWithSide>();
		}

		void Start() 
		{

		}

		void Update() 
		{
			CheckForEnter();
		}

		public override void NotifyOfCollisionWithPhysicsObject(Collider2D col, Side side, CollisionType type)
		{
			if(col.tag == "Player")
			{
				RexActor rexActor = col.GetComponent<RexActor>();
				if(rexActor != null)
				{
					if(!IsActorAlreadyInList(rexActor))
					{
						if(side == actorSide)
						{
							ActorWithSide actorWithSide = new ActorWithSide();
							actorWithSide.actor = rexActor;
							actorWithSide.side = side;
							actorsTouched.Add(actorWithSide);
							CheckForEnter();
						}
					}
				}
			}
		}

		protected void CheckForEnter()
		{
			if(!hasOpened)
			{
				for(int i = actorsTouched.Count - 1; i >= 0; i --)
				{
					RexActor rexActor = actorsTouched[i].actor;
					if(rexActor != null)
					{
						if(actorSide == actorsTouched[i].side)
						{
							if((rexActor.slots.input && (rexActor.slots.input.verticalAxis == (int)pressDirectionVertical && rexActor.slots.input.horizontalAxis == (int)pressDirectionHorizontal)) || willOpenOnTouch)
							{
								hasOpened = true;
								Open(rexActor);
							}
						}
					}
				}
			}
		}

		protected void Open(RexActor _actor)
		{
			StartCoroutine("EnterDoorCoroutine", _actor as Object);
		}

		protected IEnumerator EnterDoorCoroutine(Object _actor)
		{
			if(slots.anim != null && doorAnimations.openAnimation != null)
			{
				slots.anim.Play(doorAnimations.openAnimation.name);
			}

			RexActor actor = _actor as RexActor;

			actor.GetComponent<BoxCollider2D>().enabled = false;
			actor.slots.physicsObject.isEnabled = false;
			actor.slots.controller.isEnabled = false;
			actor.RemoveControl();

			float duration = 2.0f;

			Animator anim = actor.GetComponent<Animator>();
			if(anim != null)
			{
				AnimationClip entryAnimation = null;
				AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
				for(int i = 0; i < animationClips.Length; i ++)
				{
					if(animationClips[i].name == actorAnimations.entryAnimationName)
					{
						entryAnimation = animationClips[i];
					}
				}

				if(entryAnimation != null)
				{
					anim.Play(entryAnimation.name, 0, 0);
					duration = entryAnimation.length;
				}
			}

			yield return new WaitForSeconds(duration);

			SceneLoader sceneLoader = GetComponent<SceneLoader>();
			if(sceneLoader != null)
			{
				RexSceneManager.Instance.playerOffsetFromSceneLoader = 0.0f;
				sceneLoader.LoadSceneWithFadeOut();
			}
		}

		public void ExitDoor(RexActor _actor)
		{
			StartCoroutine("ExitDoorCoroutine", _actor as Object);
		}

		protected IEnumerator ExitDoorCoroutine(Object _actor)
		{
			if(slots.anim != null && doorAnimations.closeAnimation != null)
			{
				slots.anim.Play(doorAnimations.closeAnimation.name);
			}

			RexActor actor = _actor as RexActor;

			actor.slots.physicsObject.isEnabled = true;
			actor.slots.physicsObject.StopAllMovement();

			float duration = 2.0f;

			Animator anim = actor.GetComponent<Animator>();
			if(anim != null)
			{
				AnimationClip exitAnimation = null;
				AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
				for(int i = 0; i < animationClips.Length; i ++)
				{
					if(animationClips[i].name == actorAnimations.exitAnimationName)
					{
						exitAnimation = animationClips[i];
					}
				}

				if(exitAnimation != null)
				{
					anim.Play(exitAnimation.name, 0, 0);
					duration = exitAnimation.length;
				}
			}

			yield return new WaitForSeconds(duration);

			actor.slots.physicsObject.isEnabled = true;
			actor.slots.controller.isEnabled = true;
			actor.RegainControl();
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player")
			{
				RexActor rexActor = col.GetComponent<RexActor>();
				if(rexActor != null)
				{
					if(!IsActorAlreadyInList(rexActor))
					{
						ActorWithSide actorWithSide = new ActorWithSide();
						actorWithSide.actor = rexActor;
						actorWithSide.side = RexObject.Side.None;
						actorsTouched.Add(actorWithSide);
						CheckForEnter();
					}
				}
			}
		}

		protected void OnTriggerExit2D(Collider2D col)
		{
			if(col.tag == "Player")
			{
				RexActor rexActor = col.GetComponent<RexActor>();
				if(rexActor != null)
				{
					for(int i = actorsTouched.Count - 1; i >= 0; i --)
					{
						if(actorsTouched[i].actor == rexActor)
						{
							actorsTouched.RemoveAt(i);
							return;
						}
					}
				}
			}
		}

		protected bool IsActorAlreadyInList(RexActor rexActor)
		{
			bool isRexActorInList = false;
			for(int i = actorsTouched.Count - 1; i >= 0; i --)
			{
				if(actorsTouched[i].actor == rexActor)
				{
					isRexActorInList = true;
				}
			}

			return isRexActorInList;
		}
	}
}
