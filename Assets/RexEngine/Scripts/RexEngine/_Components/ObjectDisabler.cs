using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ObjectDisabler:MonoBehaviour 
	{
		public enum DisableDuration
		{
			SingleLife,
			FullSession
		}

		public DisableDuration disableDuration;

		protected string spawnString;
		protected bool isDisabled;

		void Start() 
		{
			string sceneName = RexSceneManager.Instance.GetCurrentLevel();
			double spawnX = transform.position.x;
			double spawnY = transform.position.y;

			spawnString = sceneName + "_" + gameObject.name + "_" + spawnX.ToString("F2") + "_" + spawnY.ToString("F2");

			isDisabled = ObjectDisablerManager.Instance.GetIfObjectIsDisabled(spawnString);

			if(isDisabled)
			{
				gameObject.SetActive(false);
			}
		}

		public void MarkForDisable()
		{
			if(disableDuration == DisableDuration.SingleLife)
			{
				ObjectDisablerManager.Instance.MarkObjectForSingleLifeDisable(spawnString);
			}
			else if(disableDuration == DisableDuration.FullSession)
			{
				ObjectDisablerManager.Instance.MarkObjectForFullSessionDisable(spawnString);
			}
		}
	}
}