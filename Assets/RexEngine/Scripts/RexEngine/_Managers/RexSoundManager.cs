/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class RexSoundManager:MonoBehaviour 
{
	public bool isMuteKeyEnabled;

	protected bool isMuted;
	protected AudioSource musicAudio;
	protected AudioClip currentTrack;
	protected float preMuteVolume = 1.0f;
	protected FadeType fadeType = FadeType.None;
	protected float fadeAmount = 0.0065f;
	protected float fadeMultiplier = 1.0f;
	protected float volume = 1.0f;

	public enum FadeType
	{
		None,
		In,
		Out
	}

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}

		musicAudio = GetComponent<AudioSource>();
		musicAudio.playOnAwake = false;
		musicAudio.rolloffMode = AudioRolloffMode.Linear;
		gameObject.name = "RexSound";
	}

	void Update()
	{
		if(fadeType != FadeType.None)
		{
			HandleFade();
		}

		if(isMuteKeyEnabled)
		{
			if(Input.GetKeyDown(KeyCode.M))
			{
				if(isMuted)
				{
					Unmute();
				}
				else
				{
					Mute();
				}
			}
		}

		float muteMultiplier = (isMuted) ? 0.0f : 1.0f;
		musicAudio.volume = fadeMultiplier * muteMultiplier * volume;
	}

	//Typically, use this to play music that's connected through multiple scenes, since it won't change anything if the right track is already playing
	public void SetMusic(AudioClip _track, bool willLoop = true, float _volume = 1.0f)
	{
		bool isTrackAlreadyPlaying = (currentTrack && currentTrack == _track);
		if(!isTrackAlreadyPlaying || !musicAudio.isPlaying)
		{
			Play(_track, willLoop, _volume);
		}
	}

	public void Play(AudioClip _track, bool willLoop = true, float _volume = 1.0f)
	{
		currentTrack = _track;
		musicAudio.loop = willLoop;
		fadeMultiplier = 1.0f;
		volume = _volume;
		musicAudio.clip = _track;

		bool isDebugMuteSet = false;

		#if UNITY_EDITOR
		if(EditorPrefs.GetBool("IsMusicMuted"))
		{
			isDebugMuteSet = true;
			Mute();
		}
		#endif

		if(_track != null && !isDebugMuteSet)
		{
			if(!musicAudio.isPlaying) //If this track was already set, but was paused, then we start it over from the beginning
			{
				musicAudio.time = 0.0f;
			}

			musicAudio.Play();
		}
		else
		{
			musicAudio.Stop();
		}
	}

	public void Fade()
	{
		fadeType = FadeType.Out;
	}

	public void FadeIn()
	{
		fadeType = FadeType.In;
	}

	public void FadeInAfterDuration(float _duration)
	{
		Invoke("FadeIn", _duration);
	}

	public void Mute()
	{
		isMuted = true;
	}

	public void Unmute()
	{
		isMuted = false;
	}

	public void Pause()
	{
		musicAudio.Pause();
	}

	public void SetLoop(bool _willLoop)
	{
		musicAudio.loop = _willLoop;
	}

	protected void HandleFade()
	{
		if(fadeType == FadeType.In)
		{
			fadeMultiplier += fadeAmount;
			if(fadeMultiplier >= 1.0f)
			{
				fadeMultiplier = 1.0f;
				fadeType = FadeType.None;
			}
		}
		else if(fadeType == FadeType.Out)
		{
			fadeMultiplier -= fadeAmount;
			if(fadeMultiplier <= 0.0f)
			{
				fadeMultiplier = 0.0f;
				fadeType = FadeType.None;
			}
		}
	}

	private static RexSoundManager instance = null;
	public static RexSoundManager Instance 
	{ 
		get 
		{
			if(instance == null)
			{
				GameObject go = new GameObject();
				instance = go.AddComponent<RexSoundManager>();
				go.name = "RexSoundManager";
			}
			
			return instance; 
		} 
	}
}
