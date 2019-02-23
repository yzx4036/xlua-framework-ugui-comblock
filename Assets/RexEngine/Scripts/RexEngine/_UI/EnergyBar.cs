/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyBar:MonoBehaviour 
{
	public enum Orientation
	{
		Horizontal,
		Vertical
	}

	public Orientation orientation;

	[System.Serializable]
	public class Sprites
	{
		public SpriteRenderer main;
		public SpriteRenderer backing;
		public SpriteRenderer end;
		public SpriteRenderer frame;
		public SpriteRenderer red;
	}

	public Sprites sprites;
	public AudioClip lowHealthSound;
	public bool willFlashBarOnLow;
	public bool willResizeWithValueChange = true; //If false; unitsPerPoint is not used
	public float unitsPerPoint = 0.2f;
	public TextMesh numberDisplay;

	protected int maxValue = 100;
	protected int currentValue;
	protected float originalSpriteSize;
	protected float originalBackingSize;

	protected float fullScale;
	protected float previousPercent;

	protected bool isBarFlashing;
	protected bool isEnergyScaling;
	protected float newEnergyScale;
	protected bool isRedScaling;
	protected float newRedScale;
	protected bool isBackingScaling;
	protected float newBackingScale;

	void Awake()
	{
		originalSpriteSize = (orientation == Orientation.Horizontal) ? sprites.main.bounds.size.x : sprites.main.bounds.size.y;
		originalBackingSize = (orientation == Orientation.Horizontal) ? sprites.backing.bounds.size.x : sprites.backing.bounds.size.y;
	}

	void Update()
	{
		if(isEnergyScaling)
		{
			sprites.main.transform.localScale = Vector3.Lerp(sprites.main.transform.localScale, new Vector3(newEnergyScale, sprites.main.transform.localScale.y, 1.0f), 0.25f);
		}

		if(isRedScaling)
		{
			sprites.red.transform.localScale = Vector3.Lerp(sprites.red.transform.localScale, new Vector3(newRedScale, sprites.red.transform.localScale.y, 1.0f), 0.25f);
		}

		if(isBackingScaling)
		{
			sprites.backing.transform.localScale = Vector3.Lerp(sprites.backing.transform.localScale, new Vector3(newBackingScale, sprites.backing.transform.localScale.y, 1.0f), 0.25f);
		}
	}

	void LateUpdate()
	{
		sprites.end.transform.position = new Vector3(sprites.backing.bounds.max.x - 0.01f, sprites.end.transform.position.y, sprites.end.transform.position.z);
		if(sprites.main.transform.localScale.x > sprites.backing.transform.localScale.x)
		{
			sprites.main.transform.localScale = new Vector3(sprites.backing.transform.localScale.x, sprites.main.transform.localScale.y, sprites.main.transform.localScale.z);
		}
	}

	protected IEnumerator FlashCoroutine()
	{
		while(isBarFlashing)
		{
			sprites.main.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			yield return new WaitForSeconds(0.1f);

			sprites.main.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			yield return new WaitForSeconds(0.1f);
		}
	}

	protected void FlashBar()
	{
		isBarFlashing = true;
		StartCoroutine("FlashCoroutine");
	}

	protected void StopFlash()
	{
		isBarFlashing = false;
		StopCoroutine("FlashCoroutine");
		sprites.main.color = new Color(1.0f, 1.0f, 1.0f, sprites.main.color.a);
	}

	public void SetMaxValue(int _maxValue, bool willAnimateTransition = true, bool willAnimateBarIncrease = false)
	{
		if(_maxValue > currentValue)
		{
			previousPercent = (float)currentValue / (float)maxValue;
		}

		maxValue = _maxValue;

		if(willResizeWithValueChange)
		{
			SetBackingSize(willAnimateBarIncrease);
		}

		float percent = (float)currentValue / (float)maxValue;

		UpdateDisplay(percent, false, willAnimateTransition, willAnimateBarIncrease);
	}

	public void SetValue(int _value, bool willAnimateTransition = true)
	{
		if(_value > currentValue)
		{
			previousPercent = (float)currentValue / (float)maxValue;
		}

		currentValue = _value;
		float percent = (float)currentValue / (float)maxValue;

		UpdateDisplay(percent, true, willAnimateTransition);
	}

	protected void SetBackingSize(bool willAnimateBarIncrease = false)
	{
		float totalLength = unitsPerPoint * maxValue;
		float newScale = totalLength / originalBackingSize;

		if(willAnimateBarIncrease)
		{
			AnimateBackingScale(newScale);
		}
		else
		{
			StopBackingScale();
			sprites.backing.transform.localScale = new Vector3(newScale, 1.0f, 1.0f);
		}
	}

	protected void AnimateEnergyScale(float newScale)
	{
		newEnergyScale = newScale;
		StartCoroutine("ScaleEnergyCoroutine");
	}

	protected void StopEnergyScale()
	{
		isEnergyScaling = false;
		StopCoroutine("ScaleEnergyCoroutine");
	}

	protected void AnimateRedScale(float newScale)
	{
		newRedScale = newScale;

		if(gameObject.activeSelf)
		{
			StartCoroutine("ScaleRedCoroutine");
		}
	}

	protected void StopRedScale()
	{
		isRedScaling = false;
		StopCoroutine("ScaleRedCoroutine");
	}

	protected void AnimateBackingScale(float newScale)
	{
		newBackingScale = newScale;
		StartCoroutine("ScaleBackingCoroutine");
	}

	protected void StopBackingScale()
	{
		isBackingScaling = false;
		StopCoroutine("ScaleBackingCoroutine");
	}

	protected IEnumerator ScaleEnergyCoroutine()
	{
		isEnergyScaling = true;

		yield return new WaitForSeconds(0.25f);

		sprites.main.transform.localScale = new Vector3(newEnergyScale, sprites.main.transform.localScale.y, 1.0f);
		isEnergyScaling = false;
	}

	protected IEnumerator ScaleRedCoroutine()
	{
		yield return new WaitForSeconds(1.25f);

		isRedScaling = true;

		yield return new WaitForSeconds(0.25f);

		sprites.red.transform.localScale = new Vector3(newRedScale, sprites.red.transform.localScale.y, 1.0f);
		isRedScaling = false;
	}

	protected IEnumerator ScaleBackingCoroutine()
	{
		yield return new WaitForSeconds(1.25f);

		isBackingScaling = true;

		yield return new WaitForSeconds(0.25f);

		sprites.backing.transform.localScale = new Vector3(newBackingScale, sprites.backing.transform.localScale.y, 1.0f);
		isBackingScaling = false;
	}

	//isCurrentValueChanging helps us determine if the current health amount is changing, or the OVERALL size of the bar is changing
	protected void UpdateDisplay(float _percent, bool isCurrentValueChanging, bool willAnimateTransition, bool willAnimateBarIncrease = false)
	{
		float newScale = sprites.backing.transform.localScale.x * _percent;
		sprites.red.transform.localScale = new Vector3(sprites.backing.transform.localScale.x * previousPercent, 1, 1);

		sprites.end.transform.position = new Vector3(sprites.backing.bounds.max.x - 0.01f, sprites.end.transform.position.y, sprites.end.transform.position.z);

		//Debug.Log("Percent is: " + _percent + "   Previous is: " + previousPercent);
		if(_percent < previousPercent && isCurrentValueChanging && willAnimateTransition)
		{
			StopEnergyScale();
			sprites.main.transform.localScale = new Vector3(newScale, 1.0f, 1.0f);
			AnimateRedScale(newScale);
		}
		else if(_percent > previousPercent && isCurrentValueChanging && willAnimateTransition)
		{
			AnimateEnergyScale(newScale);
		}
		else
		{
			if(willAnimateBarIncrease)
			{
				float totalLength = unitsPerPoint * maxValue;
				newScale = totalLength / originalBackingSize;

				AnimateEnergyScale(newScale);
			}
			else
			{
				sprites.main.transform.localScale = new Vector3(newScale, 1.0f, 1.0f);
			}
		}

		if(_percent <= 0.25f && previousPercent > 0.25f)
		{
			if(willFlashBarOnLow)
			{
				FlashBar();
			}

			if(lowHealthSound != null)
			{
				GetComponent<AudioSource>().PlayOneShot(lowHealthSound);
			}
		}
		else if(_percent > 0.25f && previousPercent <= 0.25f)
		{
			StopFlash();
		}

		if(!willAnimateTransition)
		{
			sprites.red.transform.localScale = sprites.main.transform.localScale;
		}

		previousPercent = _percent;

		if(numberDisplay != null)
		{
			numberDisplay.text = currentValue.ToString();
		}
	}
}
