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
#endif

using RexEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public class RexPalette:EditorWindow
{
	protected Vector2 scrollPosition;
	protected GameObject actors;
	protected bool showFieldTemplate = false;
	protected bool showFieldGimmicks = false;
	protected bool showFieldPowerups = false;
	protected bool showFieldNewEnemyFromScript = false;
	protected bool showFieldNewPlayerFromScript = false;
	protected string scriptNameString_Enemy;
	protected string scriptNameString_Player;

	static RexPalette()
	{
		//Debug.Log("RexPalette :: Awake");
	}

	public float snapNumber;

	void OnEnable()
	{
		snapNumber = EditorPrefs.GetFloat("com.skyty.rexengine.SnapNumber");
	}

	void OnGUI()
	{
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label(new GUIContent("Create", "Tools to automatically create new objects in the current scene."), EditorStyles.boldLabel);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		float previousSnapNumber = snapNumber;
		EditorGUILayout.BeginHorizontal();
		{
			snapNumber = EditorGUILayout.FloatField("Snap to Nearest: ", snapNumber);
		}
		EditorGUILayout.EndHorizontal();
		if(previousSnapNumber != snapNumber)
		{
			EditorPrefs.SetFloat("com.skyty.rexengine.SnapNumber", snapNumber);
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		showFieldTemplate = EditorGUILayout.Foldout(showFieldTemplate, new GUIContent("From Template", "Tools to create new objects from a template; these should then be modified as desired and saved in the Library."));
		if(showFieldTemplate)
		{
			EditorGUI.indentLevel ++;

			if(GUILayout.Button("Enemy"))
			{
				GameObject _gameObject = CreateAtPath("Templates/EnemyTemplate");
				PrefabUtility.DisconnectPrefabInstance(_gameObject);
				_gameObject.name = "Enemy";
			}

			if(GUILayout.Button("Player"))
			{
				GameObject _gameObject = CreateAtPath("Templates/PlayerTemplate");
				PrefabUtility.DisconnectPrefabInstance(_gameObject);
				_gameObject.name = "Player";
			}

			if(GUILayout.Button("NPC"))
			{
				GameObject _gameObject = CreateAtPath("Templates/NPCTemplate");
				PrefabUtility.DisconnectPrefabInstance(_gameObject);
				_gameObject.name = "NPC";
			}

			showFieldNewEnemyFromScript = EditorGUILayout.Foldout(showFieldNewEnemyFromScript, new GUIContent("New Enemy From Script", "Create a new enemy using a specific script extending RexActor."));
			if(showFieldNewEnemyFromScript)
			{
				showFieldNewPlayerFromScript = false;
				scriptNameString_Enemy = EditorGUILayout.TextField(new GUIContent("New Script Name", "The name of the script being used to create this new enemy. The new script must extend RexActor. If the new script is part of a specific namespace, the name of that namespace must be included here in dot syntax format (i.e. RexEngine.RexActor)"), scriptNameString_Enemy);
				if(scriptNameString_Enemy != "")
				{
					if(GUILayout.Button("New Enemy From Script"))
					{
						System.Type scriptType = System.Type.GetType(scriptNameString_Enemy);
						if(scriptType != null)
						{
							if(scriptType.IsSubclassOf(typeof(RexActor)))
							{
								GameObject _gameObject = CreateAtPath("Templates/EnemyTemplate");
								PrefabUtility.DisconnectPrefabInstance(_gameObject);
								_gameObject.name = "Enemy";

								RexActor rexActor = _gameObject.GetComponent<RexActor>();
								RexMenu.SwapRexActorScript(rexActor, scriptNameString_Enemy, false);
							}
							else
							{
								EditorUtility.DisplayDialog("Oh no!", "The script you are trying to instantiate must extend RexActor.", "Gotcha");
							}
						}
						else
						{
							EditorUtility.DisplayDialog("Oh no!", "The script you are trying to instantiate couldn't be found. If the script is part of a specific namespace, make sure to include that namespace in the Script Name field using dot syntax format.", "Gotcha");
						}
					}
				}
			}

			showFieldNewPlayerFromScript = EditorGUILayout.Foldout(showFieldNewPlayerFromScript, new GUIContent("New Player From Script", "Create a new player using a specific script extending RexActor."));
			if(showFieldNewPlayerFromScript)
			{
				showFieldNewEnemyFromScript = false;
				scriptNameString_Player = EditorGUILayout.TextField(new GUIContent("New Script Name", "The name of the script being used to create this new player. The new script must extend RexActor. If the new script is part of a specific namespace, the name of that namespace must be included here in dot syntax format (i.e. RexEngine.RexActor)"), scriptNameString_Player);
				if(scriptNameString_Player != "")
				{
					if(GUILayout.Button("New Player From Script"))
					{
						System.Type scriptType = System.Type.GetType(scriptNameString_Player);
						if(scriptType != null)
						{
							if(scriptType.IsSubclassOf(typeof(RexActor)))
							{
								GameObject _gameObject = CreateAtPath("Templates/PlayerTemplate");
								PrefabUtility.DisconnectPrefabInstance(_gameObject);
								_gameObject.name = "Player";

								RexActor rexActor = _gameObject.GetComponent<RexActor>();
								RexMenu.SwapRexActorScript(rexActor, scriptNameString_Player, false);
							}
							else
							{
								EditorUtility.DisplayDialog("Oh no!", "The script you are trying to instantiate must extend RexActor.", "Gotcha");
							}
						}
						else
						{
							EditorUtility.DisplayDialog("Oh no!", "The script you are trying to instantiate couldn't be found. If the script is part of a specific namespace, make sure to include that namespace in the Script Name field using dot syntax format.", "Gotcha");
						}
					}
				}
			}

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel --;
		}

		showFieldGimmicks = EditorGUILayout.Foldout(showFieldGimmicks, new GUIContent("Gimmick", "Tools to add new environmental gimmicks and hazards in the current scene."));
		if(showFieldGimmicks)
		{
			EditorGUI.indentLevel ++;

			if(GUILayout.Button("Block"))
			{
				CreateAtPath("Gimmicks/Block");
			}

			if(GUILayout.Button("Block (Breakable Bottom)"))
			{
				CreateAtPath("Gimmicks/BreakBottom_Block");
			}

			if(GUILayout.Button("Block (Item)"))
			{
				CreateAtPath("Gimmicks/ItemBottom_Block");
			}

			if(GUILayout.Button("Breakable Tile"))
			{
				CreateAtPath("Gimmicks/BreakableTile");
			}

			if(GUILayout.Button("Breaking Platform"))
			{
				CreateAtPath("Gimmicks/BreakingPlatform");
			}

			if(GUILayout.Button("Checkpoint"))
			{
				CreateAtPath("Gimmicks/Checkpoint");
			}

			if(GUILayout.Button("Conveyor Belt"))
			{
				CreateAtPath("Gimmicks/ConveyorBelt");
			}

			if(GUILayout.Button("Dialogue Popup"))
			{
				CreateAtPath("Gimmicks/DialoguePopup");
			}

			if(GUILayout.Button("Door"))
			{
				CreateAtPath("Gimmicks/Door");
			}

			if(GUILayout.Button("Enemy Spawner"))
			{
				CreateAtPath("Gimmicks/EnemySpawner");
			}

			if(GUILayout.Button("Falling Platform"))
			{
				CreateAtPath("Gimmicks/FallingPlatform");
			}

			if(GUILayout.Button("Gravity Changer (Down)"))
			{
				CreateAtPath("Gimmicks/GravityChanger_Normal");
			}

			if(GUILayout.Button("Gravity Changer (Up)"))
			{
				CreateAtPath("Gimmicks/GravityChanger_Reverse");
			}

			if(GUILayout.Button("Ice"))
			{
				CreateAtPath("Gimmicks/Ice");
			}

			if(GUILayout.Button("Ladder"))
			{
				CreateAtPath("Gimmicks/Ladder");
			}

			if(GUILayout.Button("Moving Platform"))
			{
				CreateAtPath("Gimmicks/MovingPlatform");
			}

			if(GUILayout.Button("One-Way Platform"))
			{
				CreateAtPath("Gimmicks/OneWayPlatform/OneWayPlatform_Middle");
			}

			if(GUILayout.Button("Stairs"))
			{
				CreateAtPath("Gimmicks/Stairs");
			}

			if(GUILayout.Button("Swap Player"))
			{
				CreateAtPath("Gimmicks/SwapPlayer");
			}

			if(GUILayout.Button("TimeShift"))
			{
				CreateAtPath("Gimmicks/TimeShift");
			}

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel --;
		}

		showFieldPowerups = EditorGUILayout.Foldout(showFieldPowerups, new GUIContent("Powerups", "Tools to add powerups to the current scene."));
		if(showFieldPowerups)
		{
			EditorGUI.indentLevel ++;

			if(GUILayout.Button("Bomb"))
			{
				CreateAtPath("Powerups/DestroyEnemiesPowerup");
			}

			if(GUILayout.Button("Extra Life"))
			{
				CreateAtPath("Powerups/ExtraLife");
			}

			if(GUILayout.Button("Extra Points"))
			{
				CreateAtPath("Powerups/Bolt");
			}

			if(GUILayout.Button("HP Restore (Large)"))
			{
				CreateAtPath("Powerups/HPRestoreLarge");
			}

			if(GUILayout.Button("HP Restore (Small)"))
			{
				CreateAtPath("Powerups/HPRestoreSmall");
			}

			if(GUILayout.Button("Invincibility"))
			{
				CreateAtPath("Powerups/InvincibilityPowerup");
			}

			if(GUILayout.Button("MP Restore (Large)"))
			{
				CreateAtPath("Powerups/WeaponEnergyRestoreLarge");
			}

			if(GUILayout.Button("MP Restore (Small)"))
			{
				CreateAtPath("Powerups/WeaponEnergyRestoreSmall");
			}

			if(GUILayout.Button("Shield"))
			{
				CreateAtPath("Powerups/ShieldPowerup");
			}

			if(GUILayout.Button("Stopwatch"))
			{
				CreateAtPath("Powerups/StopwatchPowerup");
			}

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel --;
		}

		EditorGUILayout.EndScrollView();
	}

	protected GameObject CreateAtPath(string prefabPath)
	{
		Object prefab = Resources.Load(prefabPath) as Object;
		Vector3 position = (SceneView.lastActiveSceneView) ? SceneView.lastActiveSceneView.pivot : Vector3.zero;

		if(snapNumber != 0.0f)
		{
			position.x = (position.x - (position.x % snapNumber));
			position.y = (position.y - (position.y % snapNumber));
		}

		GameObject _gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		_gameObject.transform.position = position;
		ParentToActors(_gameObject);
		Selection.activeObject = _gameObject;
		Undo.RegisterCreatedObjectUndo(_gameObject, "Modify_" + prefabPath);
		_gameObject.name = _gameObject.name.Split('(')[0];

		return _gameObject;
	}

	protected void ParentToActors(GameObject _gameObject)
	{
		if(actors == null)
		{
			actors = GameObject.Find("Actors");
		}

		if(actors != null)
		{
			_gameObject.transform.parent = actors.transform;
		}
	}
}
#endif
