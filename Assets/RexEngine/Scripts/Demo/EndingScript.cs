/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class EndingScript:LevelScript 
{
	public List<GameObject> paragraphs;

	protected int currentParagraph = 0;
	protected bool hasClickedThroughToTitle = false;

	void Awake() 
	{
		
	}

	/*void Start() 
	{
		GameManager.Instance.MakePlayersInactive();
		UIManager.Instance.MakeUIInactive();

		RexTouchInput rexTouchInput = GameManager.Instance.player.GetComponent<RexTouchInput>();
		if(rexTouchInput != null)
		{
			rexTouchInput.ToggleTouchInterface(false);
		}

		GameManager.Instance.player.gameObject.SetActive(false);
		GameManager.Instance.player.hp.bar.gameObject.SetActive(false);
		ScoreManager.Instance.text.gameObject.SetActive(false);
		PauseManager.Instance.isPauseEnabled = false;
		LivesManager.Instance.Hide();

		if(GameManager.Instance.player.mpProperties.mp)
		{
			GameManager.Instance.player.mpProperties.mp.gameObject.SetActive(false);
		}
	}*/
	
	void Update() 
	{
		if(!hasClickedThroughToTitle)
		{
			if(GameManager.Instance.input.isJumpButtonDownThisFrame || Input.GetMouseButtonDown(0))
			{
				AdvanceText();
			}
		}
	}

	protected void AdvanceText()
	{
		currentParagraph ++;
		if(currentParagraph <= paragraphs.Count - 1)
		{
			paragraphs[currentParagraph].SetActive(true);

			if(currentParagraph == paragraphs.Count - 1)
			{
				for(int i = 0; i < paragraphs.Count - 1; i ++)
				{
					paragraphs[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			hasClickedThroughToTitle = true;
			ExitScene();
		}
	}
}
