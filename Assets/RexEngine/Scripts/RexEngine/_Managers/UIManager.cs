using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class UIManager:MonoBehaviour 
	{
		private static UIManager instance = null;
		public static UIManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<UIManager>();
					go.name = "UIManager";
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

			//DontDestroyOnLoad(gameObject);
		}

		public void MakeUIActive()
		{
			#if UNITY_ANDROID || UNITY_IPHONE
			RexTouchInput rexTouchInput = GameManager.Instance.players[0].GetComponent<RexTouchInput>();
			if(rexTouchInput != null)
			{
			rexTouchInput.ToggleTouchInterface(true);
			}
			#endif

			if(GameManager.Instance.settings.isScoreEnabled)
			{
				ScoreManager.Instance.gameObject.SetActive(true);
				ScoreManager.Instance.SetScoreAtCheckpoint(0);
				ScoreManager.Instance.SetScore(0);
			}

			if(TimerManager.Instance.settings.isEnabled)
			{
				TimerManager.Instance.Show();
				TimerManager.Instance.StartTimer();
			}

			for(int i = 0; i < GameManager.Instance.players.Count; i ++)
			{
				RexActor player = GameManager.Instance.players[i];
				if(player.hp.bar)
				{
					player.hp.bar.gameObject.SetActive(true);
				}

				if(player.hp)
				{
					player.RestoreHP(player.hp.max);
				}

				if(player.mpProperties.mp)
				{
					player.mpProperties.mp.bar.gameObject.SetActive(true);
				}

				player.CancelActivePowerups();
			}

			if(LivesManager.Instance.settings.areLivesEnabled)
			{
				LivesManager.Instance.Show();
			}
		}

		public void MakeUIInactive()
		{
			RexTouchInput rexTouchInput = GameManager.Instance.player.GetComponent<RexTouchInput>();
			if(rexTouchInput != null)
			{
				rexTouchInput.ToggleTouchInterface(false);
			}

			ScoreManager.Instance.gameObject.SetActive(false);

			for(int i = 0; i < GameManager.Instance.players.Count; i ++)
			{
				RexActor player = GameManager.Instance.players[i];
				if(player.hp.bar)
				{
					player.hp.bar.gameObject.SetActive(false);
				}

				if(player.mpProperties.mp)
				{
					player.mpProperties.mp.bar.gameObject.SetActive(false);
				}
			}

			ScoreManager.Instance.text.gameObject.SetActive(false);
			PauseManager.Instance.isPauseEnabled = false;
			LivesManager.Instance.Hide();

			if(TimerManager.Instance.settings.isEnabled)
			{
				TimerManager.Instance.Hide();
				TimerManager.Instance.StopTimer();
			}
		}
	}

}
