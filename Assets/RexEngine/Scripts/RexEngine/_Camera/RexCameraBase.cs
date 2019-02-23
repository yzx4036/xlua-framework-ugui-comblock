using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class RexCameraBase:MonoBehaviour 
	{
		public bool usePlayerAsTarget = true;

		//[HideInInspector]
		public Vector2 boundariesMin; //Set by objects like the Boundary object

		//[HideInInspector]
		public Vector2 boundariesMax; //Set by objects like the Boundary object

		[System.NonSerialized]
		public Vector2 shakeOffset; //The offset that ScreenShake is giving the camera; should not be set directly

		protected Transform focusObject; //The object the camera will focus on and follow

		void Start() 
		{
			ScreenShake.Instance.SetCamera(this);
		}

		public virtual void SetFocusObject(Transform _focusObject)
		{
			focusObject = _focusObject;
		}

		public virtual void NotifyOfShake()
		{
			
		}

		public virtual void CenterOnPlayer()
		{
			
		}

		public virtual void SetCameraBoundary(SceneBoundary.Edge _edge, float _value)
		{
			switch(_edge)
			{
				case SceneBoundary.Edge.Left:
					boundariesMin.x = _value + CameraHelper.GetScreenSizeInUnits().x * 0.5f;
					break;
				case SceneBoundary.Edge.Right:
					boundariesMax.x = _value - CameraHelper.GetScreenSizeInUnits().x * 0.5f;
					break;
				case SceneBoundary.Edge.Bottom:
					boundariesMin.y = _value + CameraHelper.GetScreenSizeInUnits().y * 0.5f;
					break;
				case SceneBoundary.Edge.Top:
					boundariesMax.y = _value - CameraHelper.GetScreenSizeInUnits().y * 0.5f;
					break;
			}
		}
	}
}
