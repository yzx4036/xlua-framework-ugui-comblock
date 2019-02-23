/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jitter:MonoBehaviour 
{
	public enum JitterStrength
	{
		Mild = 0,
		Medium = 1,
		Hot = 2
	}

	public GameObject objectToJitter;

	private Vector3 basePosition;
	private float distancePerJitter;

    void Awake() 
	{

	}
	
	void Start() 
	{
		basePosition = objectToJitter.transform.localPosition;
	}
	
	public void Play(int _numberOfJitters = 4, JitterStrength _strength = JitterStrength.Medium) //Pass -1 to _numberOfJitters to play forever
	{
		switch(_strength)
		{
			case JitterStrength.Mild:
				distancePerJitter = 0.05f;
				break;
			case JitterStrength.Medium:
				distancePerJitter = 0.2f;
				break;
			case JitterStrength.Hot:
				distancePerJitter = 0.75f;
				break;
		}

		objectToJitter.transform.localPosition = basePosition;

		StartCoroutine("JitterCoroutine");
	}

	public void Stop()
	{
		OnJitterComplete();
	}

	public void UpdateBasePosition()
	{
		basePosition = objectToJitter.transform.localPosition;
	}

	protected IEnumerator JitterCoroutine()
	{
		float durationPerJitter = 0.01f;
		int numberOfJitters = 4;

		while(numberOfJitters != 0)
		{
			objectToJitter.transform.localPosition = new Vector3(basePosition.x + distancePerJitter, basePosition.y, basePosition.z);
			yield return new WaitForSeconds(durationPerJitter);

			objectToJitter.transform.localPosition = new Vector3(basePosition.x - distancePerJitter, basePosition.y, basePosition.z);
			yield return new WaitForSeconds(durationPerJitter);

			objectToJitter.transform.localPosition = new Vector3(basePosition.x, basePosition.y + distancePerJitter, basePosition.z);
			yield return new WaitForSeconds(durationPerJitter);

			objectToJitter.transform.localPosition = new Vector3(basePosition.x, basePosition.y - distancePerJitter, basePosition.z);
			yield return new WaitForSeconds(durationPerJitter);

			numberOfJitters --;
		}

		OnJitterComplete();
	}

	private void OnJitterComplete()
	{
		StopCoroutine("JitterCoroutine");
		objectToJitter.transform.localPosition = basePosition;
	}

	protected void OnDestroy()
	{
		objectToJitter = null;
	}
}
