using UnityEngine;

public class Key : Process
{

	[HideInInspector]
	public bool inLock = false;
	[HideInInspector]
	public Color inLockColor;

	public Color[] validOnLockColors;
	Renderer myRenderer;

	public float colorChangeDelay = 1f;

	float age = 0;

	void Start ()
	{
		myRenderer = GetComponent<Renderer> ();
	}

	[HideInInspector]
	public Color currentColor;

	void Update ()
	{
		Color materialColor = myRenderer.material.GetColor ("_Tint");
		Color colorToUse = materialColor;
		if (inLock) {
			colorToUse = inLockColor;
		} else {
			if (validOnLockColors.Length == 0) {
				colorToUse = Color.white;
			} else {
				float period = validOnLockColors.Length * colorChangeDelay;
				float ageInPeriod = age % period;
				int colorIndex = Mathf.FloorToInt (ageInPeriod / colorChangeDelay);
				colorToUse = validOnLockColors [colorIndex];
			}
		}

		if (colorToUse != materialColor) {
			myRenderer.material.SetColor ("_Tint", colorToUse);
		}
		currentColor = colorToUse;
		age += myDeltaTime;
		
	}
}
