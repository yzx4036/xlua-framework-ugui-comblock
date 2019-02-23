using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

#if UNITY_EDITOR
namespace RexEngine
{
	[CustomEditor(typeof(RexController), true)]
	public class RexControllerEditor:Editor 
	{

		public override void OnInspectorGUI()
		{
			RexController rexController = (RexController)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginHorizontal(); { GUILayout.Label(""); } EditorGUILayout.EndHorizontal();

			if(GUILayout.Button("Add RexState"))
			{
				GenericMenu menu = new GenericMenu();

				menu.AddItem(new GUIContent("Bounce"), false, RexMenu.AddBounceState);
				menu.AddItem(new GUIContent("Crouch"), false, RexMenu.AddCrouchState);
				menu.AddItem(new GUIContent("Dash"), false, RexMenu.AddDashState);
				menu.AddItem(new GUIContent("Flutter Jump"), false, RexMenu.AddFlutterJumpState);
				menu.AddItem(new GUIContent("Glide"), false, RexMenu.AddGlideState);
				menu.AddItem(new GUIContent("Ground Pound"), false, RexMenu.AddGroundPoundState);
				menu.AddItem(new GUIContent("Jump"), false, RexMenu.AddJumpState);
				menu.AddItem(new GUIContent("Knockback"), false, RexMenu.AddKnockbackState);
				menu.AddItem(new GUIContent("Ladder Climbing"), false, RexMenu.AddLadderState);
				menu.AddItem(new GUIContent("Landing"), false, RexMenu.AddLandingState);
				menu.AddItem(new GUIContent("Moving"), false, RexMenu.AddMovingState);
				menu.AddItem(new GUIContent("Stair Climbing"), false, RexMenu.AddStairClimbingState);
				menu.AddItem(new GUIContent("Wall Cling"), false, RexMenu.AddWallClingState);

				menu.ShowAsContext();
			}
		}
	}
}
#endif