/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class GameOverScript:LevelScript 
{
	void Start() 
	{
		ToggleActiveObjectsAtSceneStart();
		LivesManager.Instance.ResetSavedValues();
	}

	void Update() 
	{
		if(GameManager.Instance.input.isJumpButtonDownThisFrame || Input.GetMouseButtonDown(0))
		{
			LoadPostGameOverScene();
		}
	}

	protected void LoadPostGameOverScene()
	{
		RexSceneManager.Instance.LoadSceneWithFadeOut(LivesManager.Instance.settings.postGameOverScene, Color.white);
	}
}
