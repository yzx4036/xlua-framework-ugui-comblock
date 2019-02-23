/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

public class ScreenFlash:MonoBehaviour 
{
	private float textureAlpha = 1.0f;
	private Texture2D flashTexture;
	private FlashType flashType = FlashType.None;
	private float timeElapsedSinceFlashStart = 0.0f;
	private float flashDuration = 0.0f;
	private float previousRealtimeSinceStartup;

	public enum FlashType
	{
		Hard,
		None
	}

	public enum FlashDuration
	{
		Short = 5,
		Medium = 15,
		Long = 30
	}
	
	private static ScreenFlash instance = null;
	public static ScreenFlash Instance 
	{ 
		get 
		{
			if(instance == null)
			{
				GameObject go = new GameObject();
				instance = go.AddComponent<ScreenFlash>();
				go.name = "ScreenFlash";
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

		DontDestroyOnLoad(gameObject);
		previousRealtimeSinceStartup = Time.realtimeSinceStartup;
	}

	void Update()
	{
		float realtimeSinceLastUpdate = Time.realtimeSinceStartup - previousRealtimeSinceStartup;

		if(flashType != FlashType.None)
		{
			timeElapsedSinceFlashStart += realtimeSinceLastUpdate;
			if(timeElapsedSinceFlashStart >= flashDuration)
			{
				OnFlashComplete();
			}
		}

		previousRealtimeSinceStartup = Time.realtimeSinceStartup;
	}
	
	public void Flash(FlashDuration _duration = FlashDuration.Short)
	{
		flashType = FlashType.Hard;

		timeElapsedSinceFlashStart = 0.0f;
		flashDuration = (float)_duration * 0.01f;;
		
		CreateFlashTexture();

		textureAlpha = 1.0f;
	}
	
	void OnGUI() 
	{
		if(flashType != FlashType.None)
		{
			GUI.color = new Color(255, 255, 255, textureAlpha);
			GUI.DrawTexture(new Rect(0, 0, Screen.width * 1.5f, Screen.height * 1.5f), flashTexture);
		}
	}
	
	private void OnFlashComplete()
	{
		textureAlpha = 0.0f;
		GUI.color = new Color(255, 255, 255, textureAlpha);
		flashType = FlashType.None;
	}
	
	private void CreateFlashTexture()
	{
		if(flashTexture == null)
		{
			flashTexture = new Texture2D(1, 1);
			flashTexture.SetPixel(0, 0, Color.white);
			flashTexture.Apply();
		}
	}
}
