/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

namespace RexEngine
{
	public class RexCamera:RexCameraBase 
	{
		[System.Serializable]
		public class Cameras
		{
			public Camera main;
			public Camera foreground;
			public Camera midground;
			public Camera background;
			public Camera canvas;
			public Camera ui;
		}

		public Scrolling scrolling;
		public LookAhead lookAhead;
		public Cameras cameras;

		public bool willTrackFocusObject;

		[System.Serializable]
		public class LookAhead
		{
			public bool useLookAhead = true;
			public float distance = 2.0f;
			public float smoothing = 3.0f;
		}

		[System.Serializable]
		public class Scrolling
		{
			public bool willScrollHorizontally = true;
			public bool willScrollVertically = true;
		}

		[System.Serializable]
		public class ScrollProperties
		{
			[HideInInspector]
			public bool willScrollForegroundX = true;

			[HideInInspector]
			public bool willScrollForegroundY = true;

			[HideInInspector]
			public bool willScrollMidgroundX = true;

			[HideInInspector]
			public bool willScrollMidgroundY = true;

			[HideInInspector]
			public bool willScrollBackgroundX = true;

			[HideInInspector]
			public bool willScrollBackgroundY = true;
		}

		//The below willScroll properties are set by a LevelManager automatically on its Awake based on the settings you give it in the Inspector
		[HideInInspector]
		public ScrollProperties secondaryScrolling;

		protected Vector2 offsetFromFocusObject;

		[HideInInspector]
		public Vector3 rawPosition; //Camera position before external factors like Shake

		void Awake() 
		{
			rawPosition = transform.position;

			if(cameras.canvas)
			{
				cameras.canvas.cullingMask = 1 << LayerMask.NameToLayer("Canvas");
			}

			if(cameras.background)
			{
				cameras.background.cullingMask = 1 << LayerMask.NameToLayer("Background");
			}

			if(cameras.midground)
			{
				cameras.midground.cullingMask = 1 << LayerMask.NameToLayer("Midground");
			}

			if(cameras.foreground)
			{
				cameras.foreground.cullingMask = 1 << LayerMask.NameToLayer("Foreground");
			}

			if(cameras.ui)
			{
				cameras.ui.cullingMask = 1 << LayerMask.NameToLayer("UI");
			}

			cameras.main.cullingMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("TransparentFX") | 1 << LayerMask.NameToLayer("Ignore Raycast") | 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("PassThroughBottom") | 1 << LayerMask.NameToLayer("Stairs");
		}

		void LateUpdate() 
		{
			if(Time.timeScale > 0)
			{
				UpdateCameras();
			}
		}

		public override void SetFocusObject(Transform _focusObject)
		{
			focusObject = _focusObject;
		}

		public void SetPosition(Vector2 position)
		{
			rawPosition = new Vector3(position.x, position.y, rawPosition.z);
			transform.position = rawPosition;

			UpdateCameras();
		}

		protected void UpdateCameras()
		{
			Vector3 newPosition;

			newPosition = FocusOnFocusObject(rawPosition);
			newPosition = ApplyLookAhead(newPosition);
			newPosition = SnapToCameraBoundaries(newPosition);
			newPosition = StopDirectionalScrollingIfDisabled(newPosition);

			rawPosition = newPosition;

			newPosition = ApplyShake(newPosition);

			transform.position = newPosition;

			ScrollSecondaryCameras();
		}

		protected Vector3 LerpToNewPosition(Vector3 position)
		{
			Vector2 maxChange = new Vector2(0.75f, 1.5f);

			Vector3 adjustedPosition = position;
			if(Mathf.Abs(position.x - transform.position.x) > maxChange.x)
			{
				if(adjustedPosition.x > transform.position.x)
				{
					adjustedPosition.x = transform.position.x + maxChange.x;
				}
				else if(adjustedPosition.x < transform.position.x)
				{
					adjustedPosition.x = transform.position.x - maxChange.x;
				}
			}

			if(Mathf.Abs(position.y - transform.position.y) > maxChange.y)
			{
				if(adjustedPosition.y > transform.position.y)
				{
					adjustedPosition.y = transform.position.y + maxChange.y;
				}
				else if(adjustedPosition.y < transform.position.y)
				{
					adjustedPosition.y = transform.position.y - maxChange.y;
				}
			}

			return adjustedPosition;
		}

