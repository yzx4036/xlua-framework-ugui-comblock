/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RexEngine
{
	//Reflects Projectiles
	public class Reflector:MonoBehaviour 
	{
		public enum Orientation
		{
			Right,
			Left,
			Up,
			Down
		}

		public Orientation orientation;
		public bool willDamagePlayerOnReflect;
		public bool willDamageEnemiesOnReflect = true;

		//[HideInInspector]
		public RexActor actor;

		void Awake()
		{
			gameObject.tag = "Reflector";

			Attack attack = GetComponent<Attack>();
			if(attack)
			{
				actor = attack.slots.actor;
			}
		}
	}
}
