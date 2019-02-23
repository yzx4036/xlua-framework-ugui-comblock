/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RexEngine;

public class Demo_3_LevelScript:MonoBehaviour 
{
	public RexActor midboss;
	public float bridgeX;
	public float bridgeY;
	public float topRiseY;
	public float bossLerpTime = 0.05f;

	private bool hasEndSequencePlayed;
	private bool isRising;

	void Awake() 
	{
		
	}

	void Start() 
	{
		
	}
	
	void Update() 
	{
		if(midboss.isDead)
		{
			if(!hasEndSequencePlayed)
			{
				hasEndSequencePlayed = true;
				StartCoroutine("BuildBridgeCoroutine");
			}

			if(midboss.transform.position.x > bridgeX)
			{
				midboss.transform.position = Vector3.Lerp(midboss.transform.position, new Vector3(bridgeX, midboss.transform.position.y, midboss.transform.position.z), bossLerpTime);
			}

			if(isRising && midboss.transform.position.y < topRiseY)
			{
				midboss.transform.position = Vector3.MoveTowards(midboss.transform.position, new Vector3(midboss.transform.position.x, topRiseY, midboss.transform.position.z), 0.25f);
				if(midboss.transform.position.y >= topRiseY - 0.25f)
				{
					isRising = false;
					midboss.transform.position = new Vector3(midboss.transform.position.x, topRiseY, midboss.transform.position.z);
				}
			}
			else if(midboss.transform.position.y > bridgeY)
			{
				midboss.transform.position = Vector3.MoveTowards(midboss.transform.position, new Vector3(midboss.transform.position.x, bridgeY, midboss.transform.position.z), 0.25f);
			}

			if(midboss.transform.localEulerAngles.z < 90.0f)
			{
				midboss.transform.localEulerAngles = new Vector3(0.0f, 0.0f, midboss.transform.localEulerAngles.z + 1.75f);
				if(midboss.transform.localEulerAngles.z > 90.0f)
				{
					midboss.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
				}
			}
		}
	}

	private IEnumerator BuildBridgeCoroutine()
	{
		isRising = true;

		midboss.canBounceOn = false;
		midboss.GetComponent<ContactDamage>().enabled = false;
		midboss.GetComponent<BoxCollider2D>().enabled = true;
		midboss.gameObject.layer = LayerMask.NameToLayer("Terrain");

		yield return new WaitForSeconds(1.5f);

		ScreenShake.Instance.Shake();

		yield return new WaitForSeconds(0.25f);

		midboss.GetComponent<Jitter>().Play();
	}
}
