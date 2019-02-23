/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint:MonoBehaviour
{
    void OnDrawGizmos()
	{
		gameObject.hideFlags = HideFlags.NotEditable;

		#if UNITY_EDITOR
		string path = "Assets/RexEngine/Gizmos/IconUnusable.png";
		Texture2D texture  = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

		GUIStyle iconStyle = new GUIStyle();
		iconStyle.alignment = TextAnchor.MiddleCenter;
		iconStyle.contentOffset = new Vector2(-10, -13);

		Handles.Label(transform.position, new GUIContent(texture), iconStyle);
		#endif
	}
}
