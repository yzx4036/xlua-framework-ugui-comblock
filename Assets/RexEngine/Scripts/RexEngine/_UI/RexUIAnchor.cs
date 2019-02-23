/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

public class RexUIAnchor:MonoBehaviour 
{
	public enum Anchor
	{
		Center,
		Upper,
		UpperRight,
		Right,
		LowerRight,
		Lower,
		LowerLeft,
		Left,
		UpperLeft
	}

	public Anchor anchor = Anchor.Center;
	public Vector2 distanceFromAnchor;
	public bool isAnchoredByDefault = true;
	public string cameraName;

    void Start() 
	{
		if(isAnchoredByDefault)
		{
			SnapToAnchor();
		}
	}

	public void SnapToAnchor()
	{
		transform.position = GetAnchoredPosition(anchor, distanceFromAnchor);
	}

	public Vector3 GetAnchoredPosition(Anchor _anchor, Vector2 _distanceFromAnchor)
	{
		Vector3 anchoredPosition;
		Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0.0f);
		Camera camera;
		if(cameraName != "" && GameObject.Find(cameraName) != null)
		{
			camera = GameObject.Find(cameraName).GetComponent<Camera>();
		}
		else
		{
			camera = Camera.main;
		}

		Vector3 screenSizeInUnits = camera.ScreenToWorldPoint(screenSize);
		Vector2 startingPosition = new Vector2(0, 0);

		switch(anchor)
		{
			case Anchor.Center:
				startingPosition.x = screenSizeInUnits.x * 0.0f;
				startingPosition.y = screenSizeInUnits.y * 0.0f;
				break;
			case Anchor.Upper:
				startingPosition.x = screenSizeInUnits.x * 0.0f;
				startingPosition.y = screenSizeInUnits.y * 1.0f;
				break;
			case Anchor.UpperRight:
				startingPosition.x = screenSizeInUnits.x * 1.0f;
				startingPosition.y = screenSizeInUnits.y * 1.0f;
				break;
			case Anchor.Right:
				startingPosition.x = screenSizeInUnits.x * 1.0f;
				startingPosition.y = screenSizeInUnits.y * 0.0f;
				break;
			case Anchor.LowerRight:
				startingPosition.x = screenSizeInUnits.x * 1.0f;
				startingPosition.y = screenSizeInUnits.y * -1.0f;
				break;
			case Anchor.Lower:
				startingPosition.x = screenSizeInUnits.x * 0.0f;
				startingPosition.y = screenSizeInUnits.y * -1.0f;
				break;
			case Anchor.LowerLeft:
				startingPosition.x = screenSizeInUnits.x * -1.0f;
				startingPosition.y = screenSizeInUnits.y * -1.0f;
				break;
			case Anchor.Left:
				startingPosition.x = screenSizeInUnits.x * -1.0f;
				startingPosition.y = screenSizeInUnits.y * 0.0f;
				break;
			case Anchor.UpperLeft:
				startingPosition.x = screenSizeInUnits.x * -1.0f;
				startingPosition.y = screenSizeInUnits.y * 1.0f;
				break;
			default:
				startingPosition.x = screenSizeInUnits.x * 0.0f;
				startingPosition.y = screenSizeInUnits.y * 0.0f;
				break;
		}

		anchoredPosition = new Vector3(startingPosition.x + distanceFromAnchor.x, startingPosition.y + distanceFromAnchor.y, transform.position.z);

		return anchoredPosition;
	}
}
