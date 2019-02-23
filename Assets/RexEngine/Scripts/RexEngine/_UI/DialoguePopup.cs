/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DialoguePopup:MonoBehaviour 
	{
		public List<DialogueManager.Page> pages = new List<DialogueManager.Page>();
		public bool willAutoShow = false;

		void Awake() 
		{

		}

		void Start() 
		{
			if(willAutoShow)
			{
				ShowDialogue();
			}
		}

		public void ShowDialogue()
		{
			GetComponent<BoxCollider2D>().enabled = false;
			DialogueManager.Instance.Show(pages);
		}

		protected void OnTriggerEnter2D(Collider2D col)
		{
			if(col.tag == "Player" && col.gameObject.GetComponent<RexActor>() != null &&  col.gameObject.GetComponent<RexActor>() == GameManager.Instance.players[0])
			{
				ShowDialogue();
			}
		}
	}

}
