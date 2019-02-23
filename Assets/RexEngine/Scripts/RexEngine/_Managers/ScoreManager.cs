/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ScoreManager:MonoBehaviour 
	{
		public int score;
		public TextMesh text;

		protected int scoreAtLastCheckpoint;

		private static ScoreManager instance = null;
		public static ScoreManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<ScoreManager>();
					go.name = "ScoreManager";
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

		void Start()
		{
			if(!GameManager.Instance.settings.isScoreEnabled)
			{
				gameObject.SetActive(false);
			}
		}

		public void IncrementScore(int _incrementBy)
		{
			score += _incrementBy;
			UpdateDisplay();
		}

		public void DecrementScore(int _incrementBy)
		{
			score -= _incrementBy;
			if(score < 0)
			{
				score = 0;
			}

			UpdateDisplay();
		}

		public void SetScore(int _score)
		{
			score = _score;
			UpdateDisplay();
		}

		public void SetScoreAtCheckpoint(int _score)
		{
			scoreAtLastCheckpoint = _score;
		}

		public void RevertToLastCheckpointScore()
		{
			score = scoreAtLastCheckpoint;
			UpdateDisplay();
		}

		protected void UpdateDisplay()
		{
			text.text = score.ToString().PadLeft(3, '0');
		}

		public void Show()
		{

		}

		public void Hide()
		{

		}
	}
}
