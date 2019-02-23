/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class PauseManager:MonoBehaviour 
	{
		public bool isPauseEnabled;
		public GameObject pauseOverlay;

		protected bool isPaused;

		private static PauseManager instance = null;
		public static PauseManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<PauseManager>();
					go.name = "PauseManager";
				}

				return instance; 
			} 
		}

		void Awake()
		{
			if(instance == null)
			{
				instance = this;
			}
		}

		void Update()
		{
			if(GameManager.Instance.input.isPauseButtonDown && isPauseEnabled)
			{
				TogglePause();
			}
		}

		public void TogglePause()
		{
			if(!IsGamePaused())
			{
				Pause();
			}
			else
			{
				UnPause();
			}
		}

		public bool IsGamePaused()
		{
			return isPaused;
		}

		protected void Pause()
		{
			isPaused = true;
			Time.timeScale = 0.0f;

			if(pauseOverlay)
			{
				pauseOverlay.SetActive(true);
			}
		}

		protected void UnPause()
		{
			isPaused = false;
			Time.timeScale = 1.0f;

			if(pauseOverlay)
			{
				pauseOverlay.SetActive(false);
			}
		}
	}
}
