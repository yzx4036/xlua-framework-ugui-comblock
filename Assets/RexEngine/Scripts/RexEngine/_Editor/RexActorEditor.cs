using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using RexEngine;

#if UNITY_EDITOR
namespace RexEngine
{
	[CustomEditor(typeof(RexActor), true)]
	public class RexActorEditor:Editor 
	{
		protected bool showFieldEditorOptions;
		protected bool showFieldReplace;
		protected string scriptNameString;

		public override void OnInspectorGUI()
		{
			RexActor rexActor = (RexActor)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			showFieldEditorOptions = EditorGUILayout.Foldout(showFieldEditorOptions, new GUIContent("Editor Options", "Automatically fill the slots for this actor, or replace this RexActor script with another one."));
			if(showFieldEditorOptions)
			{
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Auto-Fill Slots"))
				{
					RexMenu.FillRexActorSlots();
				}
				EditorGUILayout.EndHorizontal();

				//newActor = EditorGUILayout.ObjectField(new GUIContent("New Actor", "The RexActor prefab to load for Player 1; this will be automatically loaded when the game starts."), newActor, typeof(RexActor), true) as RexActor;		

				showFieldReplace = EditorGUILayout.Foldout(showFieldReplace, new GUIContent("Swap RexActor Script", "Replace this RexActor script with another one."));
				if(showFieldReplace)
				{
					scriptNameString = EditorGUILayout.TextField(new GUIContent("New Script Name", "The name of the script being used to replace this current RexActor script. The new script must extend RexActor. If the new script is part of a specific namespace, the name of that namespace must be included here in dot syntax format (i.e. RexEngine.RexActor)"), scriptNameString);
					if(scriptNameString != "")
					{
						EditorGUILayout.BeginHorizontal();
						if(GUILayout.Button("Swap RexActor Script"))
						{
							RexMenu.SwapRexActorScript(rexActor, scriptNameString);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
		}
	}
}
#endif