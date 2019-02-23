using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class TimerManager:MonoBehaviour 
	{
		[System.Serializable]
		public class Settings
		{
			public bool isEnabled;
			public float startingTime = 300.0f;
		}

		public Settings settings;

		public TextMesh textMesh;

		protected float currentTime;
		protected bool isPaused = true;

		private static TimerManager instance = null;
		public static TimerManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<TimerManager>();
					go.name = "TimerManager";
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

			settings = ProjectSettingsAsset.Instance.rexSettingsData.timerManagerSettings;

			//DontDestroyOnLoad(gameObject);
		}

		void Start()
		{
			if(!settings.isEnabled)
			{
				if(textMesh)
				{
					textMesh.gameObject.SetActive(false);
				}
			}

			if(settings.isEnabled)
			{
				ResetTimer();
				StartTimer();
			}
		}

		public void StartTimer()
		{
			isPaused = false;
		}

		public void StopTimer()
		{
			isPaused = true;
		}

		public void ResetTimer()
		{
			currentTime = settings.startingTime;
		}

		public void Show()
		{
			textMesh.gameObject.SetActive(true);
		}

		public void Hide()
		{
			textMesh.gameObject.SetActive(false);
		}

		void Update()
		{
			if(settings.isEnabled && !isPaused)
			{
				currentTime -= Time.deltaTime;
				if(currentTime < 0.0f)
				{
					currentTime = 0.0f;
					GameManager.Instance.OnTimerExpired();
				}

				UpdateText();
			}
		}

		protected void UpdateText()
		{
			textMesh.text = "Time: " + ((int)currentTime).ToString();
		}
	}
}
