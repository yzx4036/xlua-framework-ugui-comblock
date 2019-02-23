/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	public class GameManager:MonoBehaviour 
	{
		[System.Serializable]
		public class Settings
		{
			public int numberOfPlayers;
			public List<RexActor> playerPrefabs = new List<RexActor>();
			public bool willStopTimeOnDialogueShow;
			public bool isScoreEnabled;
			public bool showReadyMessage = true;
			public bool stopMusicOnPlayerDeath = true;
		}

		[HideInInspector]
		public Settings settings;

		public List<RexActor> players;

		[System.NonSerialized]
		public bool useDebugInvincibility;

		[System.NonSerialized]
		public RexActor player;

		public RexInput input;

		private static GameManager instance = null;
		public static GameManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<GameManager>();
					go.name = "GameManager";
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

			settings = ProjectSettingsAsset.Instance.rexSettingsData.gameManagerSettings;

			useDebugInvincibility = false;

			if(settings.numberOfPlayers < 1)
			{
				settings.numberOfPlayers = 1;
			}
			else if(settings.numberOfPlayers > 2)
			{
				settings.numberOfPlayers = 2;
			}

			//DontDestroyOnLoad(gameObject);
		}

		public void Init()
		{

		}

		void Start()
		{
			#if UNITY_EDITOR
			useDebugInvincibility = EditorPrefs.GetBool("DebugInvincibility");
			#endif

			if(useDebugInvincibility)
			{
				GameManager.Instance.useDebugInvincibility = true;
				player.GetComponent<RexActor>().invincibility.isDebugInvincibilityActive = true;
			}
		}

		public void MakePlayersActive()
		{
			RexSceneManager.Instance.playerSpawnType = RexSceneManager.PlayerSpawnType.SpawnPoint;

			for(int i = 0; i < GameManager.Instance.players.Count; i ++)
			{
				RexActor player = GameManager.Instance.players[i];
				player.gameObject.SetActive(true);

				if(player.GetComponent<Booster>() != null) //TODO: Would be nice to not worry about Booster here
				{
					player.Reset();
				}

				player.Revive();

				player.slots.input.isEnabled = true;
				player.SetPosition(new Vector2(-10000.0f, -10000.0f));
			}
		}

		public void MakePlayersInactive()
		{
			for(int i = 0; i < GameManager.Instance.players.Count; i ++)
			{
				RexActor player = GameManager.Instance.players[i];
				if(player.GetComponent<Booster>() != null) //TODO: Would be nice to not worry about Booster here
				{
					player.Reset();
				}

				player.CancelInvoke();

				player.slots.input.isEnabled = false;
				player.SetPosition(new Vector2(-10000.0f, -10000.0f));

				player.gameObject.SetActive(false);
			}
		}

		public void OnPlayerSpawn()
		{
			StartCoroutine("PlayerSpawnCoroutine");
		}

		public void OnTimerExpired()
		{
			player.KillImmediately();
		}

		protected IEnumerator PlayerSpawnCoroutine()
		{
			float duration = 0.0f;

			for(int i = 0; i < players.Count; i ++)
			{
				RexActor additionalPlayer = players[i];
				additionalPlayer.slots.input.isEnabled = false;

				if(additionalPlayer.slots.controller && additionalPlayer.slots.controller.animations.spawnAnimation)
				{
					if(additionalPlayer.slots.controller.animations.spawnAnimation.length > duration)
					{
						duration = additionalPlayer.slots.controller.animations.spawnAnimation.length;
					}

					additionalPlayer.slots.controller.PlaySingleAnimation(additionalPlayer.slots.controller.animations.spawnAnimation);
				}
			}

			float readyMessageDuration = 3.5f;
			if(readyMessageDuration > duration)
			{
				duration = readyMessageDuration;
			}

			if(settings.showReadyMessage)
			{
				ReadyMessage.Instance.Show();
				yield return new WaitForSeconds(duration);
			}

			for(int i = 0; i < players.Count; i ++)
			{
				RexActor additionalPlayer = players[i];
				additionalPlayer.slots.input.isEnabled = true;
			}
		}

		public void OnPlayerDeath()
		{
			StartCoroutine("PlayerDeathCoroutine");
		}

		public void OnPlayerEnteredBottomlessPit(RexActor _player)
		{
			if(!_player.isDead)
			{
				_player.KillImmediately();
			}
		}

		protected IEnumerator PlayerDeathCoroutine()
		{
			LivesManager.Instance.deaths ++;
			ObjectDisablerManager.Instance.ResetSingleLifeDisabledObjects();

			for(int i = 0; i < players.Count; i ++)
			{
				RexActor additionalPlayer = players[i];
				if(!players[i].isDead)
				{
					additionalPlayer.slots.controller.SetToDead();
					additionalPlayer.isDead = true;
					additionalPlayer.RemoveControl();
					additionalPlayer.slots.physicsObject.isEnabled = false;
					additionalPlayer.slots.physicsObject.StopAllMovement();
				}
			}

			if(TimerManager.Instance.settings.isEnabled)
			{
				TimerManager.Instance.StopTimer();
			}

			DialogueManager.Instance.Hide();

			if(settings.stopMusicOnPlayerDeath)
			{			
				RexSoundManager.Instance.Pause();
			}

			yield return new WaitForSeconds(2.0f);

			RexSceneManager.Instance.playerSpawnType = RexSceneManager.PlayerSpawnType.SpawnPoint;

			ScreenFade.Instance.Fade(ScreenFade.FadeType.Out, ScreenFade.FadeDuration.Short, Color.white);
			yield return new WaitForSeconds(ScreenFade.Instance.currentFadeDuration);

			PhysicsManager.Instance.gravityScale = 1.0f;

			for(int i = 0; i < players.Count; i ++)
			{
				RexActor additionalPlayer = players[i];
				if(additionalPlayer.slots.spriteHolder)
				{
					additionalPlayer.slots.spriteHolder.gameObject.SetActive(false);
				}
			}

			ScoreManager.Instance.RevertToLastCheckpointScore();
			DataManager.Instance.Load();

			for(int i = 0; i < players.Count; i ++)
			{
				RexActor additionalPlayer = players[i];
				additionalPlayer.Reset();
			}

			bool isGameOver = LivesManager.Instance.DecrementLives(1);

			if(!isGameOver) //The player has lives remaining; return to the last checkpoint
			{
				string sceneToLoad = LivesManager.Instance.lastSavedScene; //TODO: DataManager.Instance.lastSavedScene was the old way //DataManager.Instance.lastSavedScene;
				RexSceneManager.Instance.LoadSceneWithFadeOut(sceneToLoad, Color.white, false, false);

				for(int i = 0; i < players.Count; i ++)
				{
					RexActor additionalPlayer = players[i];
					additionalPlayer.RegainControl();
				}

				yield return new WaitForSeconds(1.0f);

				if(TimerManager.Instance.settings.isEnabled)
				{
					TimerManager.Instance.ResetTimer();
					TimerManager.Instance.StartTimer();
				}

				for(int i = 0; i < players.Count; i ++)
				{
					RexActor additionalPlayer = players[i];
					additionalPlayer.Revive();
					additionalPlayer.slots.physicsObject.isEnabled = true;
				}
			}
			else //The player has no more lives; go to the Game Over scene
			{
				RexSceneManager.Instance.LoadSceneWithFadeOut(LivesManager.Instance.settings.gameOverScene, Color.white, false, false);

				yield return new WaitForSeconds(1.0f);

				if(TimerManager.Instance.settings.isEnabled)
				{
					TimerManager.Instance.ResetTimer();
				}
			}

			yield return new WaitForSeconds(0.5f);

			ScreenFade.Instance.Fade(ScreenFade.FadeType.In);
		}

		public void SetSavedScene(string _lastSavedScene, string _checkpointID)
		{
			LivesManager.Instance.lastSavedScene = _lastSavedScene;
			LivesManager.Instance.lastCheckpointID = _checkpointID;
		}
	}
}
