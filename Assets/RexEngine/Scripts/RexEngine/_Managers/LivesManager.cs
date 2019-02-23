using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class LivesManager:MonoBehaviour 
	{
		[System.Serializable]
		public class Settings
		{
			public int startingLives = 3;
			public int maxLives = 99;
			public bool areLivesEnabled = true;
			public bool does0Count = true;

			[HideInInspector]
			public string gameOverScene = "Demo_GameOver";

			[HideInInspector]
			public string postGameOverScene = "Demo_Title";

			[HideInInspector]
			public string defaultRespawnScene = "Demo_1";
		}

		public Settings settings;

		public TextMesh textMesh;
		public GameObject icon;

		[System.NonSerialized]
		public string lastCheckpointID = "A";

		[System.NonSerialized]
		public string lastSavedScene = "Demo_1";

		[System.NonSerialized]
		public int deaths = 0;

		protected int currentLives;

		private static LivesManager instance = null;
		public static LivesManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<LivesManager>();
					go.name = "LivesManager";
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

			settings = ProjectSettingsAsset.Instance.rexSettingsData.livesManagerSettings;
		}

		void Start() 
		{
			currentLives = settings.startingLives;
			UpdateTextMesh();

			if(!settings.areLivesEnabled)
			{
				Hide();
				gameObject.SetActive(false);
			}

			ResetSavedValues();
		}

		public void IncrementLives(int amountToIncrement = 1)
		{
			if(settings.areLivesEnabled)
			{
				currentLives += amountToIncrement;
				if(currentLives > settings.maxLives)
				{
					currentLives = settings.maxLives;
				}

				UpdateTextMesh();
			}
		}

		public bool DecrementLives(int amountToDecrement = 1)
		{
			bool isGameOver = false;

			if(settings.areLivesEnabled)
			{
				currentLives -= amountToDecrement;
				if((settings.does0Count && currentLives < 0) || (!settings.does0Count && currentLives <= 0))
				{
					currentLives = 0;
					isGameOver = true;
				}

				UpdateTextMesh();
			}

			return isGameOver;
		}

		public int GetCurrentLives()
		{
			return currentLives;
		}

		public void ResetLivesToStartingValue()
		{
			currentLives = settings.startingLives;
			UpdateTextMesh();
		}

		public void ResetSavedValues()
		{
			deaths = 0;
			lastSavedScene = settings.defaultRespawnScene;
			lastCheckpointID = "A";
		}

		public void Show()
		{
			if(settings.areLivesEnabled)
			{
				textMesh.gameObject.SetActive(true);
				icon.SetActive(true);
			}
		}

		public void Hide()
		{
			textMesh.gameObject.SetActive(false);
			icon.SetActive(false);
		}

		protected void UpdateTextMesh()
		{
			if(textMesh != null)
			{
				textMesh.text = currentLives.ToString();
			}
		}
	}
}
