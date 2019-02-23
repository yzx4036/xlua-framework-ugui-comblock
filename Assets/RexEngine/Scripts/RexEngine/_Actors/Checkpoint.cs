/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class Checkpoint:MonoBehaviour 
	{
		public string id = "A"; //Every checkpoint in a room needs a unique ID so the game knows which one to load the player into
		public Direction.Horizontal playerSpawnDirection; //Whether the player faces left or right when spawned into this checkpoint
		public Transform flagSprite;
		public AudioSource audioSource;
		public AudioClip activateSound;

		protected bool hasBeenActivated = false;

		void Awake() 
		{

		}

		void Start() 
		{
			CheckForPriorActivation();
		}

		//Check to see if the player is already registered at this checkpoint; if so, disable it for future collisions
		protected void CheckForPriorActivation()
		{
			if(LivesManager.Instance.lastSavedScene == RexSceneManager.Instance.GetCurrentLevel() && LivesManager.Instance.lastCheckpointID == id)
			{
				hasBeenActivated = true;
				RaiseFlag();
			}
		}

		protected void RaiseFlag()
		{
			if(flagSprite)
			{
				flagSprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				flagSprite.transform.localPosition = new Vector3(0.0f, 0.34f, 0.0f);
			}
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player")
			{
				if(!hasBeenActivated)
				{
					if(audioSource && activateSound)
					{
						audioSource.PlayOneShot(activateSound);
					}

					hasBeenActivated = true;
					RaiseFlag();
					GameManager.Instance.SetSavedScene(RexSceneManager.Instance.GetCurrentLevel(), id); //Save our progress at this checkpoint
					ScoreManager.Instance.SetScoreAtCheckpoint(ScoreManager.Instance.score);
					DataManager.Instance.Save();
				}
			}
		}
	}
}

