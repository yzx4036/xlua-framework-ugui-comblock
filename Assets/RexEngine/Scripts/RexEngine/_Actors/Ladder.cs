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
	public class Ladder:MonoBehaviour
	{
		public int tiles = 5;
		public Sprites sprites;

		[System.Serializable]
		public class Sprites
		{
			public GameObject topSprite;
			public GameObject middleSprite;
			public GameObject bottomSprite;
		}

		private int previousTiles;

		void Awake() 
		{
			gameObject.tag = "Ladder";
			gameObject.layer = LayerMask.NameToLayer("PassThroughBottom");
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
					BoxCollider2D col = GetComponent<BoxCollider2D>();
					col.size = new Vector2(GlobalValues.tileSize, tiles * GlobalValues.tileSize);

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

			float tileTop = (GetComponent<BoxCollider2D>().size.y * 0.5f * 1.0f) - (GlobalValues.tileSize * 0.5f);
			for(int i = 0; i < tiles; i ++)
			{
				GameObject prefab = sprites.middleSprite;
				if(i == 0 && sprites.topSprite != null)
				{
					prefab = sprites.topSprite;
				}
				else if(i == tiles - 1 && sprites.bottomSprite != null)
				{
					prefab = sprites.bottomSprite;	
				}

				GameObject ladderSegment = Instantiate(prefab) as GameObject;
				ladderSegment.transform.parent = transform;
				ladderSegment.transform.localPosition = new Vector3(0.0f, tileTop - (i * GlobalValues.tileSize), 0.0f);
			}
		}
	}
}

