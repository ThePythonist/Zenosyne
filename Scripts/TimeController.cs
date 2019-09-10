using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{

	Texture2D tint;
	Color tintColor;
	bool useColor = false;

	void Start ()
	{
		tint = new Texture2D (1, 1);
		tint.SetPixel (0, 0, Color.white);
	}

	void Update ()
	{
		foreach (Process process in Process.processes) {
			process.timeRate = 1;
			foreach (TimeSphere timeSphere in TimeSphere.timeSpheres) {
				if (timeSphere.active && timeSphere.CollidesWith (process)) {
					process.timeRate = timeSphere.timeRate;
				}
			}
		}
		TimeSphere playerIn = null;
		foreach (TimeSphere timeSphere in TimeSphere.timeSpheres) {
			if (timeSphere.PlayerInside ()) {
				if (timeSphere.active) {
					playerIn = timeSphere;
				}
			}
		}
		if (playerIn != null) {
			MakeTint (playerIn);
			useColor = true;
		} else {
			useColor = false;
		}
	}

	Color LerpColors (Color a, Color b, float t)
	{
		return new Color (Mathf.Lerp (a.r, b.r, t), Mathf.Lerp (a.g, b.g, t), Mathf.Lerp (a.b, b.b, t), 0.6f);
	}

	void MakeTint (TimeSphere timeSphere)
	{
		tint = new Texture2D (1, 1);

		Material material = timeSphere.radiusSphere.GetComponent<MeshRenderer> ().material;
		Color slowColor = material.GetColor ("_SlowColor");
		Color neutralColor = material.GetColor ("_NeutralColor");
		Color fastColor = material.GetColor ("_FastColor");

		float maxTimeRate = material.GetFloat ("_MaxTimeRate");

		Color colorToUse;

		if (timeSphere.timeRate <= 1) {
			colorToUse = LerpColors (slowColor, neutralColor, timeSphere.timeRate);
		} else {
			colorToUse = LerpColors (neutralColor, fastColor, Mathf.InverseLerp (1, maxTimeRate, timeSphere.timeRate));
		}

		tintColor = colorToUse;
	}

	void OnGUI ()
	{
		
		if (useColor) {
			GUI.color = tintColor;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), tint);
		}
	}
}
