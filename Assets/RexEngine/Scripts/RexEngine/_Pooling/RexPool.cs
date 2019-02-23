/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RexPool:MonoBehaviour 
{
	public GameObject prefab; //The prefab that will be spawned
	public int startingPoolSize = 5; //The amount of the prefab spawned in Start

	protected List<GameObject> objects; //All the objects this Pool has spawned, both active and in reserve
	protected List<GameObject> activeObjects; //Objects currently active in the Pool

	void Awake() 
	{
		objects = new List<GameObject>();
		activeObjects = new List<GameObject>();
		for(int i = 0; i < startingPoolSize; i ++)
		{
			CreateObject();
		}
	}

	public GameObject Spawn()
	{
		for(int i = 0; i < objects.Count; i ++)
		{
			if(!objects[i].activeInHierarchy)
			{
				objects[i].SetActive(true);
				activeObjects.Add(objects[i]);

				return objects[i];
			}
		}

		GameObject obj = CreateObject();
		obj.SetActive(true);
		activeObjects.Add(obj);

		return obj;
	}

	public void Despawn(GameObject _object)
	{
		if(_object != null && activeObjects != null && activeObjects.Contains(_object))
		{
			_object.SetActive(false);
			activeObjects.Remove(_object);
		}
	}

	public int ActiveObjects()
	{
		return activeObjects.Count;
	}

	protected GameObject CreateObject()
	{
		GameObject obj = (GameObject)Instantiate(prefab);
		obj.SetActive(false);
		objects.Add(obj);
		obj.transform.parent = transform;

		return obj;
	}

	void OnDestroy()
	{
		prefab = null;
		objects = null;
		activeObjects = null;
	}
}
