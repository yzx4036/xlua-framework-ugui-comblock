using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class SwapPlayer:MonoBehaviour 
	{
		public enum StartType
		{
			OnCollision,
			OnSceneLoad,
			Manual
		}

		public StartType startType;
		public RexActor player;

		protected BoxCollider2D boxCollider;

		void Awake() 
		{
			boxCollider = GetComponent<BoxCollider2D>();
		}

		void Start() 
		{
			if(startType == StartType.OnSceneLoad)
			{
				Swap();
			}
		}

		public void Swap()
		{
			RexActor newPlayer = Instantiate(player).GetComponent<RexActor>();

			RexActor oldPlayer = GameManager.Instance.player;

			GameManager.Instance.player = newPlayer;
			GameManager.Instance.players[0] = newPlayer;
			RexSceneManager.Instance.player = newPlayer;

			newPlayer.SetPosition(new Vector2(oldPlayer.transform.position.x, oldPlayer.transform.position.y));

			Destroy(oldPlayer.gameObject);

			Camera.main.GetComponent<RexCamera>().SetFocusObject(newPlayer.transform);
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			if(startType == StartType.OnCollision && col.tag == "Player")
			{
				Swap();
				boxCollider.enabled = false;
			}
		}
	}
}
