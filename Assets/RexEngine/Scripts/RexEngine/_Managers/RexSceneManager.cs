/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	public class RexSceneManager:MonoBehaviour 
	{
		public string startingLevel;

		[System.NonSerialized]
		public string previousLevel;

		[HideInInspector]
		public string levelToLoad;

		private bool willLoadSceneOnFadeoutComplete;

		[System.NonSerialized]
		public PlayerSpawnType playerSpawnType = PlayerSpawnType.SpawnPoint;

		[System.NonSerialized]
		public string loadPoint;

		[HideInInspector]
		public bool isLoadingNewScene;

		//Used internally to keep the player's position consistent between scenes
		[System.NonSerialized]
		public float playerOffsetFromSceneLoader;

		//Used internally to keep the player's position consistent between scenes
		[System.NonSerialized]
		public Vector3 playerPositionOnSceneExit;

		public delegate void OnPlayerPositionedDelegate(); //Called when the player is loaded into a new scene and successfully positioned at their spawn/load point
		public event OnPlayerPositionedDelegate OnPlayerPositioned;

		protected bool willNextSceneFadeIn = true;

		protected bool isFirstLoad = true; //Used internally to determine if you're in the Editor and you just hit Play

		protected string sceneToUnload;
		protected bool isNewSceneLoaded = true;

		protected List<Vector2> playerColliderSizes;

		[HideInInspector]
		public RexActor player;

		public List<SceneLoader> sceneLoaders;

		public enum PlayerSpawnType
		{
			SceneLoader,
			SpawnPoint
		}

		private static RexSceneManager instance = null;
		public static RexSceneManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					Debug.Log("RexSceneManager :: Instantiate");
					GameObject go = new GameObject();
					instance = go.AddComponent<RexSceneManager>();
					go.name = "Scene Manager";
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

			isLoadingNewScene = true;
			sceneLoaders = new List<SceneLoader>();
		}

		void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		void Start()
		{
		}

		//Fades out the screen and loads in a new scene
		public void LoadSceneWithFadeOut(string level, Color fadeColor = default(Color), bool willStartFreshFade = true, bool _willNextSceneFadeIn = true)
		{
			StopAllCoroutines();

			for(int j = 0; j < GameManager.Instance.players.Count; j ++)
			{
				RexActor additionalPlayer = GameManager.Instance.players[j];
				additionalPlayer.isBeingLoadedIntoNewScene = true;
			}

			playerPositionOnSceneExit = player.transform.position;
			willNextSceneFadeIn = _willNextSceneFadeIn;
			levelToLoad = level;
			previousLevel = SceneManager.GetActiveScene().name;
			willLoadSceneOnFadeoutComplete = true;
			isLoadingNewScene = true;
			PurgeSceneLoaders();
			ScreenFade.Instance.Fade(ScreenFade.FadeType.Out, ScreenFade.FadeDuration.Medium, fadeColor, willStartFreshFade);

			StartCoroutine("LoadNewSceneCoroutine");
		}

		public void MovePlayerToSpawnPoint()
		{
			StartCoroutine("MovePlayerToSpawnPointCoroutine");
		}

		public string GetCurrentLevel()
		{
			return (levelToLoad == "") ? SceneManager.GetActiveScene().name : levelToLoad;
		}

		public string GetLoadPoint()
		{
			return loadPoint;
		}

		public void RegisterSceneLoader(SceneLoader sceneLoader)
		{
			sceneLoaders.Add(sceneLoader);
		}

		public void UnregisterSceneLoader(SceneLoader sceneLoader)
		{
			sceneLoaders.Remove(sceneLoader);
		}

		protected IEnumerator MovePlayerToSpawnPointCoroutine()
		{
			yield return new WaitForEndOfFrame();

			bool willSearchForSpawnPoints = true;

			#if UNITY_EDITOR
			if(isFirstLoad)
			{
				isFirstLoad = false;
				willSearchForSpawnPoints = false;
			}
			#endif

			Collider2D playerCollider = player.GetComponent<Collider2D>();
			if(playerSpawnType == PlayerSpawnType.SpawnPoint) //The player is loading into a spawn point or checkpoint
			{
				Vector3 loadPoint = new Vector3(0, 0, 0);

				bool isSpawningIntoCheckpoint = false;
				if(willSearchForSpawnPoints)
				{
					foreach(Checkpoint checkpoint in FindObjectsOfType<Checkpoint>())
					{
						if(checkpoint.id == LivesManager.Instance.lastCheckpointID) //We found the checkpoint the player should spawn at
						{
							isSpawningIntoCheckpoint = true;
							GameManager.Instance.OnPlayerSpawn();
							loadPoint = checkpoint.transform.position;

							for(int j = 0; j < GameManager.Instance.players.Count; j ++)
							{
								RexActor additionalPlayer = GameManager.Instance.players[j];
								if(additionalPlayer.slots.controller)
								{
									additionalPlayer.slots.controller.FaceDirection(checkpoint.playerSpawnDirection);
								}
							}
						}
					}
				}

				if(!isSpawningIntoCheckpoint) //If the scene we're loading doesn't have a checkpoint, check for a spawn point or simply load them in at Vector3.zero
				{
					GameObject playerSpawnPoint = GameObject.Find("PlayerSpawnPoint");
					Vector3 spawnPosition = (playerSpawnPoint) ? playerSpawnPoint.transform.position : Vector3.zero;
					//Debug.Log("Spawn position is: " + spawnPosition);

					for(int j = 0; j < GameManager.Instance.players.Count; j ++)
					{
						RexActor additionalPlayer = GameManager.Instance.players[j];
						if(additionalPlayer != null && additionalPlayer.slots.controller)
						{
							additionalPlayer.slots.controller.FaceDirection(Direction.Horizontal.Right);
						}
					}

					loadPoint = spawnPosition;
				}

				for(int j = 0; j < GameManager.Instance.players.Count; j ++)
				{
					RexActor additionalPlayer = GameManager.Instance.players[j];
					if(additionalPlayer != null)
					{
						additionalPlayer.GetComponent<RexActor>().SetPosition(new Vector2(loadPoint.x, loadPoint.y));
						additionalPlayer.GetComponent<RexActor>().loadedIntoScenePoint = loadPoint;
					}
				}
			}
			else //The player went through a SceneLoader and is loading into a new room
			{
				for(int i = 0; i < sceneLoaders.Count; i++)
				{
					if(sceneLoaders[i].identifier == loadPoint)
					{
						SceneLoader sceneLoader = sceneLoaders[i];

						Vector3 playerMovementDuringLoad = playerPositionOnSceneExit - new Vector3(0, 0, 0) - player.transform.position;
						Vector3 position = sceneLoader.transform.position;
						float offset = (sceneLoader.isAttachedToGameObject) ? 0.0f : 1.0f;
						Vector2 additionalOffset = (sceneLoader.isAttachedToGameObject) ? Vector2.zero : new Vector2(playerOffsetFromSceneLoader - playerMovementDuringLoad.x, playerOffsetFromSceneLoader - playerMovementDuringLoad.y);

						if(sceneLoader.edge == SceneBoundary.Edge.Left)
						{
							position = new Vector3(sceneLoader.GetComponent<Collider2D>().bounds.max.x + offset + (playerColliderSizes[0].x / 2.0f) - playerCollider.offset.x, position.y + additionalOffset.y, position.z);
						}
						else if(sceneLoader.edge == SceneBoundary.Edge.Right)
						{
							position = new Vector3(sceneLoader.GetComponent<Collider2D>().bounds.min.x - offset - (playerColliderSizes[0].x / 2.0f) + playerCollider.offset.x, position.y + additionalOffset.y, position.z);
						}
						else if(sceneLoader.edge == SceneBoundary.Edge.Top)
						{
							position = new Vector3(position.x + additionalOffset.x, sceneLoader.GetComponent<Collider2D>().bounds.min.y - offset - (playerColliderSizes[0].y / 2.0f) + playerCollider.offset.y, position.z);
						}
						else if(sceneLoader.edge == SceneBoundary.Edge.Bottom)
						{
							position = new Vector3(position.x + additionalOffset.x, sceneLoader.GetComponent<Collider2D>().bounds.max.y + offset + (playerColliderSizes[0].y / 2.0f) - playerCollider.offset.y, position.z);
						}
						else if(sceneLoader.edge == SceneBoundary.Edge.None)
						{
							position = new Vector3(position.x, position.y, position.z);
						}

						if(sceneLoader.isAttachedToGameObject)
						{
							Door door = sceneLoader.GetComponent<Door>();
							if(door != null)
							{
								for(int j = 0; j < GameManager.Instance.players.Count; j ++)
								{
									RexActor additionalPlayer = GameManager.Instance.players[j];
									door.ExitDoor(additionalPlayer);
								}
							}
						}

						for(int j = 0; j < GameManager.Instance.players.Count; j ++)
						{
							RexActor additionalPlayer = GameManager.Instance.players[j];
							additionalPlayer.loadedIntoScenePoint = position;
							additionalPlayer.SetPosition(position);

							if(sceneLoader.edge == SceneBoundary.Edge.Left || sceneLoader.edge == SceneBoundary.Edge.Right)
							{
								ClampActorToSceneLoader(additionalPlayer, sceneLoader.GetComponent<BoxCollider2D>());
							}
						}

						//player.OnLoadedIntoNewScene();

						break;
					}
				}
			}

			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.01f);

			for(int j = 0; j < GameManager.Instance.players.Count; j ++)
			{
				RexActor additionalPlayer = GameManager.Instance.players[j];
				if(additionalPlayer != null)
				{
					Collider2D collider = additionalPlayer.GetComponent<Collider2D>();
					Collider2D controllerCollider = additionalPlayer.slots.controller.GetComponent<Collider2D>();

					if(collider) //We toggled the player's collider off to prevent unwanted collisions as the scene loaded; here, we toggle it back on
					{
						collider.enabled = true;
					}

					if(controllerCollider) //We toggled the player's collider off to prevent unwanted collisions as the scene loaded; here, we toggle it back on
					{
						controllerCollider.enabled = true;
					}

					if(additionalPlayer.timeStop.isTimeStopped)
					{
						additionalPlayer.StartTime();
					}

					additionalPlayer.isBeingLoadedIntoNewScene = false;
					isLoadingNewScene = false;
				}
			}

			if(OnPlayerPositioned != null)
			{
				OnPlayerPositioned();
			}
		}

		public void DestroyScene()
		{
			//Projectiles need to be cleared separately, since they auto-parent themselves to the Projectiles GameObject which is in DontDestroyOnLoad
			GameObject projectiles = GameObject.Find("Projectiles");
			if(projectiles)
			{
				foreach(Projectile projectile in projectiles.GetComponentsInChildren<Projectile>())
				{
					if(projectile.willDestroyWhenSceneChanges)
					{
						projectile.Clear();
					}
				}
			}

			foreach(GameObject foundObject in GameObject.FindObjectsOfType<GameObject>())
			{
				if(foundObject.scene.name != "DontDestroyOnLoad")
				{
					Destroy(foundObject);
				}
			}
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			isNewSceneLoaded = true;
		}

		protected IEnumerator LoadNewSceneCoroutine()
		{
			for(int i = 0; i < GameManager.Instance.players.Count; i ++)
			{
				RexActor additionalPlayer = GameManager.Instance.players[i];
				additionalPlayer.StopTime();
			}

			float duration = ScreenFade.Instance.currentFadeDuration; //Wait until the screen fades out completely
			yield return new WaitForSeconds(duration );

			if(willLoadSceneOnFadeoutComplete) //Destroy the existing scene, and load the next one
			{
				bool willLoadAsync = true;
				#if (UNITY_WEBGL || UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
				willLoadAsync = false;
				#endif

				willLoadSceneOnFadeoutComplete = false;
				PhysicsManager.Instance.isSceneLoading = true;

				playerColliderSizes = new List<Vector2>();
				for(int j = 0; j < GameManager.Instance.players.Count; j ++)
				{
					RexActor additionalPlayer = GameManager.Instance.players[j];
					Collider2D playerCollider = additionalPlayer.slots.collider;
					playerColliderSizes.Add(new Vector2(playerCollider.bounds.size.x, playerCollider.bounds.size.y));
					playerCollider.enabled = false; //Toggle the player's collider off to prevent unwanted collisions as the new scene loads (but before the player has been moved to the proper position in the new scene)
					Collider2D controllerCollider = additionalPlayer.slots.controller.GetComponent<Collider2D>();
					if(controllerCollider) 
					{
						controllerCollider.enabled = false;
					}
				}

				yield return new WaitForEndOfFrame();

				isNewSceneLoaded = false;

				if(willLoadAsync)
				{
					DestroyScene();
					AsyncOperation async = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive); //Use Additive to circumvent a Unity bug that drops controller/keyboard inputs between scenes
					async.allowSceneActivation = false;

					while(!async.isDone)
					{
						yield return new WaitForEndOfFrame();

						if(async.progress >= 0.9f) //0.9 is as far as it needs to go for loading the scene itself; because of a Unity bug, it often won't hit 1.0
						{				
							async.allowSceneActivation = true;
							break;
						}
					}
				}
				else
				{
					SceneManager.LoadScene(levelToLoad); //Use Additive to circumvent a Unity bug that drops controller/keyboard inputs between scenes
					yield return new WaitForSeconds(0.05f);
				}

				yield return new WaitForEndOfFrame();

				StartCoroutine("OnSceneLoadComplete");
			}
		}

		protected void ClampActorToSceneLoader(RexActor actor, BoxCollider2D sceneLoaderCollider)
		{
			float loadPointColliderBottom = sceneLoaderCollider.bounds.min.y; //Prevent the player from moving beyond the boundaries of the SceneLoader
			float loadPointColliderTop = sceneLoaderCollider.bounds.max.y; //Prevent the player from moving beyond the boundaries of the SceneLoader
			BoxCollider2D playerCollider = actor.GetComponent<BoxCollider2D>();
			if(actor.transform.position.y < loadPointColliderBottom + playerCollider.bounds.size.y / 2.0f + playerCollider.offset.y)
			{
				actor.SetPosition(new Vector2(actor.transform.position.x, loadPointColliderBottom + playerCollider.bounds.size.y / 2.0f + playerCollider.offset.y));
			}
			else if(actor.transform.position.y > loadPointColliderTop - playerCollider.bounds.size.y / 2.0f - playerCollider.offset.y)
			{
				actor.SetPosition(new Vector2(actor.transform.position.x, loadPointColliderTop - playerCollider.bounds.size.y / 2.0f - playerCollider.offset.y));
			}
		}

		protected IEnumerator OnSceneLoadComplete()
		{
			yield return new WaitForSeconds(0.1f);

			while(!isNewSceneLoaded)
			{
				yield return new WaitForEndOfFrame();
			}

			sceneToUnload = SceneManager.GetActiveScene().name;
			SceneManager.UnloadSceneAsync(sceneToUnload);

			yield return new WaitForEndOfFrame();

			System.GC.Collect();

			if(willNextSceneFadeIn) //Fade the new scene in
			{
				yield return new WaitForSeconds(0.2f);

				StartCoroutine("FadeSceneInCoroutine");
			}

			/*if(SceneManager.GetSceneByName(sceneToUnload) != null)
			{
				SceneManager.MergeScenes(SceneManager.GetSceneByName(sceneToUnload), SceneManager.GetSceneByName(levelToLoad));
			}*/

			PhysicsManager.Instance.isSceneLoading = false;

			yield return new WaitForSeconds(0.01f);

			foreach(SceneBoundary boundary in GameObject.FindObjectsOfType<SceneBoundary>())
			{
				boundary.Init();
			}

			//Move the player to the spawn point or to the SceneLoader that corresponds to the SceneLoader they exited the last scene from, if applicable
			MovePlayerToSpawnPoint();

			RexCameraBase camera = Camera.main.GetComponent<RexCameraBase>();
			if(camera != null && camera.usePlayerAsTarget)
			{
				camera.CenterOnPlayer();
			}
		}

		protected IEnumerator FadeSceneInCoroutine()
		{
			yield return new WaitForSeconds(0.15f);

			ScreenFade.Instance.Fade(ScreenFade.FadeType.In, ScreenFade.FadeDuration.Short);
		}

		//Called automatically when a scene is destroyed; cleans up the SceneLoaders from the scene
		private void PurgeSceneLoaders()
		{
			for(int i = sceneLoaders.Count - 1; i >= 0; i--)
			{
				sceneLoaders.RemoveAt(i);
			}
		}
	}
}
