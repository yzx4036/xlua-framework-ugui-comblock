/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy:MonoBehaviour 
{
	public int current = 1;
	public int max = 1;
	public EnergyBar barPrefab;

	[HideInInspector]
	public int previous = 1;

	[HideInInspector]
	public EnergyBar bar;

	void Awake()
	{
		CreateBar();
	}

	public void Restore(int amount)
	{
		previous = current;
		current += amount;
		if(current > max)
		{
			current = max;
		}

		if(bar != null)
		{
			bar.SetValue(current);
		}
	}

	public void Decrement(int amount)
	{
		previous = current;
		current -= amount;
		if(current <= 0)
		{
			current = 0;
		}

		if(bar != null)
		{
			bar.SetValue(current);
		}
	}

	public void SetToMax()
	{
		current = max;

		if(bar != null)
		{
			bar.SetValue(current, false);
		}
	}

	public void SetValue(int value)
	{
		current = value;

		if(bar != null)
		{
			bar.SetValue(current, false);
		}
	}

	protected void CreateBar()
	{
		if(barPrefab != null)
		{
			GameObject newObject = Instantiate(barPrefab).gameObject;
			newObject.name = newObject.name.Split('(')[0];
			DontDestroyOnLoad(newObject);
			ParentHelper.Parent(newObject, ParentHelper.ParentObject.UI);
			bar = newObject.GetComponent<EnergyBar>();

			bar.SetMaxValue(max, false);
			bar.SetValue(current, false);
		}
	}
}
