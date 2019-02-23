/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace RexEngine
{
	[SelectionBase]
	public class SceneBoundary:MonoBehaviour
	{
		public enum Edge
		{
			Left,
			Right,
			Bottom,
			Top,
			None
		}

		[HideInInspector]
		public Edge edge;

		[HideInInspector]
		public int gridIndex;

		[Tooltip("If set to True, this will kill the player when they enter it.")]
		public bool isBottomlessPit; 
		[Tooltip("If set to True, actors will treat this like a wall.")]
		public bool isSolid; 
		[Tooltip("If set to True, this SceneBoundary can only be positioned in multiples of the global Room Size value, as defined in GlobalValues.cs. If False, this can be positioned anywhere.")]
		public bool willSnapToSceneEdges = true;

		private BoxCollider2D boxCollider;
		private SceneBoundaries sceneBoundaries;

		public class SceneBoundaries
		{
			public SceneBoundary top;
			public SceneBoundary bottom;
			public SceneBoundary left;
			public SceneBoundary right;
		}

		void Start() 
		{

		}

		public void Init()
		{
			SetSceneBoundary();
			PositionCollider();

			if(isSolid)
			{
				gameObject.layer = LayerMask.NameToLayer("Boundaries");
			}
		}

		public void SetPosition(Vector2 position)
		{
			transform.position = new Vector3(position.x, position.y, 0.0f);
			SetSceneBoundary();
		}

		public void SetSceneBoundary()
		{
			RexCameraBase camera = Camera.main.GetComponent<RexCameraBase>();
			if(camera != null)
			{
				if(edge == Edge.Left)
				{
					camera.SetCameraBoundary(Edge.Left, transform.position.x);
					//camera.boundariesMin.x = transform.position.x + CameraHelper.GetScreenSizeInUnits().x * 0.5f;
				}
				else if(edge == Edge.Right)
				{
					camera.SetCameraBoundary(Edge.Right, transform.position.x);
					//camera.boundariesMax.x = transform.position.x - CameraHelper.GetScreenSizeInUnits().x * 0.5f;
				}
				else if(edge == Edge.Bottom)
				{
					camera.SetCameraBoundary(Edge.Bottom, transform.position.y);
					//camera.boundariesMin.y = transform.position.y + CameraHelper.GetScreenSizeInUnits().y * 0.5f;
				}
				else if(edge == Edge.Top)
				{
					camera.SetCameraBoundary(Edge.Top, transform.position.y);
					//camera.boundariesMax.y = transform.position.y - CameraHelper.GetScreenSizeInUnits().y * 0.5f;
				}
			}
		}

		void OnDrawGizmos() 
		{
			#if UNITY_EDITOR
			gameObject.hideFlags = HideFlags.NotEditable;

			if(edge == Edge.Left)
			{
				if(sceneBoundaries == null)
				{
					GetSceneBoundaries();
				}

				DrawLabel();
				sceneBoundaries.right.DrawLabel();
				sceneBoundaries.bottom.DrawLabel();
				sceneBoundaries.top.DrawLabel();

				DrawLine();
				sceneBoundaries.right.DrawLine();
				sceneBoundaries.bottom.DrawLine();
				sceneBoundaries.top.DrawLine();

			}
			#endif
		}

		public void GetSceneBoundaries()
		{
			sceneBoundaries = new SceneBoundaries();
			sceneBoundaries.left = GameObject.Find("SceneBoundary_left").GetComponent<SceneBoundary>();
			sceneBoundaries.right = GameObject.Find("SceneBoundary_right").GetComponent<SceneBoundary>();
			sceneBoundaries.top = GameObject.Find("SceneBoundary_top").GetComponent<SceneBoundary>();
			sceneBoundaries.bottom = GameObject.Find("SceneBoundary_bottom").GetComponent<SceneBoundary>();
		}

		public void DrawLine()
		{
			if(sceneBoundaries == null)
			{
				GetSceneBoundaries();
				return;
			}

			if(boxCollider == null)
			{
				boxCollider = GetComponent<BoxCollider2D>();
				return;
			}

			#if UNITY_EDITOR
			Handles.color = Color.white;

			Vector3 startPoint = new Vector3(0.0f, 0.0f, 1.1f);
			Vector3 endPoint = new Vector3(0.0f, 0.0f, 1.1f);
			Vector2 sideLineOffset = new Vector2(0, 0);
			float sideLineOffsetSize = 0.05f;

			if(edge == Edge.Top || edge == Edge.Bottom)
			{
				startPoint.x = boxCollider.bounds.min.x + GetMarginSize();
				endPoint.x = boxCollider.bounds.max.x - GetMarginSize();
				startPoint.y = endPoint.y = transform.position.y;

				if(edge == Edge.Top)
				{
					sideLineOffset.y = sideLineOffsetSize;
				}
				else
				{
					sideLineOffset.y = -sideLineOffsetSize;
				}
			}
			else if(edge == Edge.Left || edge == Edge.Right)
			{
				startPoint.x = endPoint.x = transform.position.x;
				startPoint.y = boxCollider.bounds.min.y + GetMarginSize();
				endPoint.y = boxCollider.bounds.max.y - GetMarginSize();

				if(edge == Edge.Left)
				{
					sideLineOffset.x = -sideLineOffsetSize;
				}
				else
				{
					sideLineOffset.x = sideLineOffsetSize;
				}
			}

			Handles.DrawLine(startPoint, endPoint);
			#endif

			PositionCollider();
		}

		protected void DrawLabel()
		{
			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if(willSnapToSceneEdges)
				{
					SnapToRoomSize();
				}

				if(boxCollider == null)
				{
					boxCollider = GetComponent<BoxCollider2D>();
				}

				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.white;
				style.fontSize = 8;
				style.fontStyle = FontStyle.Bold;
				string prefix = (edge == Edge.Top || edge == Edge.Bottom) ? "Y: " : "X: ";
				string text = prefix + gridIndex.ToString();

				if(!willSnapToSceneEdges)
				{
					text = "Free";
				}

				Vector3 position = transform.position;
				string newName = "SceneBoundary_";
				switch(edge)
				{
					case Edge.Left:
						style.contentOffset = new Vector2(-29.0f, -8.0f);
						position.y = boxCollider.bounds.min.y + boxCollider.size.y * 0.5f;
						newName += "left";
						break;
					case Edge.Right:
						style.contentOffset = new Vector2(18.0f, -8.0f);
						position.y = boxCollider.bounds.min.y + boxCollider.size.y * 0.5f;
						newName += "right";
						break;
					case Edge.Bottom:
						position.x = boxCollider.bounds.min.x + boxCollider.size.x * 0.5f;
						style.contentOffset = new Vector2(-6.0f, 12.0f);
						newName += "bottom";
						break;
					case Edge.Top:
						position.x = boxCollider.bounds.min.x + boxCollider.size.x * 0.5f;
						style.contentOffset = new Vector2(-6.0f, -27.0f);
						newName += "top";
						break;
				}

				if(Selection.activeGameObject == gameObject)
				{
					Handles.Label(position, text, style);
				}

				string gameObjectName = gameObject.name.Split('_')[0];
				if(gameObjectName == "CameraBoundary")
				{
					gameObject.name = newName;
					EditorUtility.SetDirty(gameObject);
					EditorSceneManager.MarkSceneDirty(gameObject.scene);
				}

			}
			#endif
		}

		protected void SnapToRoomSize()
		{
			int roomPosition = 0;
			if(edge == Edge.Right)
			{
				roomPosition = Mathf.CeilToInt((transform.position.x - (GlobalValues.roomSize.x * 0.5f)) / GlobalValues.roomSize.x);
				if(roomPosition < 0)
				{
					roomPosition = 0;
				}

				transform.position = new Vector3((roomPosition * GlobalValues.roomSize.x) + (GlobalValues.roomSize.x * 0.5f), transform.position.y, transform.position.z);
			}
			else if(edge == Edge.Left)
			{
				roomPosition = Mathf.FloorToInt((transform.position.x + (GlobalValues.roomSize.x * 0.5f)) / GlobalValues.roomSize.x);
				if(roomPosition > 0)
				{
					roomPosition = 0;
				}
				transform.position = new Vector3((roomPosition * GlobalValues.roomSize.x) - (GlobalValues.roomSize.x * 0.5f), transform.position.y, transform.position.z);
			}
			else if(edge == Edge.Top)
			{
				roomPosition = Mathf.FloorToInt((transform.position.y - (GlobalValues.roomSize.y * 0.5f)) / GlobalValues.roomSize.y);
				if(roomPosition < 0)
				{
					roomPosition = 0;
				}
				transform.position = new Vector3(transform.position.x, (roomPosition * GlobalValues.roomSize.y) + (GlobalValues.roomSize.y * 0.5f), transform.position.z);
			}
			else if(edge == Edge.Bottom)
			{
				roomPosition = Mathf.CeilToInt((transform.position.y + (GlobalValues.roomSize.y * 0.5f)) / GlobalValues.roomSize.y);
				if(roomPosition > 0)
				{
					roomPosition = 0;
				}
				transform.position = new Vector3(transform.position.x, (roomPosition * GlobalValues.roomSize.y) - (GlobalValues.roomSize.y * 0.5f), transform.position.z);
			}

			gridIndex = roomPosition;

			if(edge == Edge.Top || edge == Edge.Right)
			{
				gridIndex += 1; //This makes sure the grid index displays properly in gizmos
			}
		}

		public float GetBoundarySize() //TODO: Sync this up better with the sideOffset value in PositionCollider so you aren't hard-coding the same value in two places
		{
			float marginSize = 10.0f * GlobalValues.tileSize * 2.0f;
			return (edge == Edge.Left || edge == Edge.Right) ? GetComponent<BoxCollider2D>().size.y - marginSize : GetComponent<BoxCollider2D>().size.x - marginSize;
		}

		public float GetMarginSize()
		{
			return 10.0f * GlobalValues.tileSize;
		}

		private void PositionCollider()
		{
			if(boxCollider == null)
			{
				boxCollider = GetComponent<BoxCollider2D>();
			}

			if(boxCollider == null)
			{
				return;
			}

			if(sceneBoundaries == null)
			{
				GetSceneBoundaries();
			}

			float sideOffset = 5.0f * GlobalValues.tileSize;
			float size = 10.0f * GlobalValues.tileSize;

			if(edge == Edge.Right)
			{
				if(sceneBoundaries.top && sceneBoundaries.bottom && boxCollider != null)
				{
					boxCollider.size = new Vector2(size, Mathf.Abs(sceneBoundaries.top.transform.position.y - sceneBoundaries.bottom.transform.position.y) + size * 2.0f);
					boxCollider.offset = new Vector2(sideOffset, sceneBoundaries.top.transform.position.y - boxCollider.size.y / 2.0f + size);
					transform.position = new Vector3(transform.position.x, 0.0f, 0.0f);
				}
			}
			else if(edge == Edge.Left)
			{
				if(sceneBoundaries.top && sceneBoundaries.bottom && boxCollider != null)
				{
					boxCollider.size = new Vector2(size, Mathf.Abs(GameObject.Find("SceneBoundary_top").transform.position.y - sceneBoundaries.bottom.transform.position.y) + size * 2.0f);
					boxCollider.offset = new Vector2(-sideOffset, GameObject.Find("SceneBoundary_top").transform.position.y - boxCollider.size.y / 2.0f + size);
					transform.position = new Vector3(transform.position.x, 0.0f, 0.0f);
				}
			}
			else if(edge == Edge.Top)
			{
				if(sceneBoundaries.left && sceneBoundaries.right && boxCollider != null)
				{
					boxCollider.size = new Vector2(Mathf.Abs(sceneBoundaries.left.transform.position.x - sceneBoundaries.right.transform.position.x) + size * 2.0f, size);
					boxCollider.offset = new Vector2(sceneBoundaries.left.transform.position.x + boxCollider.size.x / 2.0f - size, sideOffset);
					transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
				}
			}
			else if(edge == Edge.Bottom)
			{
				if(sceneBoundaries.left && sceneBoundaries.right && boxCollider != null)
				{
					boxCollider.size = new Vector2(Mathf.Abs(sceneBoundaries.left.transform.position.x - sceneBoundaries.right.transform.position.x) + size * 2.0f, size);
					boxCollider.offset = new Vector2(sceneBoundaries.left.transform.position.x + boxCollider.size.x / 2.0f - size, -sideOffset);
					transform.position = new Vector3(0.0f, transform.position.y, 0.0f);
				}
			}
		}

		private void ProcessCollision(Collider2D col)
		{
			RexObject rexObject = col.GetComponent<RexObject>();
			if(rexObject != null)
			{
				RexActor actor = col.GetComponent<RexActor>();
				if(actor != null && actor.isDead)
				{
					return;
				}

				if(rexObject.willDespawnOnSceneExit || (isBottomlessPit && rexObject.willDieInBottomlessPits))
				{
					float buffer = 0.5f * GlobalValues.tileSize;
					float warningBuffer = 0.15f * GlobalValues.tileSize;
					if(edge == Edge.Right)
					{
						if(rexObject.transform.position.x > transform.position.x + buffer)
						{
							rexObject.Clear();
						}
						else if(rexObject.transform.position.x > transform.position.x + warningBuffer)
						{
							rexObject.OnSceneBoundaryCollisionInsideBuffer();
						}
					}
					else if(edge == Edge.Left)
					{
						if(rexObject.transform.position.x < transform.position.x - buffer)
						{
							rexObject.Clear();
						}
						else if(rexObject.transform.position.x < transform.position.x - warningBuffer)
						{
							rexObject.OnSceneBoundaryCollisionInsideBuffer();
						}
					}
					else if(edge == Edge.Top)
					{
						if(rexObject.transform.position.y > transform.position.y + buffer)
						{
							rexObject.Clear();
						}
						else if(rexObject.transform.position.y > transform.position.y + warningBuffer)
						{
							rexObject.OnSceneBoundaryCollisionInsideBuffer();
						}
					}
					else if(edge == Edge.Bottom)
					{
						if(rexObject.transform.position.y < transform.position.y - buffer)
						{
							rexObject.Clear();

						}
						else if(rexObject.transform.position.y < transform.position.y - warningBuffer)
						{
							rexObject.OnSceneBoundaryCollisionInsideBuffer();
						}
					}
				}
			}
		}

		protected void CheckBottomlessPit(Collider2D col)
		{
			float bottomlessPitBuffer = 1.25f * GlobalValues.tileSize;
			if(isBottomlessPit)
			{
				if(GameManager.Instance.player != null && !GameManager.Instance.player.isBeingLoadedIntoNewScene && col.GetComponent<RexActor>() != null && !col.GetComponent<RexActor>().isDead)
				{
					if(edge == Edge.Bottom)
					{
						if(col.transform.position.y < transform.position.y - bottomlessPitBuffer)
						{
							GameManager.Instance.OnPlayerEnteredBottomlessPit(col.GetComponent<RexActor>());
						}
						else
						{
							ReEnableCollider();
						}
					}
					else if(edge == Edge.Top)
					{
						if(col.transform.position.y > transform.position.y + bottomlessPitBuffer)
						{
							GameManager.Instance.OnPlayerEnteredBottomlessPit(col.GetComponent<RexActor>());
						}
						else
						{
							ReEnableCollider();
						}
					}
				}
			}
		}

		private void ReEnableCollider()
		{
			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			if(collider != null)
			{
				collider.enabled = false;
				collider.enabled = true;
			}
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player" && col.GetComponent<RexActor>())
			{
				SetSceneBoundary();
				CheckBottomlessPit(col);
				if(col.transform.position.x > transform.position.x)
				{
					col.GetComponent<RexActor>().SetPosition(new Vector2(col.transform.position.x, col.transform.position.y));
				}
			}
			else if(col.tag != "Terrain" && col.tag != "Untagged")
			{
				ProcessCollision(col);
			}
		}
	}
}
