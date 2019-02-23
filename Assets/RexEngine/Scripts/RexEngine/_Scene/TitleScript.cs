/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class TitleScript:LevelScript 
	{
		void Awake() 
		{

		}

		void Start() 
		{
			RexCamera rexCamera = Camera.main.GetComponent<RexCamera>();
			if(rexCamera)
			{
				rexCamera.willTrackFocusObject = true;
				rexCamera.scrolling.willScrollHorizontally = true;
			}

			ToggleActiveObjectsAtSceneStart();
		}

		protected override void OnExitScene()
		{
			ResetData();
		}
	}
}
