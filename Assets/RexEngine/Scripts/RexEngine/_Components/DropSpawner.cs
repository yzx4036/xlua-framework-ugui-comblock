using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DropSpawner:MonoBehaviour 
	{
		[System.Serializable]
		public class ObjectToDrop
		{
			public string attackName;
			public GameObject prefabToDrop;
		}

		public GameObject objectToSpawn;
		public List<ObjectToDrop> objectsToDrop;

		public GameObject DropObject()
		{
			SetDropObject();

			GameObject dropObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity).gameObject;
			dropObject.transform.parent = transform.parent;

			RexObject rexObject = dropObject.GetComponent<RexObject>();
			if(rexObject != null)
			{
				rexObject.SetPosition(new Vector2(transform.position.x, transform.position.y));
			}

			RexPhysics rexPhysics = dropObject.GetComponent<RexPhysics>();
			if(rexPhysics != null)
			{
				rexPhysics.willSnapToFloorOnStart = false;
			}

			return dropObject;
		}

		protected void SetDropObject()
		{
			Attack[] attacks = GameManager.Instance.player.GetComponentsInChildren<Attack>();
			for(int i = 0; i < attacks.Length; i ++)
			{
				for(int j = 0; j < objectsToDrop.Count; j ++)
				{
					string attackName = attacks[i].name.Split('(')[0];
					if(objectsToDrop[j].attackName == attackName && attacks[i].isEnabled)
					{
						objectToSpawn = objectsToDrop[j].prefabToDrop;

						return;
					}
				}
			}
		}
	}
}
