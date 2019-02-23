/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParentHelper:MonoBehaviour 
{
	public enum ParentObject
	{
		Particles,
		Powerups,
		Projectiles,
		Actors,
		Singletons,
		Drops,
		UI
	}

	protected static GameObject particles;
	protected static GameObject powerups;
	protected static GameObject projectiles;
	protected static GameObject actors;
	protected static GameObject singletons;
	protected static GameObject drops;
	protected static GameObject ui;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Start() 
	{
		particles = GameObject.Find("Particles");
		powerups = GameObject.Find("Powerups");
		projectiles = GameObject.Find("Projectiles");
		actors = GameObject.Find("Actors");
		singletons = GameObject.Find("Singletons");
		drops = GameObject.Find("Drops");
		ui = GameObject.Find("UI");
	}
	
	public static void Parent(GameObject objectToParent, ParentObject parentObject)
	{
		GameObject objectToParentTo = actors;
		switch(parentObject)
		{
			case ParentObject.Particles:
				if(particles == null)
				{
					particles = GameObject.Find("Particles");
					if(particles == null)
					{
						particles = new GameObject();
						DontDestroyOnLoad(particles);
						particles.name = "Particles";
					}
				}
				objectToParentTo = particles;
				break;
			case ParentObject.Powerups:
				if(powerups == null)
				{
					powerups = GameObject.Find("Powerups");
					if(powerups == null)
					{
						powerups = new GameObject();
						DontDestroyOnLoad(powerups);
						powerups.name = "Powerups";
					}
				}
				objectToParentTo = powerups;
				break;
			case ParentObject.Projectiles:
				if(projectiles == null)
				{
					projectiles = GameObject.Find("Projectiles");
					if(projectiles == null)
					{
						projectiles = new GameObject();
						DontDestroyOnLoad(projectiles);
						projectiles.name = "Projectiles";
					}
				}
				objectToParentTo = projectiles;
				break;
			case ParentObject.Singletons:
				if(singletons == null)
				{
					singletons = GameObject.Find("Singletons");
					if(singletons == null)
					{
						singletons = new GameObject();
						DontDestroyOnLoad(singletons);
						singletons.name = "Singletons";
					}
				}
				objectToParentTo = singletons;
				break;
			case ParentObject.Actors:
				if(actors == null)
				{
					actors = GameObject.Find("Actors");
					if(actors == null)
					{
						actors = new GameObject();
						DontDestroyOnLoad(actors);
						actors.name = "Actors";
					}
				}
				objectToParentTo = actors;
				break;
			case ParentObject.Drops:
				if(drops == null)
				{
					drops = GameObject.Find("Drops");
					if(drops == null)
					{
						drops = new GameObject();
						DontDestroyOnLoad(drops);
						drops.name = "Drops";
					}
				}
				break;
			case ParentObject.UI:
				if(ui == null)
				{
					ui = GameObject.Find("UI");
					if(ui == null)
					{
						ui = new GameObject();
						DontDestroyOnLoad(ui);
						ui.name = "UI";
					}
				}
				objectToParentTo = ui;
				break;
			default:
				if(actors == null)
				{
					actors = GameObject.Find("Actors");
					if(actors == null)
					{
						actors = new GameObject();
						DontDestroyOnLoad(actors);
						actors.name = "Actors";
					}
				}
				objectToParentTo = actors;
				break;
		}

		objectToParent.transform.parent = objectToParentTo.transform;
	}
}
