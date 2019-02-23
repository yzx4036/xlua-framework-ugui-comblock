using UnityEngine;
using System.Collections;

public class FPSDisplay:MonoBehaviour
{
	public bool willDisplay;

	protected float updateInterval = 0.5F;
	protected float fpsAverage;
	protected float deltaTime = 0.0f;
	protected float fpsString;
	protected double lastInterval;
	protected int frames = 0;
	protected float fps;
	protected GUIStyle style;

	void Start() 
	{
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = Screen.height * 4 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);

		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}

	void Update()
	{
		if(!willDisplay)
		{
			return;
		}

		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

		frames ++;
		float timeSinceStartup = Time.realtimeSinceStartup;
		if(timeSinceStartup > lastInterval + updateInterval) 
		{
			fps = (float)(frames / (timeSinceStartup - lastInterval));
			lastInterval = timeSinceStartup;
			frames = 0;
		}

		fpsAverage += ((Time.deltaTime / Time.timeScale) - fpsAverage) * 0.03f;
		fpsString = (1.0f / fpsAverage); 
	}

	void OnGUI()
	{
		if(!willDisplay)
		{
			return;
		}

		GUILayout.Label(((int)fpsString).ToString());

	}
}