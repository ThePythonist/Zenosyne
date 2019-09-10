using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Headache : MonoBehaviour
{

	public PostProcessVolume volume;
	public new Camera camera;
	public AnimationCurve curve;
	ChromaticAberration ab;
	public float effectDuration = 3f;
	public float maxFOV = 10f;
	public AudioSource audioSource;
	float baseVolume;
	float time = 0;
	float baseFOV;

	bool doEffect = true;
	bool checkedYet = false;

	void Start ()
	{
		ab = volume.profile.GetSetting<ChromaticAberration> ();
		baseFOV = camera.fieldOfView;
		baseVolume = audioSource.volume;

	}

	void Update ()
	{
		if (!checkedYet) {
			checkedYet = true;
			foreach (OneWayDoor door in OneWayDoor.instances) {
				if (door.loaded) {
					doEffect = false;
					break;
				}
			}
		}
		if (doEffect) {
			if (time < effectDuration) {
				if (!audioSource.isPlaying) {
					audioSource.Play ();
				}
				float mag = curve.Evaluate (time / effectDuration);
				ab.intensity.Override (mag);
				camera.fieldOfView = Mathf.Lerp (baseFOV, maxFOV, mag);
				audioSource.volume = Mathf.Lerp (0, baseVolume, mag);
			} else {
				doEffect = false;
				if (audioSource.isPlaying) {
					audioSource.Stop ();
				}
				ab.intensity.Override (0);
				camera.fieldOfView = baseFOV;
			}
		}
		time += Time.deltaTime;
	}


}
