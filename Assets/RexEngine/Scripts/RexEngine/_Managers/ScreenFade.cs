/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

public class ScreenFade:MonoBehaviour 
{
	[HideInInspector]
	public float currentFadeDuration = 0.25f;

	private float textureAlpha = 1.0f;
	private Texture2D fadeTexture;
	private FadeType fadeType = FadeType.None;
	private Color fadeColor = Color.black;
	private float targetTextureAlpha;
	
	public enum FadeType
	{
		In,
		Out,
		None
	}
	
	public enum FadeDuration
	{
		Immediate = 0,
		Short = 10,
		Medium = 25,
		Long = 65,
		Dramatic = 150
	}
	
	private static ScreenFade instance = null;
	public static ScreenFade Instance 
	{ 
		get 
		{
			if(instance == null)
			{
				GameObject go = new GameObject();
				instance = go.AddComponent<ScreenFade>();
				go.name = "ScreenFade";
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

	void Update()
	{
		if(fadeType != FadeType.None)
		{
			textureAlpha = Mathf.Lerp(textureAlpha, targetTextureAlpha, currentFadeDuration); 

			if((textureAlpha >= 1.0f && fadeType == FadeType.Out) || (textureAlpha <= 0.0f && fadeType == FadeType.In))
			{
				if(textureAlpha > 1.0f)
				{
					textureAlpha = 1.0f;
				}
				else if(textureAlpha < 0.0f)
				{
					textureAlpha = 0.0f;
				}

				OnFadeComplete();
			}
		}
	}

	//if willStartFreshFade is true, a fade out will initialize the screen fade to clear, and then fade it to opaque, and vice versa; thus, if the screen is already black, a fade out will revert the fade to clear and fade to black from there
	//if willStartFreshFade is false, a fade out will start the fade at wherever the fade was previously -- so if it's a black screen, it'll keep it at a black screen
	public void Fade(FadeType _fadeType, FadeDuration _duration = FadeDuration.Short, Color _fadeColor = default(Color), bool willStartFreshFade = true)
	{
		fadeType = _fadeType;
		
		targetTextureAlpha = 0.0f;
		float duration = (float)_duration * 0.01f;
		currentFadeDuration = duration;

		if(!object.Equals(_fadeColor, default(Color)))
		{
			fadeColor = _fadeColor;
		}
		
		CreateFadeTexture(fadeColor);
		
		if(fadeType == FadeType.In)
		{
			if(willStartFreshFade)
			{
				textureAlpha = 1.0f;
			}

			targetTextureAlpha = 0.0f;
			
		}
		else if(fadeType == FadeType.Out)
		{
			if(willStartFreshFade)
			{
				textureAlpha = 0.0f;
				if(_duration == FadeDuration.Immediate)
				{
					textureAlpha = 1.0f;
				}
			}

			targetTextureAlpha = 1.0f;
		}
	}
	
	void OnGUI() 
	{
		if(fadeType != FadeType.None)
		{	
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, textureAlpha);
			GUI.DrawTexture(new Rect(0, 0, Screen.width * 1.5f, Screen.height * 1.5f), fadeTexture);
		}
	}
	
	private void OnFadeComplete()
	{
		if(fadeType == FadeType.In)
		{
			fadeTexture = null;
			fadeType = FadeType.None;
		}
	}
	
	private void CreateFadeTexture(Color textureColor = default(Color))
	{
		fadeTexture = new Texture2D(1, 1);
		fadeTexture.SetPixel(0, 0, fadeColor);
		fadeTexture.Apply();
	}
}
