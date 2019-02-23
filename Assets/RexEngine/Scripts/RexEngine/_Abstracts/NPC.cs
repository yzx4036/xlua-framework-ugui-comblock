using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class NPC:MonoBehaviour 
	{
		[Tooltip("Whether or not the player can initiate dialogue with this NPC.")]
		public bool isDialogueEnabled = true;
		[Tooltip("A slot to house the attached RexActor.")]
		public Slots slots;
		[Tooltip("When the final dialogue is reached, this determines whether to loop back to the first dialogue, repeat the final dialogue, or disable dialogue.")]
		public DialogueLoopType loopType;
		[Tooltip("If slotted, this AnimationClip will play when the NPC speaks.")]
		public AnimationClip talkAnimation;
		[Tooltip("If there are multiple dialogues, this allows you to set which dialogue the NPC starts with.")]
		public int startingDialogue = 0;
		[Tooltip("Each dialogue below will display when the player talks to the NPC. One dialogue will display each time they talk, before moving to the next dialogue the next time they're spoken to.")]
		public List<Dialogues> dialogues = new List<Dialogues>();
		[Tooltip("Whether or not the NPC will face towards the player when the player talks to them.")]
		public bool facePlayerOnTalk = true;

		protected int currentDialogue; 

		[System.Serializable]
		public class Dialogues
		{
			public List<DialogueManager.Page> pages = new List<DialogueManager.Page>();
		}

		[System.Serializable]
		public class Slots
		{
			public RexActor actor;
		}

		public enum DialogueLoopType
		{
			Looping,
			RepeatFinal,
			Disable
		}

		void Awake() 
		{

		}

		void Start() 
		{
			if(startingDialogue < 0)
			{
				startingDialogue = 0;
			}

			currentDialogue = (startingDialogue > dialogues.Count - 1) ? dialogues.Count - 1 : startingDialogue;
		}

		public void ShowDialogue()
		{
			if(slots.actor && talkAnimation)
			{
				slots.actor.slots.controller.PlayLoopingAnimation(talkAnimation);
				if(slots.actor.slots.physicsObject)
				{
					slots.actor.slots.physicsObject.isEnabled = false;
				}
			}

			DialogueManager.Instance.Show(dialogues[currentDialogue].pages); 
			DialogueManager.Instance.onDialogueComplete += this.OnDialogueComplete;
		}

		protected void OnDialogueComplete()
		{
			DialogueManager.Instance.onDialogueComplete -= this.OnDialogueComplete;
			if(slots.actor && slots.actor.slots.controller)
			{
				slots.actor.slots.controller.StopLoopingAnimation();
			}

			if(slots.actor.slots.physicsObject)
			{
				slots.actor.slots.physicsObject.isEnabled = true;
			}

			currentDialogue ++;
			if(currentDialogue >= dialogues.Count)
			{
				if(loopType == DialogueLoopType.Looping)
				{
					currentDialogue = 0;
				}
				else if(loopType == DialogueLoopType.RepeatFinal)
				{
					currentDialogue = dialogues.Count - 1;
				}
				else if(loopType == DialogueLoopType.Disable)
				{
					isDialogueEnabled = false;
				}
			}
		}

		protected void OnTriggerStay2D(Collider2D col)
		{
			if(!isDialogueEnabled || DialogueManager.Instance.IsDialogueActive())
			{
				return;
			}

			if(col.tag == "Player")
			{
				RexActor actor = col.GetComponent<RexActor>();
				if(actor != null && actor.slots.input != null)
				{
					if(actor.slots.input.verticalAxis > 0.0f)
					{
						if(facePlayerOnTalk)
						{
							if(col.transform.position.x < transform.position.x)
							{
								slots.actor.slots.controller.FaceDirection(Direction.Horizontal.Left);
							}
							else
							{
								slots.actor.slots.controller.FaceDirection(Direction.Horizontal.Right);
							}
						}

						ShowDialogue();
					}
				}
			}
		}
	}
}
