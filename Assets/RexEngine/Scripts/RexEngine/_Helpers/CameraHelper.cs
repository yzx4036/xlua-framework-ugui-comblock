/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

public class CameraHelper:MonoBehaviour 
{
	public static bool CameraContainsPoint(Vector3 point, float buffer = 0.0f)
	{
		Rect rect = new Rect();
		rect.xMin = GetLeftEdgeOfCamera() - buffer;
		rect.xMax = GetRightEdgeOfCamera() + buffer;
		rect.yMin = GetBottomEdgeOfCamera() - buffer;
		rect.yMax = GetTopEdgeOfCamera() + buffer;
	
		if(rect.Contains(point))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public static Vector3 GetScreenSizeInUnits(Camera _camera = null)
	{
		if(_camera == null)
		{
			_camera = Camera.main;
		}

		Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0.0f);
		Vector3 screenSizeInUnits;

		if(!_camera.orthographic)
		{
			screenSizeInUnits = new Vector3(_camera.WorldToViewportPoint(screenSize * 0.5f).x, _camera.WorldToViewportPoint(screenSize * 0.5f).y, _camera.WorldToViewportPoint(screenSize * 0.5f).z);
		}
		else
		{
			float height = Camera.main.orthographicSize * 2;
			float width = height * screenSize.x / screenSize.y;
			screenSizeInUnits = new Vector3(width, height, 0.0f);
		}

		return screenSizeInUnits;
	}

	public static float GetLeftEdgeOfCamera()
	{
		return Camera.main.transform.position.x - GetScreenSizeInUnits().x * 0.5f;
	}
	
	public static float GetRightEdgeOfCamera()
	{
		return Camera.main.transform.position.x + GetScreenSizeInUnits().x * 0.5f;
	}

	public static float GetTopEdgeOfCamera()
	{
		return Camera.main.transform.position.y + GetScreenSizeInUnits().y * 0.5f;
	}

	public static float GetBottomEdgeOfCamera()
	{
		return Camera.main.transform.position.y - GetScreenSizeInUnits().y * 0.5f;
	}
}
