/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	public class PhysicsManager:MonoBehaviour 
	{
		public enum UpdateType
		{
			Update,
			Fixed,
			FixedWithInterpolation
		}

		public float fixedDeltaTime = 0.01667f;
		public UpdateType updateType;
		public float gravityScale = 1.0f;

		[System.NonSerialized]
		public bool isSceneLoading = false;

		protected float previousGravityScale = 1.0f;
		public List<RexPhysics> physicsObjects;
		public List<RexPhysics> physicsMovers;

		private static PhysicsManager instance = null;
		public static PhysicsManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<PhysicsManager>();
					go.name = "PhysicsManager";
					DontDestroyOnLoad(go);
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

			#if !UNITY_WEBGL
			Application.targetFrameRate = 60;
			#endif

			QualitySettings.vSyncCount = 1; //If vSync is set to 0, then FPS matters; iOS ALWAYS has Sync at 1
			physicsObjects = new List<RexPhysics>();
			physicsMovers = new List<RexPhysics>();
			updateType = UpdateType.FixedWithInterpolation;
		}

		void Update()
		{
			if(updateType == UpdateType.Update)
			{
				MovePhysics();
				return;
			}

			if(updateType == UpdateType.FixedWithInterpolation)
			{
				foreach(RexPhysics physicsObject in physicsMovers)
				{
					if(physicsObject.isEnabled)
					{
						float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
						Vector3 lerpState = Vector3.Lerp(physicsObject.previousFrameProperties.position, physicsObject.properties.position, alpha);
						physicsObject.transform.position = lerpState;
					}
				}

				foreach(RexPhysics physicsObject in physicsObjects)
				{
					if(physicsObject.isEnabled)
					{
						float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
						Vector3 lerpState = Vector3.Lerp(physicsObject.previousFrameProperties.position, physicsObject.properties.position, alpha);
						physicsObject.transform.position = lerpState;
					}
				}
			}
		}

		void FixedUpdate() 
		{
			if(updateType != UpdateType.Update)
			{
				MovePhysics();
				CheckGravityScale();
			}
		}

		public void RegisterPhysicsObject(RexPhysics physicsObject)
		{
			if(physicsObject.GetComponent<PhysicsMover>())
			{
				physicsMovers.Add(physicsObject);
			}
			else
			{
				physicsObjects.Add(physicsObject);
			}
		}

		public void UnregisterPhysicsObject(RexPhysics physicsObject)
		{
			if(physicsObjects.Contains(physicsObject))
			{
				physicsObjects.Remove(physicsObject);
			}
			else if(physicsMovers.Contains(physicsObject))
			{
				physicsMovers.Remove(physicsObject);
			}
		}

		public void OnDestroy()
		{
			for(int i = physicsObjects.Count - 1; i >= 0; i--)
			{
				RexPhysics physicsObject = physicsObjects[i];
				Destroy(physicsObject);
			}

			for(int i = physicsMovers.Count - 1; i >= 0; i--)
			{
				RexPhysics physicsObject = physicsMovers[i];
				Destroy(physicsObject);
			}
		}

		public void FlipGravity()
		{
			SetGravityScale(gravityScale * -1);
		}

		protected void MovePhysics()
		{
			if(Time.timeScale > 0 && !isSceneLoading)
			{
				for(int i = physicsMovers.Count - 1; i >= 0; i --)
				{
					RexPhysics physicsObject = physicsMovers[i];
					if(physicsObject != null && physicsObject.isEnabled)
					{
						physicsObject.ResetFlags();

						physicsObject.StepPhysics();
						physicsObject.transform.position = physicsObject.properties.position;
					}
				}

				for(int i = physicsObjects.Count - 1; i >= 0; i --)
				{
					RexPhysics physicsObject = physicsObjects[i];
					if(physicsObject != null && physicsObject.isEnabled)
					{
						physicsObject.ResetFlags();

						physicsObject.StepPhysics();
						physicsObject.transform.position = physicsObject.properties.position;
					}
				}
			}
		}

		protected void CheckGravityScale()
		{
			if(gravityScale != previousGravityScale)
			{
				SetGravityScale(gravityScale);
			}

			previousGravityScale = gravityScale;
		}

		protected void SetGravityScale(float _gravityScale)
		{
			gravityScale = _gravityScale;
			for(int i = physicsObjects.Count - 1; i >= 0; i--)
			{
				physicsObjects[i].SyncGravityScale();
			}
		}
	}


}
