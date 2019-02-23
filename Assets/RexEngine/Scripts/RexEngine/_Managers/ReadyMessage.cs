/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyMessage:MonoBehaviour 
{
	public TextMesh textMesh;
	public List<string> messages;

	private static ReadyMessage instance = null;
	public static ReadyMessage Instance 
	{ 
		get 
		{
			if(instance == null)
			{
				GameObject go = new GameObject();
				instance = go.AddComponent<ReadyMessage>();
				go.name = "ReadyMessage";
				DontDestroyOnLoad(go);
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

	public void Show()
	{
		string message = "READY";
		if(messages.Count > 0)
		{
			int random = RexMath.RandomInt(0, messages.Count - 1);	
			message = messages[random];
		}

		textMesh.text = message;

		StartCoroutine("ShowCoroutine");
	}

	protected IEnumerator ShowCoroutine()
	{
		float longDelay = 1.5f;
		float shortDelay = 0.35f;

		textMesh.gameObject.SetActive(true);
		yield return new WaitForSeconds(longDelay);

		textMesh.gameObject.SetActive(false);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(true);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(false);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(true);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(false);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(true);
		yield return new WaitForSeconds(shortDelay);

		textMesh.gameObject.SetActive(false);
		yield return new WaitForSeconds(shortDelay);
	}
}
