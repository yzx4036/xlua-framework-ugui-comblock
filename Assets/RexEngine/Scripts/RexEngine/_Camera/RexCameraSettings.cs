using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class RexCameraSettings:MonoBehaviour 
	{
		public Transform focusObject;

		void Start() 
		{
			RexCameraBase rexCamera = Camera.main.GetComponent<RexCameraBase>();
			if(rexCamera != null)
			{
				rexCamera.SetFocusObject(focusObject);
			}
		}
	}
}
