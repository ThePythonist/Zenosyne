using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class Narrator : MonoBehaviour
{

	public Narration playOnLoad;
	Queue<Narration> queue = new Queue<Narration> ();
	AudioSource audioSource;
	Narration current = null;
	float clipStartTime;
	string captionText = "";
	public CaptionController captionController;

	void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void Play (Narration narration)
	{
		queue.Enqueue (narration);
	}

	public void ForcePlay (Narration narration)
	{
		Stop ();
		Play (narration);
	}

	public void PlayNext (Narration narration)
	{
		queue.Clear ();
		Play (narration);
	}

	public void Stop ()
	{
		queue.Clear ();
		audioSource.Stop ();
	}

	public void StopWhenFinishedCurrent ()
	{
		queue.Clear ();
	}

	void Update ()
	{
		string newCaptionText = "";
		if (!audioSource.isPlaying) {
			if (queue.Count > 0) {
				current = queue.Dequeue ();
				clipStartTime = Time.time;
				audioSource.clip = current.audio;
				audioSource.Play ();
			}
		} else {
			float currentDuration = Time.time - clipStartTime;
			foreach (TranscriptLine line in current.transcript) {
				if (line.startTime <= currentDuration && currentDuration <= line.endTime) {
					newCaptionText = line.text;
					break;
				}
			}
		}
		if (!newCaptionText.Equals (captionText)) {
			captionText = newCaptionText;
			if (captionText.Equals ("")) {
				captionController.gameObject.SetActive (false);
			} else {
				captionController.gameObject.SetActive (true);
				captionController.Show (captionText);
			}
		}
	}

}
