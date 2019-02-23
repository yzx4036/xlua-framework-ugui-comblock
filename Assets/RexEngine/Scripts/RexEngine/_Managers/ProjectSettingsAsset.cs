using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	public class ProjectSettingsAsset:MonoBehaviour 
	{
		public RexSettingsData rexSettingsData;

		protected static bool wasUsingDefaults;

		private static ProjectSettingsAsset instance = null;
		public static ProjectSettingsAsset Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<ProjectSettingsAsset>();
					go.name = "ProjectSettingsAsset";
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

			LoadSettings();
		}

		void Start()
		{
			#if UNITY_EDITOR
			if(wasUsingDefaults)
			{
				SavePrefab();
			}
			#endif
		}

		public void SavePrefab()
		{
			#if UNITY_EDITOR
			GameObject singletons = GameObject.Find("Singletons");
			if(singletons != null)
			{
				string localPath = "Assets/RexEngine/Resources/System/Singletons.prefab";
				Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
				PrefabUtility.ReplacePrefab(singletons, prefab, ReplacePrefabOptions.ConnectToPrefab);
				EditorUtility.SetDirty(singletons);
			}
			#endif
		}

		public void LoadSettings()
		{
			#if UNITY_EDITOR
			if(rexSettingsData == null || rexSettingsData.name == "RexEngineDefaultSettings")
			{
				Debug.Log("ProjectSettingsAsset :: No Rex Settings Data found.");

				string[] results;

				bool didFindCustomSettings = false;

				wasUsingDefaults = true;

				results = AssetDatabase.FindAssets("RexEngineSettings");
				foreach(string guid in results)
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					RexSettingsData existingRexSettingsData = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(RexSettingsData)) as RexSettingsData;
					rexSettingsData = existingRexSettingsData;
					didFindCustomSettings = true;
				}

				if(!didFindCustomSettings)
				{
					results = AssetDatabase.FindAssets("RexEngineDefaultSettings");
					foreach(string guid in results)
					{
						string path = AssetDatabase.GUIDToAssetPath(guid);
						Debug.Log("ProjectSettingsAsset :: Creating new user settings.");
						string newPath = "Assets/RexEngineSettings.asset";
						AssetDatabase.CopyAsset(path, newPath);
						RexSettingsData defaultRexSettingsData = UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, typeof(RexSettingsData)) as RexSettingsData;
						rexSettingsData = defaultRexSettingsData;
					}
				}
			}
			#endif

			if(rexSettingsData == null)
			{
				rexSettingsData = Resources.Load("DefaultSettings/RexEngineDefaultSettings.asset") as RexSettingsData;
			}
		}
	}
}