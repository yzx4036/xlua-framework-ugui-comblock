/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	public class Setup:MonoBehaviour 
	{
		public bool willUseMinimalSingletons = false;

		void Awake()
		{
			#if UNITY_EDITOR
			if(Time.frameCount == 0 && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if(GameObject.Find("Singletons") != null)
				{
					EditorApplication.isPlaying = false;
					EditorUtility.DisplayDialog("Uh-oh", "There's already a Singletons object here. Please delete it from the stage before pressing Play.", "Gotcha");
					return;
				}
			}
			#endif

			bool isSetupAlreadyComplete = false;
			if(willUseMinimalSingletons)
			{
				if(GameObject.Find("Singletons") == null)
				{
					GameObject prefab = Resources.Load("System/Singletons_Minimal") as GameObject;
					GameObject newObject = Instantiate(prefab).gameObject;
					newObject.name = "Singletons";
					DontDestroyOnLoad(newObject);
				}
				else
				{
					Destroy(gameObject);
					return;
				}
			}
			else
			{
				if(GameObject.Find("Singletons") == null)
				{
					GameObject prefab = Resources.Load("System/Singletons") as GameObject;
					GameObject newObject = Instantiate(prefab).gameObject;
					newObject.name = "Singletons";
					DontDestroyOnLoad(newObject);
				}
				else
				{
					isSetupAlreadyComplete = true;
				}

				if(isSetupAlreadyComplete)
				{
					Destroy(gameObject);
					return;
				}

				if(GameObject.Find("UI") == null)
				{
					GameObject prefab = Resources.Load("System/UI") as GameObject;
					GameObject newObject = Instantiate(prefab).gameObject;
					newObject.name = "UI";
					DontDestroyOnLoad(newObject);
				}

				GameManager.Instance.players = new List<RexActor>();

				GameObject existingPlayerObject = GameObject.Find("Player");
				RexActor existingPlayer = null;
				if(existingPlayerObject != null)
				{
					existingPlayer = existingPlayerObject.GetComponent<RexActor>();
				}

				if(existingPlayer != null)
				{
					GameManager.Instance.player = existingPlayer;
					GameManager.Instance.players.Add(existingPlayer);
				}

				if(GameManager.Instance.player == null)
				{
					int totalPlayers = GameManager.Instance.settings.numberOfPlayers;
					for(int i = 0; i < totalPlayers; i ++)
					{
						GameObject prefab = GameManager.Instance.settings.playerPrefabs[i].gameObject;
						GameObject newObject = Instantiate(prefab).gameObject;	
						newObject.name = newObject.name.Split('(')[0];
						DontDestroyOnLoad(newObject);

						if(i == 0)
						{
							GameManager.Instance.player = newObject.GetComponent<RexActor>();
						}

						GameManager.Instance.players.Add(newObject.GetComponent<RexActor>());

						newObject.layer = LayerMask.NameToLayer("Player");
						newObject.tag = "Player";
						foreach(Transform childObject in newObject.GetComponentsInChildren<Transform>())
						{
							if(childObject.GetComponent<RexController>() != null)
							{
								newObject.layer = LayerMask.NameToLayer("Default");
								childObject.tag = "Untagged";
							}
							else if(childObject.GetComponent<Attack>() == null)
							{
								childObject.gameObject.layer = LayerMask.NameToLayer("Player");
								childObject.tag = "Player";
							}
							else
							{
								if(childObject.tag != "Reflector")
								{
									childObject.gameObject.layer = LayerMask.NameToLayer("Default");
									childObject.tag = "Untagged";
								}
							}

							SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
							if(spriteRenderer)
							{
								spriteRenderer.sortingLayerName = "Sprites";
							}
						}
					}
				}

				if(GameObject.Find("Cameras") == null)
				{
					GameObject prefab = Resources.Load("System/Cameras") as GameObject;
					GameObject newObject = Instantiate(prefab).gameObject;
					newObject.name = "Cameras";
					DontDestroyOnLoad(newObject);
				}

				foreach(SceneBoundary boundary in GameObject.FindObjectsOfType<SceneBoundary>())
				{
					boundary.Init();
				}

				RexCameraBase rexCamera = Camera.main.GetComponent<RexCameraBase>();
				if(rexCamera != null)
				{
					if(rexCamera.usePlayerAsTarget)
					{
						rexCamera.SetFocusObject(GameManager.Instance.player.transform);
					}
				}
			}

			RexSceneManager.Instance.player = GameManager.Instance.player;
			RexSceneManager.Instance.MovePlayerToSpawnPoint(); 

			RexCameraBase mainCamera = Camera.main.GetComponent<RexCameraBase>();
			if(mainCamera != null)
			{
				mainCamera.CenterOnPlayer();
			}

			ScreenFade.Instance.Fade(ScreenFade.FadeType.Out, ScreenFade.FadeDuration.Immediate);
			StartCoroutine("FadeScreenInCoroutine");
		}

		protected IEnumerator FadeScreenInCoroutine()
		{
			yield return new WaitForEndOfFrame();

			ScreenFade.Instance.Fade(ScreenFade.FadeType.In, ScreenFade.FadeDuration.Short);
			Destroy(gameObject);
		}
	}
}
