using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class AttackBase:MonoBehaviour 
	{
		public bool isEnabled = true;

		public void Enable()
		{
			isEnabled = true;
			OnEnabled();
		}

		public void Disable()
		{
			isEnabled = false;
			OnDisabled();
		}

		public virtual void OnEnabled(){}

		public virtual void OnDisabled(){}
	}
}
