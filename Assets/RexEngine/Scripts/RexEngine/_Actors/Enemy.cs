/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	public class Enemy:RexActor
	{
		public virtual void OnSpawned()
		{
			
		}

		protected override void OnDeath()
		{
			DropSpawner dropSpawner = GetComponent<DropSpawner>();
			if(dropSpawner != null)
			{
				dropSpawner.DropObject();
			}

			ObjectDisabler objectDisabler = GetComponent<ObjectDisabler>();
			if(objectDisabler != null)
			{
				objectDisabler.MarkForDisable();	
			}
		}

		void OnDestroy()
		{
			if(hp && hp.bar)
			{
				Destroy(hp.bar.gameObject);
			}
		}
	}
}
