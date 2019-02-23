/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RexParticle is used in conjunction with RexPool; when spawned, this will play its animation and then auto-despawn itself
public class RexParticle:MonoBehaviour 
{
	public new AnimationClip animation; //If an animation is slotted, it will play it and despawn once it finishes

	public void Play()
	{
		gameObject.SetActive(true);
		StartCoroutine("PlayCoroutine");
	}

	protected IEnumerator PlayCoroutine()
	{
		float durationBeforeDespawn = 1.0f;
		if(animation)
		{
			GetComponent<Animator>().Play(animation.name, 0, 0.0f);
			durationBeforeDespawn = animation.length;
		}
		else //If no animation is slotted, check for an attached ParticleSystem and play that before despawning
		{
			ParticleSystem particleSystem = GetComponent<ParticleSystem>();
			if(particleSystem)
			{
				particleSystem.Play();
				durationBeforeDespawn = particleSystem.main.startLifetime.constant;
			}
		}

		yield return new WaitForSeconds(durationBeforeDespawn);

		gameObject.SetActive(false);
	}
}
