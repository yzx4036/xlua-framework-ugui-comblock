/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RexEngine;

public class Demo_10_LevelScript:MonoBehaviour 
{
	public TextMesh victoryText;
	public TextMesh scoreText;
	public TextMesh deathsText;
	public string sceneToLoad = "Demo_Ending";
	public bool willDisableUI = true;

	public void RunEnding()
	{
		Debug.Log("End!");
		StartCoroutine("EndingCoroutine");
	}

	protected IEnumerator EndingCoroutine()
	{
		RexSoundManager.Instance.Fade();

		for(int i = 0; i < GameManager.Instance.players.Count; i ++)
		{
			RexActor player = GameManager.Instance.players[i];
			player.slots.input.isEnabled = false;
			player.CancelActivePowerups();
			player.GetComponent<Booster>().OnBlueprintFound();

			if(willDisableUI)
			{
				player.hp.bar.gameObject.SetActive(false);
				if(player.mpProperties.mp)
				{
					player.mpProperties.mp.bar.gameObject.SetActive(false);
				}
			}
		}

		if(willDisableUI)
		{
			ScoreManager.Instance.text.gameObject.SetActive(false);
			PauseManager.Instance.isPauseEnabled = false;
			LivesManager.Instance.Hide();

			if(TimerManager.Instance.settings.isEnabled)
			{
				TimerManager.Instance.StopTimer();
			}
		}

		Debug.Log("BLUEPRINT RECOVERED!");
		ScreenShake.Instance.Shake(); 
		victoryText.gameObject.SetActive(true);
		yield return new WaitForSeconds(2.0f);

		if(scoreText != null)
		{
			string scoreString = "Total bolts collected: " + ScoreManager.Instance.score.ToString();
			scoreText.text = scoreString;
			scoreText.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.0f);
		}

		if(deathsText)
		{
			string deathString = "Total deaths: " + LivesManager.Instance.deaths.ToString();
			deathsText.text = deathString;
			deathsText.gameObject.SetActive(true);
		}

		yield return new WaitForSeconds(5.0f);

		RexSceneManager.Instance.LoadSceneWithFadeOut(sceneToLoad);
	}
}
