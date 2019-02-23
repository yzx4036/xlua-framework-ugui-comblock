/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	public class SceneLoader:MonoBehaviour 
	{
		[Tooltip("The ID of this specific SceneLoader. It should be a unique letter, different from any other SceneLoader identifier in the scene.")]
		public string identifier = "A";
		[Tooltip("The name of the scene that will be loaded when the player touches this SceneLoader.")]
		public string levelToLoad = "Level_1";
		[Tooltip("The identifier of the SceneLoader to load into in the new scene.")]
		public string loadPoint = "A";
		[Tooltip("Whether this SceneLoader sticks to the Left, Right, Top, or Bottom edge of the scene.")]
		public SceneBoundary.Edge edge;
		[Tooltip("Whether this SceneLoader functions as an Exit and Entrance, in which case the player can both load into this SceneLoader and touch it to load into a new scene; or Entrance Only, in which case the player can only be loaded into this SceneLoader from somewhere else, and not use it to enter new scenes.")]
		public SceneLoadType sceneLoadType;
		[Tooltip("The number of tiles tall or wide this SceneLoader is.")]
		public int tiles = 5;
		[Tooltip("If True, this SceneLoader will snap to the SceneBoundaries on the edges of the scene. If False, it can be freely positioned anywhere.")]
		public bool willSnapToSceneEdges = true;
		[Tooltip("Use True if this SceneLoader is attached to or triggered by other objects, like a Door.")]
		public bool isAttachedToGameObject = false;

		protected bool willDrawGizmos = true;
		protected BodyTextures textures;
		protected BodyTextures arrowTextures;

		protected class BodyTextures
		{
			public Texture2D right;
			public Texture2D left;
			public Texture2D top;
			public Texture2D bottom;
		}

		private bool hasTriggeredLoad;

		public enum SceneLoadType
		{
			ExitAndEntrance,
			EntranceOnly
		}

		void Awake()
		{
			RegisterWithRexSceneManager();
			if(sceneLoadType == SceneLoadType.EntranceOnly)
			{
				GetComponent<Collider2D>().isTrigger = false;
				gameObject.layer = LayerMask.NameToLayer("Terrain");
			}
		}

		void Start()
		{
			if(sceneLoadType == SceneLoadType.EntranceOnly) //The "Boundaries" layer is for actors tagged as "Player" only; it prevents them from walking off the side of a scene, while allowing enemies or projectiles to freely exit
			{
				gameObject.layer = LayerMask.NameToLayer("Boundaries");
			}
		}

		void OnTriggerEnter2D(Collider2D col) 
		{
			if(isAttachedToGameObject)
			{
				return;
			}

			if(col.tag == "Player" && sceneLoadType != SceneLoadType.EntranceOnly && col.GetComponent<RexActor>())
			{	
				if(!col.GetComponent<RexActor>().isBeingLoadedIntoNewScene)
				{
					if(edge == SceneBoundary.Edge.Left || edge == SceneBoundary.Edge.Right)
					{
						RexSceneManager.Instance.playerOffsetFromSceneLoader = col.transform.position.y - transform.position.y;
					}
					else if(edge == SceneBoundary.Edge.Top || edge == SceneBoundary.Edge.Bottom)
					{
						RexSceneManager.Instance.playerOffsetFromSceneLoader = col.transform.position.x - transform.position.x;
					}

					LoadSceneWithFadeOut();
				}
			}
		}

		public void LoadSceneWithFadeOut()
		{
			if(!hasTriggeredLoad)
			{
				GameManager.Instance.player.GetComponent<RexActor>().isBeingLoadedIntoNewScene = true;
				for(int i = 1; i < GameManager.Instance.players.Count; i ++)
				{
					RexActor additionalPlayer = GameManager.Instance.players[i];
					additionalPlayer.isBeingLoadedIntoNewScene = true;
				}

				hasTriggeredLoad = true;
				RexSceneManager.Instance.playerSpawnType = RexSceneManager.PlayerSpawnType.SceneLoader;
				RexSceneManager.Instance.loadPoint = loadPoint;
				RexSceneManager.Instance.LoadSceneWithFadeOut(levelToLoad, Color.black);
			}
		}

		public void SetWillDrawGizmos(bool _willDraw)
		{
			willDrawGizmos = _willDraw;
		}

		private void RegisterWithRexSceneManager()
		{
			RexSceneManager.Instance.RegisterSceneLoader(this);
		}

		private float GetDistanceToTerrain(Vector2 rayDirection)
		{
			float distanceToTerrain = 0.0f;
			RaycastHit2D hitInfo = new RaycastHit2D();
			int collisionLayerMask = 1 << LayerMask.NameToLayer("Terrain");

			float sideRayLength = 1000.0f;

			Vector2 origin = new Vector2(transform.position.x, transform.position.y);

			hitInfo = Physics2D.Raycast(origin, rayDirection, sideRayLength, collisionLayerMask);
			if(hitInfo.fraction > 0) 
			{
				distanceToTerrain = hitInfo.distance;
			}

			Debug.DrawRay(origin, rayDirection * sideRayLength, Color.red);

			return distanceToTerrain;
		}

		public void SnapToSceneBoundaries()
		{
			float distanceFromCameraSide = 0.5f;
			BoxCollider2D col = GetComponent<BoxCollider2D>();

			if(edge == SceneBoundary.Edge.Right)
			{
				float xPosition = transform.position.x;
				if(GameObject.Find("SceneBoundary_right") != null)
				{
					xPosition = GameObject.Find("SceneBoundary_right").transform.position.x + distanceFromCameraSide;
				}

				transform.position = new Vector3(xPosition + col.size.x / 2.0f, transform.position.y, 0.0f);
			}
			else if(edge == SceneBoundary.Edge.Left)
			{
				float xPosition = transform.position.x;
				if(GameObject.Find("SceneBoundary_left") != null)
				{
					xPosition = GameObject.Find("SceneBoundary_left").transform.position.x - distanceFromCameraSide;
				}

				transform.position = new Vector3(xPosition - col.size.x / 2.0f, transform.position.y, 0.0f);
			}
			else if(edge == SceneBoundary.Edge.Top)
			{
				float yPosition = transform.position.y;
				if(GameObject.Find("SceneBoundary_top") != null)
				{
					yPosition = GameObject.Find("SceneBoundary_top").transform.position.y + distanceFromCameraSide;
				}

				transform.position = new Vector3(transform.position.x, yPosition + col.size.y / 2.0f, 0.0f);
			}
			else if(edge == SceneBoundary.Edge.Bottom)
			{
				float yPosition = transform.position.y;
				if(GameObject.Find("SceneBoundary_bottom") != null)
				{
					yPosition = GameObject.Find("SceneBoundary_bottom").transform.position.y - distanceFromCameraSide;
				}

				transform.position = new Vector3(transform.position.x, yPosition - col.size.y / 2.0f, 0.0f);
			}
		}

		protected void PositionOnGrid()
		{
			if(tiles < 1)
			{
				tiles = 1;
			}

			float distanceFromCameraSide = GlobalValues.tileSize * 0.5f;
			float colliderWidth = GlobalValues.tileSize * 5.0f;

			BoxCollider2D col = GetComponent<BoxCollider2D>();
			if(edge == SceneBoundary.Edge.Left || edge == SceneBoundary.Edge.Right)
			{
				col.size = new Vector2(colliderWidth, tiles * GlobalValues.tileSize);

				float newPosition = Mathf.CeilToInt((transform.position.y - (distanceFromCameraSide)) / GlobalValues.tileSize);
				transform.position = new Vector3(transform.position.x, (newPosition * GlobalValues.tileSize) + (distanceFromCameraSide), transform.position.z);
			}
			else if(edge == SceneBoundary.Edge.Top || edge == SceneBoundary.Edge.Bottom)
			{
				col.size = new Vector2(tiles * GlobalValues.tileSize, colliderWidth);

				float newPosition = Mathf.CeilToInt((transform.position.x - (distanceFromCameraSide)) / GlobalValues.tileSize);
				transform.position = new Vector3((newPosition * GlobalValues.tileSize) + (distanceFromCameraSide), transform.position.y, transform.position.z);
			}
		}

		protected void LoadTextures()
		{
			#if UNITY_EDITOR
			textures = new BodyTextures();
			textures.top = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Top_Complete.png", typeof(Texture2D)) as Texture2D;
			textures.bottom = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Bottom_Complete.png", typeof(Texture2D)) as Texture2D;
			textures.left = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Left_Complete.png", typeof(Texture2D)) as Texture2D;
			textures.right = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Right_Complete.png", typeof(Texture2D)) as Texture2D;

			arrowTextures = new BodyTextures();
			arrowTextures.top = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Arrow_Top.png", typeof(Texture2D)) as Texture2D;
			arrowTextures.bottom = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Arrow_Bottom.png", typeof(Texture2D)) as Texture2D;
			arrowTextures.left = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Arrow_Left.png", typeof(Texture2D)) as Texture2D;
			arrowTextures.right = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/SceneLoader_Arrow_Right.png", typeof(Texture2D)) as Texture2D;
			#endif
		}

		#if UNITY_EDITOR
		public void DisplaySceneLoaderText(SceneLoader _sceneLoader, Vector3 _position)
		{
			if(textures == null)
			{
				LoadTextures();
			}

			Vector2 sceneNameOffset = Vector2.zero;
			Vector2 currentSceneIDOffset = Vector2.zero;
			Vector2 loadIntoSceneIDOffset = Vector2.zero;
			Vector2 helpLoadIntoOffset = Vector2.zero;
			Vector2 helpSceneOffset = Vector2.zero;
			Vector2 helpLoaderIDOffset = Vector2.zero;
			Vector2 helpIDOffset = Vector2.zero;

			Vector2 bodyTextureOffset = Vector2.zero;
			Vector2 capTextureOffset = Vector2.zero;
			Vector2 arrowTextureOffset = Vector2.zero;

			SceneBoundary.Edge side = _sceneLoader.edge;
			Texture2D bodyTexture = textures.right;

			float margin = 11.0f;

			switch(side)
			{
				case SceneBoundary.Edge.Top:
					currentSceneIDOffset = new Vector2(-2.5f, -8.0f);
					sceneNameOffset = new Vector2(-76.0f, -56.0f + margin);
					loadIntoSceneIDOffset = new Vector2(-3.0f, -96.0f + margin);

					helpLoaderIDOffset = new Vector2(-26.0f, 28.0f);
					helpLoadIntoOffset = new Vector2(-91.0f, -81.0f + margin);
					helpSceneOffset = new Vector2(-71.0f, -81.0f + margin);
					helpIDOffset = new Vector2(-31.0f, -112.0f + margin);

					bodyTextureOffset = new Vector2(-97.0f, -140.0f + margin);
					bodyTexture = textures.top;
					break;
				case SceneBoundary.Edge.Bottom:
					currentSceneIDOffset = new Vector2(-1.5f, -8.0f);
					sceneNameOffset = new Vector2(-76.0f, 43.0f - margin);
					loadIntoSceneIDOffset = new Vector2(-3.0f, 83.0f - margin);

					helpLoaderIDOffset = new Vector2(-26.0f, -40.0f);
					helpLoadIntoOffset = new Vector2(-91.0f, 18.5f - margin);
					helpSceneOffset = new Vector2(-71.0f, 18.5f - margin);
					helpIDOffset = new Vector2(-31.0f, 69.0f - margin);

					bodyTextureOffset = new Vector2(-97.0f, 22.0f - margin);
					bodyTexture = textures.bottom;
					break;
				case SceneBoundary.Edge.Left:
					currentSceneIDOffset = new Vector2(-1.5f, -8.0f);
					sceneNameOffset = new Vector2(-197.0f + margin, -7.0f);
					loadIntoSceneIDOffset = new Vector2(-236.0f + margin, -7.0f);

					helpLoaderIDOffset = new Vector2(-26.0f, 28.0f);
					helpLoadIntoOffset = new Vector2(-252.0f + margin, -32.0f);
					helpSceneOffset = new Vector2(-211.0f + margin, -32.0f);
					helpIDOffset = new Vector2(-233.0f + margin, -32.0f);

					bodyTextureOffset = new Vector2(-281.6f + margin, -27.6f);
					bodyTexture = textures.left;
					break;
				case SceneBoundary.Edge.Right:
					currentSceneIDOffset = new Vector2(-1.5f, -8.0f);
					sceneNameOffset = new Vector2(46.0f - margin, -7.0f);
					loadIntoSceneIDOffset = new Vector2(231.0f - margin, -7.0f);

					helpLoaderIDOffset = new Vector2(-26.0f, 28.0f);
					helpLoadIntoOffset = new Vector2(34.0f - margin, -32.0f);
					helpSceneOffset = new Vector2(55.0f - margin, -32.0f);
					helpIDOffset = new Vector2(216.0f - margin, -32.0f);

					bodyTextureOffset = new Vector2(24.75f - margin, -27.6f);
					bodyTexture = textures.right;
					break;
			}

			GUIStyle style = new GUIStyle();

			style.contentOffset = bodyTextureOffset;
			Handles.Label(_position, new GUIContent(bodyTexture), style);

			style.fontSize = 18;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.contentOffset = currentSceneIDOffset;
			style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			Handles.Label(_position, _sceneLoader.identifier, style);

			style.fontSize = 11;
			style.fontStyle = FontStyle.Normal;
			style.normal.textColor = new Color(0.125f, 0.125f, 0.125f, 1.0f);

			style.alignment = TextAnchor.MiddleLeft;

			style.contentOffset = sceneNameOffset;
			Handles.Label(_position, _sceneLoader.levelToLoad, style);

			style.fontStyle = FontStyle.Bold;

			style.contentOffset = loadIntoSceneIDOffset;
			Handles.Label(_position, _sceneLoader.loadPoint, style);

			style.fontSize = 10;
			style.fontStyle = FontStyle.BoldAndItalic;
			style.normal.textColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);

			style.contentOffset = helpLoaderIDOffset;
			Handles.Label(_position, "LOADER ID", style);

			style.fontSize = 9;
			style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			style.contentOffset = helpLoadIntoOffset;
			Handles.Label(_position, "To: ", style);

			style.fontStyle = FontStyle.Italic;

			style.contentOffset = helpSceneOffset;
			Handles.Label(_position, "Scene", style);

			style.contentOffset = helpIDOffset;
			Handles.Label(_position, "ID", style);
		}

		void OnDrawGizmos() 
		{
			gameObject.hideFlags = (GetComponent<Door>() == null) ? HideFlags.NotEditable : HideFlags.None;

			if(arrowTextures == null)
			{
				LoadTextures();
			}

			if(isAttachedToGameObject)
			{
				return;
			}

			Vector3 position = transform.position;
			Vector3 startPoint = new Vector3();
			Vector3 endPoint = new Vector3();
			Texture2D texture = arrowTextures.right;

			PositionOnGrid();

			if(willSnapToSceneEdges)
			{
				SnapToSceneBoundaries();
			}

			BoxCollider2D collider = GetComponent<BoxCollider2D>();

			GUIStyle iconStyle = new GUIStyle();
			iconStyle.alignment = TextAnchor.MiddleCenter;
			iconStyle.fontStyle = FontStyle.Bold;
			iconStyle.fontSize = 12;
			iconStyle.normal.textColor = new Color(0.35f, 0.35f, 1.0f, 1.0f);

			switch(edge)
			{
				case SceneBoundary.Edge.Left:
					if(willDrawGizmos)
					{
						iconStyle.contentOffset = new Vector2(-6.0f, -10.0f);
						texture = arrowTextures.left;
						Handles.Label(position, new GUIContent(texture), iconStyle);

						iconStyle.contentOffset = new Vector2(0.0f, -6.0f);
						Handles.Label(position, identifier, iconStyle);
					}

					position.x = position.x + GetComponent<Collider2D>().bounds.size.x / 2.0f;
					startPoint = new Vector3(position.x, position.y - GetComponent<Collider2D>().bounds.size.y / 2.0f, 0.0f);
					endPoint = new Vector3(position.x, position.y + GetComponent<Collider2D>().bounds.size.y / 2.0f, 0.0f);
					Handles.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
					Handles.DrawDottedLine(startPoint, endPoint, 2.0f);
					break;
				case SceneBoundary.Edge.Right:
					if(willDrawGizmos)
					{
						iconStyle.contentOffset = new Vector2(-2.0f, -10.0f);
						texture = arrowTextures.right;
						Handles.Label(position, new GUIContent(texture), iconStyle);

						iconStyle.contentOffset = new Vector2(0.0f, -6.0f);
						Handles.Label(position, identifier, iconStyle);
					}

					position.x = position.x - collider.bounds.size.x / 2.0f;
					startPoint = new Vector3(position.x, position.y - collider.bounds.size.y / 2.0f, 0.0f);
					endPoint = new Vector3(position.x, position.y + collider.bounds.size.y / 2.0f, 0.0f);
					Handles.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
					Handles.DrawDottedLine(startPoint, endPoint, 2.0f);
					break;
				case SceneBoundary.Edge.Top:
					if(willDrawGizmos)
					{
						iconStyle.contentOffset = new Vector2(-6.0f, -11.0f);
						texture = arrowTextures.top;
						Handles.Label(position, new GUIContent(texture), iconStyle);

						iconStyle.contentOffset = new Vector2(-1.0f, -6.0f);
						Handles.Label(position, identifier, iconStyle);
					}

					position.y = position.y -collider.bounds.size.y / 2.0f;
					startPoint = new Vector3(position.x - collider.bounds.size.x / 2.0f, position.y, 0.0f);
					endPoint = new Vector3(position.x + collider.bounds.size.x / 2.0f, position.y, 0.0f);
					Handles.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
					Handles.DrawDottedLine(startPoint, endPoint, 2.0f);
					break;
				case SceneBoundary.Edge.Bottom:
					if(willDrawGizmos)
					{
						iconStyle.contentOffset = new Vector2(-6.5f, -4.0f);
						texture = arrowTextures.bottom;
						Handles.Label(position, new GUIContent(texture), iconStyle);

						iconStyle.contentOffset = new Vector2(-1.0f, -6.0f);
						Handles.Label(position, identifier, iconStyle);
					}

					position.y = position.y + collider.bounds.size.y / 2.0f;
					startPoint = new Vector3(position.x - collider.bounds.size.x / 2.0f, position.y, 0.0f);
					endPoint = new Vector3(position.x + collider.bounds.size.x / 2.0f, position.y, 0.0f);
					Handles.color = new Color(1.0f, 0.25f, 0.25f, 1.0f);
					Handles.DrawDottedLine(startPoint, endPoint, 2.0f);
					break;
			}
		}
		#endif
	}

}
