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
public class RexWelcome:EditorWindow
{
	protected Vector2 scrollPosition;
	protected static bool hasInitialized = false;
	protected static bool willDisplayOnStartup;

	static RexWelcome()
	{
		if(!hasInitialized)
		{
			//Debug.Log("RexWelcome :: Awake");
			EditorApplication.update += Startup;
		}
	}

	public static void Startup()
	{
		bool willHide = EditorPrefs.GetBool("HideWelcomeWindow");
		willDisplayOnStartup = !EditorPrefs.GetBool("HideWelcomeWindow");
		//Debug.Log("RexWelcome :: Will display on startup: " + willDisplayOnStartup + "    Editor prefs will hide: " + EditorPrefs.GetBool("HideWelcomeWindow"));

		if(!willHide && !Application.isPlaying)
		{
			//Debug.Log("RexWelcome :: Start");

			UnityEditor.EditorWindow window = GetWindow(typeof(RexWelcome));
			window.Show();

		}
		else
		{
			UnityEditor.EditorWindow window = GetWindow(typeof(RexWelcome));
			window.Close();
		}

		EditorApplication.update -= Startup;
		hasInitialized = true;
	}

	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	void OnSceneGUI(SceneView sceneView)
	{
		if(Application.isPlaying)
		{
			return;
		}
	}

	void OnGUI()
	{
		if(Application.isPlaying || !hasInitialized)
		{
			return;
		}

		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label("Welcome to Rex Engine!", EditorStyles.boldLabel);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label("Here are some goodies to help you get started:");
		}
		EditorGUILayout.EndHorizontal();

		if(GUILayout.Button("View QuickStart Guide"))
		{
			Application.OpenURL("http://www.skytyrannosaur.com/rexenginequickstart/");
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		if(GUILayout.Button("Open Rex Settings Window"))
		{
			ShowRexSettings();
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		if(GUILayout.Button("Open Rex Level Editor Window"))
		{
			ShowRexLevelEditor();
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		if(GUILayout.Button("Open Rex Palette Window"))
		{
			ShowRexPalette();
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label("Need help? Feel free to reach out to us at:");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label("info@skytyrannosaur.com");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		GUILayout.Label("Like Rex Engine? Show it some love:");
		if(GUILayout.Button("Rate Rex Engine"))
		{
			Application.OpenURL("http://u3d.as/RLre");
		}

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			willDisplayOnStartup = GUILayout.Toggle(willDisplayOnStartup, " Display this window on startup");

			if(!willDisplayOnStartup && EditorPrefs.GetBool("HideWelcomeWindow") == false)
			{
				EditorPrefs.SetBool("HideWelcomeWindow", true);
				Debug.Log("Rex Engine :: Setting Editor Prefs to Hide Welcome Window");
			}
			else if(willDisplayOnStartup && EditorPrefs.GetBool("HideWelcomeWindow") == true)
			{
				EditorPrefs.SetBool("HideWelcomeWindow", false);
				Debug.Log("Rex Engine :: Setting Editor Prefs to Show Welcome Window");
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}

	protected static void ShowRexSettings()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(RexSettings));
		window.titleContent = new GUIContent("Rex Settings");
	}

	protected static void ShowRexLevelEditor()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(RexLevelEditor));
		window.titleContent = new GUIContent("Rex Level Editor");
	}

	protected static void ShowRexPalette()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(RexPalette));
		window.titleContent = new GUIContent("Rex Palette");
	}
}
#endif