		protected Vector3 ApplyLookAhead(Vector3 position)
		{
			Vector3 adjustedPosition = position;
			if(lookAhead.useLookAhead && focusObject != null)
			{
				float adjustedDistance = (focusObject.transform.localScale.x >= 0.0f) ? lookAhead.distance : -lookAhead.distance;
				bool willSnapImmediatelyToTarget = RexSceneManager.Instance.isLoadingNewScene;
				float adjustedSmoothing = (willSnapImmediatelyToTarget)? 1000.0f : lookAhead.smoothing;
				Vector3 targetPosition = new Vector3(adjustedPosition.x + adjustedDistance, adjustedPosition.y, adjustedPosition.z);
				adjustedPosition = Vector3.Lerp(transform.position, targetPosition, adjustedSmoothing * Time.deltaTime);
			}

			return adjustedPosition;
		}

		protected Vector3 FocusOnFocusObject(Vector3 position)
		{
			Vector3 adjustedPosition = position;
			if(willTrackFocusObject && focusObject != null)
			{
				adjustedPosition = new Vector3(focusObject.transform.position.x + offsetFromFocusObject.x, focusObject.transform.position.y + offsetFromFocusObject.y, transform.position.z);
			}

			return adjustedPosition;
		}

		protected Vector3 ApplyShake(Vector3 position)
		{
			Vector3 positionWithShake = new Vector3(position.x + shakeOffset.x, position.y + shakeOffset.y, transform.position.z);
			return positionWithShake;
		}

		protected Vector3 SnapToCameraBoundaries(Vector3 position)
		{
			if(position.y > boundariesMax.y)
			{
				position.y = boundariesMax.y;
			}

			if(position.y < boundariesMin.y)
			{
				position.y = boundariesMin.y;
			}

			if(position.x > boundariesMax.x)
			{
				position.x = boundariesMax.x;
			}

			if(position.x < boundariesMin.x)
			{
				position.x = boundariesMin.x;
			}

			return position;
		}

		protected Vector3 StopDirectionalScrollingIfDisabled(Vector3 position)
		{
			Vector3 stoppedPosition = position;
			if(!scrolling.willScrollHorizontally)
			{
				stoppedPosition.x = transform.position.x - shakeOffset.x;
			}
			if(!scrolling.willScrollVertically)
			{
				stoppedPosition.y = transform.position.y - shakeOffset.y;
			}

			return stoppedPosition;
		}

		private void ScrollSecondaryCameras()
		{
			float backgroundOffset = 0.35f;
			float midgroundOffset = 0.5f;
			float foregroundOffset = 1.2f; 

			if(cameras.foreground != null)
			{
				float foregroundX = (secondaryScrolling.willScrollForegroundX) ? transform.position.x * foregroundOffset : transform.position.x;
				float foregroundY = (secondaryScrolling.willScrollForegroundY) ? transform.position.y * foregroundOffset : transform.position.y;
				cameras.foreground.transform.position = new Vector3(foregroundX, foregroundY, cameras.foreground.transform.position.z);
			}

			if(cameras.midground != null)
			{
				float midgroundX = (secondaryScrolling.willScrollMidgroundX) ? transform.position.x * midgroundOffset : transform.position.x;
				float midgroundY = (secondaryScrolling.willScrollMidgroundY) ? transform.position.y * midgroundOffset : transform.position.y;
				cameras.midground.transform.position = new Vector3(midgroundX, midgroundY, cameras.midground.transform.position.z);
			}

			if(cameras.background != null)
			{
				float backgroundX = (secondaryScrolling.willScrollBackgroundX) ? transform.position.x * backgroundOffset : transform.position.x;
				float backgroundY = (secondaryScrolling.willScrollBackgroundY) ? transform.position.y * backgroundOffset : transform.position.y;
				cameras.background.transform.position = new Vector3(backgroundX, backgroundY, cameras.background.transform.position.z);
			}
		}

		void OnDestroy()
		{
			cameras.main = null;
			focusObject = null;	
		}
	}
}
