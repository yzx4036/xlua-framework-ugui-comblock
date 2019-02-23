using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class ObjectDisablerManager:MonoBehaviour 
	{
		protected List<string> fullSessionDisabledObjects = new List<string>();
		protected List<string> singleLifeDisabledObjects = new List<string>();

		private static ObjectDisablerManager instance = null;
		public static ObjectDisablerManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<ObjectDisablerManager>();
					go.name = "ObjectDisablerManager";
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
		}

		public void MarkObjectForSingleLifeDisable(string _spawnString)
		{
			bool isAlreadyMarked = false;
			for(int i = 0; i < singleLifeDisabledObjects.Count; i ++)
			{
				if(singleLifeDisabledObjects[i] == _spawnString)
				{
					isAlreadyMarked = true;
					break;
				}
			}

			if(!isAlreadyMarked)
			{
				singleLifeDisabledObjects.Add(_spawnString);
			}
		}

		public void MarkObjectForFullSessionDisable(string _spawnString)
		{
			bool isAlreadyMarked = false;
			for(int i = 0; i < fullSessionDisabledObjects.Count; i ++)
			{
				if(fullSessionDisabledObjects[i] == _spawnString)
				{
					isAlreadyMarked = true;
					break;
				}
			}

			if(!isAlreadyMarked)
			{
				fullSessionDisabledObjects.Add(_spawnString);
			}
		}

		public bool GetIfObjectIsDisabled(string _spawnString)
		{
			return singleLifeDisabledObjects.Contains(_spawnString) || fullSessionDisabledObjects.Contains(_spawnString);
		}

		public void ResetSingleLifeDisabledObjects()
		{
			singleLifeDisabledObjects.Clear();
		}

		public void ResetFullSessionDisabledObjects()
		{
			fullSessionDisabledObjects.Clear();
		}

		public void ResetAll()
		{
			singleLifeDisabledObjects.Clear();
			fullSessionDisabledObjects.Clear();
		}
	}
}
