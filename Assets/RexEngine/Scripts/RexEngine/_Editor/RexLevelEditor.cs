#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
#endif

using RexEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public class RexLevelEditor:EditorWindow
{
	protected List<Transform> playerSpawnPoints;
	protected LevelManager levelManager;
	protected AudioClip musicTrack;
	protected bool isPauseEnabledInScene;
	protected bool isRexGUILocked;

	protected Vector3 playerSpawnPointPosition;
	protected SceneBoundary.Edge currentSceneLoaderSide;
	protected List<SceneLoader> sceneLoaders;
	protected Material gizmoMaterial;
	protected Material lineMaterial;
	protected int tempHandleID;
	protected int currentHandleID;
	protected Tool previousTool;
	protected GameObject lastSelectedGameObject;
	protected SceneLoader currentLoader;
	protected bool wasItemSelectedLastFrame = false;
	protected Vector2 scrollPosition;

	protected bool showFieldCreate = false;
	protected bool showFieldScene = false;

	protected float handleSizeMultiplier = 0.25f;
	protected LineDirection currentLineDirection;

	protected SceneBoundary.SceneBoundaries sceneBoundaries;
	protected SceneBoundary currentSceneBoundary;
	protected bool drawSelectedButton = false;

	protected bool didSelectItemThisFrame = false;

	protected Scene previousScene;

	public enum LineDirection
	{
		Horizontal,
		Vertical
	}

	void OnEnable()
	{
		gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		playerSpawnPoints = new List<Transform>();
		GetPlayerSpawnPoints();
		GetSceneLoader();
		isRexGUILocked = EditorPrefs.GetBool("com.skyty.rexengine.IsRexGUILocked");

		if(levelManager != null)
		{
			isPauseEnabledInScene = levelManager.isPauseEnabledInScene;
			musicTrack = levelManager.musicTrack;
		}

		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	protected void OnSceneChanged()
	{
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		playerSpawnPoints = new List<Transform>();
		GetPlayerSpawnPoints();
		GetSceneLoader();

		if(levelManager != null)
		{
			isPauseEnabledInScene = levelManager.isPauseEnabledInScene;
			musicTrack = levelManager.musicTrack;
		}
	}

	void OnSceneGUI(SceneView sceneView)
	{
		if(Application.isPlaying || isRexGUILocked)
		{
			return;
		}

		if(SceneManager.GetActiveScene() != previousScene)
		{
			OnSceneChanged();
		}

		previousScene = SceneManager.GetActiveScene();

		didSelectItemThisFrame = false;
		//int bufferedHandleID = currentHandleID;

		DrawSceneBoundaries();

		if((playerSpawnPoints.Count >= 1 && playerSpawnPoints[0] == null) || playerSpawnPoints.Count < 1)
		{
			GetPlayerSpawnPoints();
		}

		if(playerSpawnPoints != null && playerSpawnPoints.Count >= 1)
		{
			if(playerSpawnPoints[0] != null)
			{
				tempHandleID = 0;
				playerSpawnPoints[0].transform.position = Handles.FreeMoveHandle(playerSpawnPoints[0].transform.position, Quaternion.identity, HandleUtility.GetHandleSize(playerSpawnPoints[0].transform.position) * handleSizeMultiplier, Vector3.zero, DrawGizmoHandleSpawnPoint);  

				if(tempHandleID != 0 && GUIUtility.hotControl == tempHandleID && !didSelectItemThisFrame)
				{
					Repaint();
					didSelectItemThisFrame = true;
					Selection.activeGameObject = playerSpawnPoints[0].gameObject;
					lastSelectedGameObject = Selection.activeGameObject;
				}

				if(playerSpawnPointPosition != playerSpawnPoints[0].transform.position)
				{
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					Undo.RegisterCompleteObjectUndo(playerSpawnPoints[0], "Move PlayerSpawnPoint");
				}

				playerSpawnPointPosition = playerSpawnPoints[0].transform.position;
			}
		}

		if(sceneLoaders == null || sceneLoaders.Count <= 0)
		{
			GetSceneLoader();
		}

		for(int i = sceneLoaders.Count - 1; i >= 0; i --)
		{
			tempHandleID = 0;
			if(sceneLoaders[i] == null)
			{
				sceneLoaders.RemoveAt(i);
			}
			else
			{
				SceneLoader loader = sceneLoaders[i];
				if(!loader.isAttachedToGameObject)
				{
					SceneBoundary.Edge side = loader.edge;
					loader.gameObject.hideFlags = HideFlags.HideInInspector;
					loader.SetWillDrawGizmos(isRexGUILocked);
					currentSceneLoaderSide = side;

					Vector3 originalLoaderPosition = loader.transform.position;

					currentLoader = loader;
					loader.transform.position = Handles.FreeMoveHandle(loader.transform.position, Quaternion.identity, HandleUtility.GetHandleSize(loader.transform.position) * handleSizeMultiplier, Vector3.zero, DrawGizmoHandleSceneLoader); 
					if(GUIUtility.hotControl != 0 && GUIUtility.hotControl == tempHandleID && !didSelectItemThisFrame)
					{
						didSelectItemThisFrame = true;
						Repaint();
						Selection.activeGameObject = loader.gameObject;
						lastSelectedGameObject = Selection.activeGameObject;
					}

					loader.DisplaySceneLoaderText(loader, loader.transform.position);

					if(loader.transform.position != originalLoaderPosition)
					{
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
						Undo.RegisterCompleteObjectUndo(loader, "Move SceneLoader");
					}
				}
			}
		}

		bool isItemSelected = false;
		for(int j = 0; j < sceneLoaders.Count; j ++)
		{
			if(Selection.activeGameObject == sceneLoaders[j].gameObject)
			{
				isItemSelected = true;
			}
		}

		if(playerSpawnPoints.Count > 0 && playerSpawnPoints[0] != null && Selection.activeGameObject == playerSpawnPoints[0].gameObject)
		{
			isItemSelected = true;
		}

		if(sceneBoundaries == null || (sceneBoundaries.top == null || sceneBoundaries.bottom == null || sceneBoundaries.left == null || sceneBoundaries.right == null))
		{
			GetSceneBoundaries();
		}

		if(sceneBoundaries != null && (Selection.activeGameObject == sceneBoundaries.top.gameObject || Selection.activeGameObject == sceneBoundaries.bottom.gameObject || Selection.activeGameObject == sceneBoundaries.left.gameObject || Selection.activeGameObject == sceneBoundaries.right.gameObject))
		{
			isItemSelected = true;
		}

		if(!isItemSelected && wasItemSelectedLastFrame)
		{
			Tools.current = previousTool;
		}
		else if(isItemSelected)
		{
			if(Tools.current != Tool.None)
			{
				previousTool = Tools.current;
			}

			Tools.current = Tool.None;

			Selection.activeObject.hideFlags = HideFlags.None;
		}

		wasItemSelectedLastFrame = isItemSelected;
	}

	protected void DrawSceneBoundaries()
	{
		if(sceneBoundaries == null || (sceneBoundaries.top == null || sceneBoundaries.bottom == null || sceneBoundaries.left == null || sceneBoundaries.right == null))
		{
			GetSceneBoundaries();
		}

		if(sceneBoundaries == null)
		{
			return;
		}

		DrawLineForBoundary(sceneBoundaries.left);
		DrawLineForBoundary(sceneBoundaries.right);
		DrawLineForBoundary(sceneBoundaries.bottom);
		DrawLineForBoundary(sceneBoundaries.top);
	}

	protected void DrawLineForBoundary(SceneBoundary boundary)
	{
		if(boundary == null)
		{
			return;
		}

		tempHandleID = 0;

		Vector2 startPoint = boundary.transform.position;
		Vector3 handlePosition;
		float thickness = 0.03f * HandleUtility.GetHandleSize(startPoint);
		float height = 14.5f;
		//float width = 26.0f;
		float handleDragSizeMultiplier = 0.125f;
		BoxCollider2D boxCollider = boundary.GetComponent<BoxCollider2D>();
		float previousBoundaryPosition = (boundary == sceneBoundaries.left || boundary == sceneBoundaries.right) ? boundary.transform.position.y : boundary.transform.position.x;

		height = boundary.GetBoundarySize() + thickness;
		handlePosition = (boundary == sceneBoundaries.left || boundary == sceneBoundaries.right) ? new Vector3(boundary.transform.position.x, boxCollider.bounds.min.y + boxCollider.size.y / 2, 0.0f) : new Vector3(boxCollider.bounds.min.x + boxCollider.size.x / 2, boundary.transform.position.y, 0.0f);//sceneBoundaries.left.transform.position;//new Vector3(startPoint.x + thickness / 2, startPoint.y + height / 2.0f, 0.0f);
		startPoint = (boundary == sceneBoundaries.left || boundary == sceneBoundaries.right) ? new Vector2(handlePosition.x - thickness / 2, boxCollider.bounds.min.y + boundary.GetMarginSize() - thickness / 2) : new Vector2(boxCollider.bounds.min.x + boundary.GetMarginSize() - thickness / 2, handlePosition.y - thickness / 2.0f);

		currentLineDirection = (boundary == sceneBoundaries.left || boundary == sceneBoundaries.right) ? LineDirection.Vertical : LineDirection.Horizontal;
		Color lineColor = Color.white;

		if(boundary.isBottomlessPit)
		{
			lineColor = new Color(1.0f, 0.5f, 0.5f, 1.0f);
		}

		if(!boundary.isSolid)
		{
			lineColor.a = 0.35f;
		}

		DrawLine(currentLineDirection, startPoint, height, thickness, lineColor);

		currentSceneBoundary = boundary;

		Vector3 previousBoundary = boundary.transform.position;
		bool wasPit = boundary.isBottomlessPit;
		bool wasSolid = boundary.isSolid;

		boundary.transform.position = Handles.FreeMoveHandle(handlePosition, Quaternion.identity, HandleUtility.GetHandleSize(handlePosition) * handleDragSizeMultiplier, Vector3.zero, DrawGizmoHandleBoundary);
		boundary.transform.position = (boundary == sceneBoundaries.left || boundary == sceneBoundaries.right) ? new Vector3(boundary.transform.position.x, previousBoundaryPosition, 0.0f) : new Vector3(previousBoundaryPosition, boundary.transform.position.y, 0.0f);

		handlePosition.x += 0.5f * HandleUtility.GetHandleSize(handlePosition);
		handlePosition.y += 0.5f * HandleUtility.GetHandleSize(handlePosition);

		if(GUIUtility.hotControl != 0 && GUIUtility.hotControl == tempHandleID && !didSelectItemThisFrame)
		{
			didSelectItemThisFrame = true;
			Repaint();
			Selection.activeGameObject = boundary.gameObject;
			lastSelectedGameObject = Selection.activeGameObject;
		}

		float scaleMultiplier = (Selection.activeGameObject == boundary.gameObject) ? 0.1f : 0.0f;
		Vector3 adjustedHandlePosition = (Selection.activeGameObject == boundary.gameObject) ? handlePosition : Camera.current.ViewportToScreenPoint(new Vector3(10000.0f, 10000.0f, 0.0f));

		GUIStyle style = new GUIStyle();
		style.contentOffset = new Vector2(16.0f, -8.625f);
		style.fontSize = (Selection.activeGameObject == boundary.gameObject) ? 10 : 0;
		style.normal.textColor = (Selection.activeGameObject == boundary.gameObject) ? Color.white : new Color(1.0f, 1.0f, 1.0f, 0.0f);

		Handles.Label(adjustedHandlePosition, "Solid", style);
		drawSelectedButton = boundary.isSolid;
		if(Handles.Button(adjustedHandlePosition, Quaternion.identity, HandleUtility.GetHandleSize(adjustedHandlePosition) * scaleMultiplier, HandleUtility.GetHandleSize(adjustedHandlePosition) * scaleMultiplier, DrawGizmoHandleToggle))
		{
			boundary.isSolid = !boundary.isSolid;
		}

		adjustedHandlePosition.y -= 0.25f * HandleUtility.GetHandleSize(handlePosition);

		Handles.Label(adjustedHandlePosition, "Pit", style);
		drawSelectedButton = boundary.isBottomlessPit;
		if(Handles.Button(adjustedHandlePosition, Quaternion.identity, HandleUtility.GetHandleSize(adjustedHandlePosition) * scaleMultiplier, HandleUtility.GetHandleSize(adjustedHandlePosition) * scaleMultiplier, DrawGizmoHandleToggle))
		{
			boundary.isBottomlessPit = !boundary.isBottomlessPit;
		}

		if(boundary.transform.position != previousBoundary || boundary.isSolid != wasSolid || boundary.isBottomlessPit != wasPit)
		{
			EditorUtility.SetDirty(boundary);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			Undo.RegisterCompleteObjectUndo(boundary, "Edit SceneBoundary");
		}
	}

	public void DrawLine(LineDirection lineDirection, Vector2 startPoint, float distance, float thickness, Color color)
	{
		Vector2 endPoint = (lineDirection == LineDirection.Horizontal) ? new Vector2(startPoint.x + distance, startPoint.y) : new Vector2(startPoint.x, startPoint.y + distance);
		Rect rect = (lineDirection == LineDirection.Horizontal) ? new Rect(startPoint.x, startPoint.y, endPoint.x - startPoint.x, thickness) : new Rect(startPoint.x, startPoint.y, thickness, endPoint.y - startPoint.y);

		DrawQuad(rect, color);
	}

	public void DrawQuad(Rect rect, Color color)
	{
		if(lineMaterial == null)
		{
			CreateLineMaterial();
		}

		GL.PushMatrix();
		lineMaterial.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS);

		Vector2 lowerLeft = Camera.current.WorldToViewportPoint(new Vector2(rect.min.x, rect.min.y));
		Vector2 upperLeft = Camera.current.WorldToViewportPoint(new Vector2(rect.min.x, rect.max.y));
		Vector2 upperRight = Camera.current.WorldToViewportPoint(new Vector2(rect.max.x, rect.max.y));
		Vector2 lowerRight = Camera.current.WorldToViewportPoint(new Vector2(rect.max.x, rect.min.y));

		GL.Color(color);
		GL.Vertex3(lowerLeft.x, lowerLeft.y, 0.0f); //Lower-left
		GL.Vertex3(upperLeft.x, upperLeft.y, 0); //Upper-left
		GL.Vertex3(upperRight.x, upperRight.y, 0); //Upper-right
		GL.Vertex3(lowerRight.x, lowerRight.y, 0); //Lower-right
		GL.End();
		GL.PopMatrix();
	}

	protected void DrawGizmoHandleSceneLoader(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
	{
		if(eventType == EventType.Layout)
		{
			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			HandleUtility.AddControl(controlId, HandleUtility.DistanceToCircle(screenPosition, size));
		}

		string path = "Assets/RexEngine/Gizmos/SceneLoader_Icon_Side.png";

		if(gizmoMaterial == null)
		{
			gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
		}

		switch(currentSceneLoaderSide)
		{
			case SceneBoundary.Edge.Top:
				path = "Assets/RexEngine/Gizmos/SceneLoader_Icon_Top.png";
				break;
			case SceneBoundary.Edge.Bottom:
				path = "Assets/RexEngine/Gizmos/SceneLoader_Icon_Bottom.png";
				break;
			case SceneBoundary.Edge.Left:
			case SceneBoundary.Edge.Right:
				path = "Assets/RexEngine/Gizmos/SceneLoader_Icon_Side.png";
				break;
		}

		Texture2D texture  = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		Vector3 cameraSide = Camera.current.transform.right * size;
		Vector3 cameraTop = Camera.current.transform.up * size;

		if(controlId == GUIUtility.hotControl)
		{
			tempHandleID = controlId;
			currentHandleID = controlId;
		}

		gizmoMaterial.mainTexture = texture;
		gizmoMaterial.SetPass(0);

		GL.Begin(GL.QUADS);

		if(Selection.activeGameObject == currentLoader.gameObject)
		{
			GL.Color(new Color(0.965f, 0.949f, 0.196f, 0.890f));
		}
		else
		{
			GL.Color(Color.white);
		}

		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex(position + cameraSide + cameraTop);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex(position + cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex(position - cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex(position - cameraSide + cameraTop);

		GL.End();
	} 

	protected void DrawGizmoHandleSpawnPoint(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
	{
		if(eventType == EventType.Layout)
		{
			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			HandleUtility.AddControl(controlId, HandleUtility.DistanceToCircle(screenPosition, size));
		}

		Texture2D texture  = AssetDatabase.LoadAssetAtPath("Assets/RexEngine/Gizmos/PlayerSpawnPoint.png", typeof(Texture2D)) as Texture2D;
		Vector3 cameraSide = Camera.current.transform.right * size;
		Vector3 cameraTop = Camera.current.transform.up * size;

		if(gizmoMaterial == null)
		{
			gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
		}

		gizmoMaterial.mainTexture = texture;
		gizmoMaterial.SetPass(0);

		GL.Begin(GL.QUADS);
		GL.Color(Handles.color);

		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex(position + cameraSide + cameraTop);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex(position + cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex(position - cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex(position - cameraSide + cameraTop);

		GL.End();

		if(controlId == GUIUtility.hotControl)
		{
			tempHandleID = controlId;
			currentHandleID = controlId;
		}
	} 

	protected void DrawGizmoHandleToggle(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
	{
		if(eventType == EventType.Layout)
		{
			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			HandleUtility.AddControl(controlId, HandleUtility.DistanceToCircle(screenPosition, size));
		}

		string path = (drawSelectedButton) ? "Assets/RexEngine/Gizmos/ToggleButton_Selected.png" : "Assets/RexEngine/Gizmos/ToggleButton_Unselected.png";
		Texture2D texture  = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		Vector3 cameraSide = Camera.current.transform.right * size;
		Vector3 cameraTop = Camera.current.transform.up * size;

		if(gizmoMaterial == null)
		{
			gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
		}

		gizmoMaterial.mainTexture = texture;
		gizmoMaterial.SetPass(0);

		GL.Begin(GL.QUADS);
		GL.Color(Handles.color);

		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex(position + cameraSide + cameraTop);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex(position + cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex(position - cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex(position - cameraSide + cameraTop);

		GL.End();
	} 

	protected void DrawGizmoHandleBoundary(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
	{
		if(eventType == EventType.Layout)
		{
			Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
			HandleUtility.AddControl(controlId, HandleUtility.DistanceToCircle(screenPosition, size));
		}

		string assetPath = (currentLineDirection == LineDirection.Horizontal) ? "Assets/RexEngine/Gizmos/DragTabHorizontal.png" : "Assets/RexEngine/Gizmos/DragTabVertical.png";
		Texture2D texture  = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
		Vector3 cameraSide = Camera.current.transform.right * size;
		Vector3 cameraTop = Camera.current.transform.up * size;

		if(gizmoMaterial == null)
		{
			gizmoMaterial = new Material(Shader.Find("Sprites/Default"));
		}

		gizmoMaterial.mainTexture = texture;
		gizmoMaterial.SetPass(0);

		GL.Begin(GL.QUADS);

		if(Selection.activeGameObject == currentSceneBoundary.gameObject)
		{
			GL.Color(new Color(0.965f, 0.949f, 0.196f, 0.890f));
		}
		else
		{
			GL.Color(Color.white);
		}

		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex(position + cameraSide + cameraTop);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex(position + cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex(position - cameraSide - cameraTop);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex(position - cameraSide + cameraTop);

		GL.End();

		if(controlId == GUIUtility.hotControl)
		{
			tempHandleID = controlId;
			currentHandleID = controlId;
		}
	}

	protected void CreateLineMaterial()
	{
		Shader shader = Shader.Find("Hidden/Internal-Colored");
		lineMaterial = new Material(shader);
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		lineMaterial.SetInt("_ZWrite", 0);
	}

	void OnGUI()
	{
		if(Application.isPlaying)
		{
			return;
		}

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label(new GUIContent("Level Editor", "Options for the current Rex Engine scene."), EditorStyles.boldLabel);
		}
		EditorGUILayout.EndHorizontal();

		if(levelManager == null)
		{
			levelManager = GameObject.FindObjectOfType<LevelManager>();
			if(levelManager == null)
			{
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button(new GUIContent("Setup Level Scene", "Pressing this automatically sets up the current scene to work with Rex Engine.")))
				{
					RexMenu.SetupLevelScene();
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button(new GUIContent("Setup Title Scene", "Pressing this automatically sets up the current scene to act as a title screen for your Rex Engine project.")))
				{
					RexMenu.SetupTitleScene();
					levelManager = GameObject.FindObjectOfType<LevelManager>();
					levelManager.isPauseEnabledInScene = false;
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		if(levelManager != null)
		{
			EditorGUILayout.BeginHorizontal();
			{
				bool wasRexGUILocked = isRexGUILocked;
				isRexGUILocked = EditorGUILayout.Toggle(new GUIContent("Lock Rex GUI", "Locks Scene Boundaries, Scene Loaders, and Player Spawn Points from being moved or edited."), isRexGUILocked);

				if(isRexGUILocked != wasRexGUILocked)
				{
					for(int i = 0; i < sceneLoaders.Count; i ++)
					{
						sceneLoaders[i].SetWillDrawGizmos(isRexGUILocked);
					}

					SceneView.RepaintAll();
					EditorPrefs.SetBool("com.skyty.rexengine.IsRexGUILocked", isRexGUILocked);
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			showFieldScene = EditorGUILayout.Foldout(showFieldScene, new GUIContent("Level Settings", "Options for the music and pause settings in the current scene."));
			if(showFieldScene)
			{
				EditorGUI.indentLevel ++;
				EditorGUILayout.BeginHorizontal();
				{
					musicTrack = EditorGUILayout.ObjectField(new GUIContent("Music Track", "Allows you to slot the music track that plays in this scene."), musicTrack, typeof(AudioClip), true) as AudioClip;		
				}		
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					isPauseEnabledInScene = EditorGUILayout.Toggle(new GUIContent("Is Pause Enabled in Scene", "Whether or not the Pause button will pause the game in the current scene."), isPauseEnabledInScene);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button(new GUIContent("Save", "Saves the current settings.")))
				{
					if(levelManager != null)
					{
						levelManager.isPauseEnabledInScene = isPauseEnabledInScene;
						levelManager.musicTrack = musicTrack;

						EditorUtility.SetDirty(levelManager);
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

						Debug.Log("Rex-cellent! Your level settings were saved!");
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

				EditorGUI.indentLevel --;
			}

			showFieldCreate = EditorGUILayout.Foldout(showFieldCreate, new GUIContent("Create", "Tools for creating Scene Loaders and Player Spawn Points in the scene."));
			if(showFieldCreate)
			{
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Create Scene Loader (Left)"))
				{
					CreateSceneLoader(SceneBoundary.Edge.Left);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Create Scene Loader (Right)"))
				{
					CreateSceneLoader(SceneBoundary.Edge.Right);

				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Create Scene Loader (Top)"))
				{
					CreateSceneLoader(SceneBoundary.Edge.Top);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Create Scene Loader (Bottom)"))
				{
					CreateSceneLoader(SceneBoundary.Edge.Bottom);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button(new GUIContent("Player Spawn Point", "Creates a Player Spawn Point. This is used only when gameplay is first started; if the player moved between scenes via a Scene Loader, they will instead enter the scene at the point where that Scene Loader specifies.")))
				{
					GameObject existingSpawnPoint = GameObject.Find("PlayerSpawnPoint");
					if(existingSpawnPoint == null)
					{
						CreateAtPath("System/PlayerSpawnPoint");
					}
					else
					{
						EditorUtility.DisplayDialog("Oh noes!", "There's already a Player Spawn Point in this scene.", "No worries!");
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.EndScrollView();
	}

	protected void UnlockRexGUI()
	{
		isRexGUILocked = false;
		for(int i = 0; i < sceneLoaders.Count; i ++)
		{
			sceneLoaders[i].SetWillDrawGizmos(isRexGUILocked);
		}

		SceneView.RepaintAll();
	}

	protected SceneLoader CreateSceneLoader(SceneBoundary.Edge _edge)
	{
		string path = "System/SceneLoader_right";
		switch(_edge)
		{
			case SceneBoundary.Edge.Left:
				path = "System/SceneLoader_left";
				break;
			case SceneBoundary.Edge.Right:
				path = "System/SceneLoader_right";
				break;
			case SceneBoundary.Edge.Top:
				path = "System/SceneLoader_top";
				break;
			case SceneBoundary.Edge.Bottom:
				path = "System/SceneLoader_bottom";
				break;
		}

		GameObject gameObject = CreateAtPath(path);
		SceneLoader newLoader = gameObject.GetComponent<SceneLoader>();
		newLoader.identifier = GetFirstUnusedID();
		sceneLoaders.Add(newLoader);

		UnlockRexGUI();
		currentHandleID = 0;
		SceneView.RepaintAll();

		return newLoader;
	}

	protected GameObject CreateAtPath(string prefabPath)
	{
		Object prefab = Resources.Load(prefabPath) as Object;
		GameObject _gameObject = Instantiate(prefab, SceneView.lastActiveSceneView.pivot, Quaternion.identity) as GameObject;
		ParentToGizmos(_gameObject);
		Selection.activeObject = _gameObject;
		Undo.RegisterCreatedObjectUndo(_gameObject, "Modify_" + prefabPath);
		_gameObject.name = _gameObject.name.Split('(')[0];

		return _gameObject;
	}

	protected GameObject gizmos;

	protected void ParentToGizmos(GameObject _gameObject)
	{
		if(gizmos == null)
		{
			gizmos = GameObject.Find("Gizmos");
		}

		if(gizmos != null)
		{
			_gameObject.transform.parent = gizmos.transform;
		}
	}

	protected string GetFirstUnusedID()
	{
		string newID = "A";

		List<string> existingIDs = new List<string>();
		for(int i = 0; i < sceneLoaders.Count; i ++)
		{
			existingIDs.Add(sceneLoaders[i].identifier);
		}

		List<string> availableIDs = new List<string>();
		for(int i = 0; i < 26; i ++)
		{
			availableIDs.Add(((char)((int) 'A' + i)).ToString());
		}

		for(int i = existingIDs.Count - 1; i >= 0; i --)
		{
			if(availableIDs.Contains(existingIDs[i]))
			{
				availableIDs.Remove(existingIDs[i]);
			}
		}

		newID = (availableIDs.Count > 0) ? availableIDs[0] : "?";

		return newID;
	}

	protected void GetPlayerSpawnPoints()
	{
		playerSpawnPoints.Clear();

		if(GameObject.Find("PlayerSpawnPoint") != null)
		{
			Transform playerSpawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
			playerSpawnPoints.Add(playerSpawnPoint);
			playerSpawnPointPosition = playerSpawnPoints[0].transform.position;
		}
	}

	protected void GetSceneBoundaries()
	{
		if(sceneBoundaries == null)
		{
			sceneBoundaries = new SceneBoundary.SceneBoundaries();
		}

		foreach(SceneBoundary sceneBoundary in GameObject.FindObjectsOfType<SceneBoundary>())
		{
			switch(sceneBoundary.edge)
			{
				case SceneBoundary.Edge.Top:
					sceneBoundaries.top = sceneBoundary;
					break;
				case SceneBoundary.Edge.Bottom:
					sceneBoundaries.bottom = sceneBoundary;
					break;
				case SceneBoundary.Edge.Left:
					sceneBoundaries.left = sceneBoundary;
					break;
				case SceneBoundary.Edge.Right:
					sceneBoundaries.right = sceneBoundary;
					break;
			}
		}

		if(sceneBoundaries.top == null || sceneBoundaries.bottom == null || sceneBoundaries.left == null || sceneBoundaries.right == null)
		{
			sceneBoundaries = null;
		}
	}

	protected void GetSceneLoader()
	{
		if(sceneLoaders == null)
		{
			sceneLoaders = new List<SceneLoader>();
		}

		for(int i = sceneLoaders.Count - 1; i >= 0; i --)
		{
			if(sceneLoaders[i] == null)
			{
				sceneLoaders.RemoveAt(i);
			}
		}

		foreach(SceneLoader _loader in GameObject.FindObjectsOfType<SceneLoader>())
		{
			if(!sceneLoaders.Contains(_loader))
			{
				sceneLoaders.Add(_loader);
			}
		}
	}

	void OnDestroy()
	{
		for(int i = 0; i < sceneLoaders.Count; i ++)
		{
			sceneLoaders[i].SetWillDrawGizmos(true);
		}
	}
}
#endif
