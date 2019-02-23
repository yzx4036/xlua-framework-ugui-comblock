/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint:MonoBehaviour 
{
	public Demo_10_LevelScript levelScript;
	public AudioSource audioSource;
	public AudioClip collectSound;
	public Transform spriteRenderer;

	protected float rotateSpeed = 1.0f;

	void Awake() 
	{
		
	}

	void Start() 
	{
		
	}
	
	void Update() 
	{
		spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, spriteRenderer.transform.localEulerAngles.z + rotateSpeed);
	}

	protected void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			if(audioSource && collectSound)
			{
				audioSource.PlayOneShot(collectSound);
			}

			GetComponentInChildren<SpriteRenderer>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = false;
			levelScript.RunEnding();
		}
	}
}
