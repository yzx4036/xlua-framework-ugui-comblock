using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class LevelScript:MonoBehaviour 
	{
		public ObjectToggle toggleOnSceneStart;
		public ObjectToggle toggleOnNewSceneLoad;
		public LoadNewScene loadNewScene;

		protected bool isExiting = false;

		[System.Serializable]
		public class ObjectToggle
		{
			public Toggle player;
			public Toggle ui;
		}

		[System.Serializable]
		public class LoadNewScene
		{
			public string sceneToLoad = "Demo_1";
			public AdvanceType advanceType = AdvanceType.Manual;
		}

		public enum AdvanceType
		{
			Manual,
			OnScreenClick,
			OnKeyPress,
			All
		}

		public enum Toggle
		{
			Active,
			Inactive,
			NoChange
		}

		void Start()
		{
			ToggleActiveObjectsAtSceneStart();
		}

		void Update() 
		{
			if(!isExiting && loadNewScene.advanceType != AdvanceType.Manual)
			{
				if((GameManager.Instance.input.isJumpButtonDownThisFrame && (loadNewScene.advanceType == AdvanceType.OnKeyPress || loadNewScene.advanceType == AdvanceType.All)) || (Input.GetMouseButtonDown(0) && (loadNewScene.advanceType == AdvanceType.OnScreenClick || loadNewScene.advanceType == AdvanceType.All)))
				{
					ExitScene();
				}
			}
		}

		public void ToggleActiveObjectsAtSceneStart() 
		{
			if(toggleOnSceneStart.player == Toggle.Inactive)
			{
				GameManager.Instance.MakePlayersInactive();
			}
			else if(toggleOnSceneStart.player == Toggle.Active)
			{
				GameManager.Instance.MakePlayersActive();
			}

			if(toggleOnSceneStart.ui == Toggle.Inactive)
			{
				UIManager.Instance.MakeUIInactive();
			}
			else if(toggleOnSceneStart.ui == Toggle.Active)
			{
				UIManager.Instance.MakeUIActive();
			}
		}

		public void ToggleActiveObjectsAtSceneEnd() 
		{
			if(toggleOnNewSceneLoad.player == Toggle.Inactive)
			{
				GameManager.Instance.MakePlayersInactive();
			}
			else if(toggleOnNewSceneLoad.player == Toggle.Active)
			{
				GameManager.Instance.MakePlayersActive();
			}

			if(toggleOnNewSceneLoad.ui == Toggle.Inactive)
			{
				UIManager.Instance.MakeUIInactive();
			}
			else if(toggleOnNewSceneLoad.ui == Toggle.Active)
			{
				UIManager.Instance.MakeUIActive();
			}
		}

		public void ResetData()
		{
			DataManager.Instance.ResetData();
			ObjectDisablerManager.Instance.ResetAll();
			LivesManager.Instance.ResetLivesToStartingValue();
			LivesManager.Instance.ResetSavedValues();
		}

		public void ExitScene()
		{
			isExiting = true;

			OnExitScene();

			StartCoroutine("ExitSceneCoroutine");
		}

		protected IEnumerator ExitSceneCoroutine()
		{
			if(toggleOnNewSceneLoad.player == Toggle.Active)
			{
				GameManager.Instance.MakePlayersActive(); 
			}
			else if(toggleOnNewSceneLoad.player == Toggle.Inactive)
			{
				GameManager.Instance.MakePlayersInactive(); 
			}

			ScreenFade.Instance.Fade(ScreenFade.FadeType.Out, ScreenFade.FadeDuration.Short, Color.white);

			yield return new WaitForSeconds(0.5f);

			RexSceneManager.Instance.LoadSceneWithFadeOut(loadNewScene.sceneToLoad, Color.white, false);

			yield return new WaitForSeconds(0.25f);

			if(toggleOnNewSceneLoad.ui == Toggle.Active)
			{
				UIManager.Instance.MakeUIActive();
			}
			else if(toggleOnNewSceneLoad.ui == Toggle.Inactive)
			{
				UIManager.Instance.MakeUIInactive();
			}
		}

		protected virtual void OnExitScene()
		{

		}
	}
}
