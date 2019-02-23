using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class DamageNumberManager:MonoBehaviour 
	{
		public RexPool spawnPool;

		private static DamageNumberManager instance = null;
		public static DamageNumberManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					Debug.Log("DamageNumberManager :: Instantiate");
					GameObject go = new GameObject();
					instance = go.AddComponent<DamageNumberManager>();
					go.name = "Damage Number Manager";
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

		void Start()
		{
			//InvokeRepeating("SpawnNumber", 2.5f, 2.5f);
		}

		public void ShowNumber(int damageAmount, Vector2 _position)
		{
			DamageNumber damageNumber = spawnPool.Spawn().GetComponent<DamageNumber>();
			damageNumber.Show(damageAmount, _position, spawnPool);
		}
	}
}
