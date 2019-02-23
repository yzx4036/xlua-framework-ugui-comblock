/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	[ExecuteInEditMode]
	[SelectionBase]
	public class Stairs:MonoBehaviour
	{
		public int tiles = 5;
		public Sprites sprites;

		[System.Serializable]
		public class Sprites
		{
			public GameObject middleSprite;
		}

		private int previousTiles;

		void Awake() 
		{
			gameObject.tag = "Stairs";
			gameObject.layer = LayerMask.NameToLayer("Stairs");
		}

		#if UNITY_EDITOR
		void Update()
		{
			if(!Application.isPlaying)
			{
				if(tiles < 1)
				{
					tiles = 1;
				}

				if(previousTiles != tiles)
				{
					EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
					Vector2[] newPoints = new Vector2[2];
					collider.isTrigger = true;
					newPoints[0] = new Vector2(-tiles * 0.5f, -tiles * 0.5f);
					newPoints[1] = new Vector2(tiles * 0.5f, tiles * 0.5f);

					collider.points = newPoints;

					CreateSprites();
				}

				previousTiles = tiles;
			}
		}
		#endif 

		protected void CreateSprites()
		{
			if(sprites.middleSprite == null)
			{
				return;
			}

			foreach(SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
			{
				DestroyImmediate(spriteRenderer.gameObject);
			}

			Transform spriteHolder = transform.Find("Sprites");

			EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
			float tileRight = (collider.points[1].x) - (GlobalValues.tileSize * 0.5f);
			float tileTop = (collider.points[1].y) - (GlobalValues.tileSize * 0.5f);
			for(int i = 0; i < tiles; i ++)
			{
				GameObject prefab = sprites.middleSprite;
				GameObject segment = Instantiate(prefab) as GameObject;
				segment.transform.parent = transform;
				segment.transform.localPosition = new Vector3(tileRight - (i * GlobalValues.tileSize), tileTop - (i * GlobalValues.tileSize), 0.0f);
				segment.transform.localScale = Vector3.one;

				if(spriteHolder != null)
				{
					segment.transform.parent = spriteHolder;
				}
			}
		}
	}
}

