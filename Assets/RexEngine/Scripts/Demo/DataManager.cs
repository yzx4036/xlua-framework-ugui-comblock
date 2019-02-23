/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager:MonoBehaviour 
{
	[System.NonSerialized]
	public bool hasVisitedBonusRoom_6;

	[System.NonSerialized]
	public bool hasUnlockedBounce;

	[System.NonSerialized]
	public bool hasUnlockedProjectile;

	[System.NonSerialized]
	public bool hasUnlockedDoubleJump;

	[System.NonSerialized]
	public bool hasUnlockedFly;

	[System.NonSerialized]
	public bool hasUnlockedWallCling;

	private static DataManager instance = null;
	public static DataManager Instance 
	{ 
		get 
		{
			if(instance == null)
			{
				GameObject go = new GameObject();
				instance = go.AddComponent<DataManager>();
				go.name = "DataManager";
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

		//DontDestroyOnLoad(gameObject);
	}

	public void Save()
	{
		PlayerPrefs.SetInt("HasVisitedBonusRoom_6", hasVisitedBonusRoom_6 ? 1 : 0);
		PlayerPrefs.SetInt("HasUnlockedBounce", hasUnlockedBounce ? 1 : 0);
		PlayerPrefs.SetInt("HasUnlockedProjectile", hasUnlockedProjectile ? 1 : 0);
		PlayerPrefs.SetInt("HasUnlockedDoubleJump", hasUnlockedDoubleJump ? 1 : 0);
		PlayerPrefs.SetInt("HasUnlockedFly", hasUnlockedFly ? 1 : 0);
		PlayerPrefs.SetInt("HasUnlockedWallCling", hasUnlockedWallCling ? 1 : 0);
	}

	public void Load()
	{
		hasVisitedBonusRoom_6 = (PlayerPrefs.GetInt("HasVisitedBonusRoom_6") == 1) ? true : false;
		hasUnlockedBounce = (PlayerPrefs.GetInt("HasUnlockedBounce") == 1) ? true : false;
		hasUnlockedProjectile = (PlayerPrefs.GetInt("HasUnlockedProjectile") == 1) ? true : false;
		hasUnlockedDoubleJump = (PlayerPrefs.GetInt("HasUnlockedDoubleJump") == 1) ? true : false;
		hasUnlockedFly = (PlayerPrefs.GetInt("HasUnlockedFly") == 1) ? true : false;
		hasUnlockedWallCling = (PlayerPrefs.GetInt("HasUnlockedWallCling") == 1) ? true : false;
	}

	public void ResetData()
	{
		hasVisitedBonusRoom_6 = false;
		hasUnlockedBounce = false;
		hasUnlockedProjectile = false;
		hasUnlockedDoubleJump = false;
		hasUnlockedFly = false;
		hasUnlockedWallCling = false;
		Save();
	}
}
