 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace RexEngine
{
	public class RexButton:MonoBehaviour 
	{
		public enum Interaction
		{
			CallFunction,
			OpenURL
		}

		public AudioClip clickSound;
		public Interaction interaction;

		[HideInInspector]
		public string url;

		[HideInInspector]
		public MonoBehaviour callFunctionOnScript;

		[HideInInspector]
		public string methodName;

		[HideInInspector]
		public int methodIndex = 0;


		void OnMouseUp() 
		{
			DoAction();
			PlayClickSound();
		}

		protected void DoAction()
		{
			switch(interaction)
			{
				case Interaction.CallFunction:
					CallFunction();
					break;
				case Interaction.OpenURL:
					OpenURL();
					break;
			}
		}

		protected void CallFunction()
		{
			if(callFunctionOnScript && methodName != "")
			{
				callFunctionOnScript.Invoke(methodName, 0.0f);
			}
		}

		protected void OpenURL()
		{
			Application.OpenURL(url);
		}

		protected void PlayClickSound()
		{
			if(clickSound != null)
			{
				AudioSource audioSource = GetComponent<AudioSource>();
				if(audioSource)
				{
					audioSource.PlayOneShot(clickSound);
				}
			}
		}
	}
}
