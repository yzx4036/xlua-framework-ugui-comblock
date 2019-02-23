/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_6_LevelScript:MonoBehaviour 
{
	public GameObject dialoguePopup;

	void Start() 
	{
		if(DataManager.Instance.hasVisitedBonusRoom_6)
		{
			dialoguePopup.SetActive(false);
		}

		DataManager.Instance.hasVisitedBonusRoom_6 = true;
	}
}
