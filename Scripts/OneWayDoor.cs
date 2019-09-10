using UnityEngine;
using System.Collections.Generic;

public class OneWayDoor : MonoBehaviour
{
	public static List<OneWayDoor> instances = new List<OneWayDoor> ();
	public Material openMat;
	public Material closedMat;

	public Narration narration;
	public bool playOnFirstEnter = false;
	public bool playOnRestart = false;

	[HideInInspector]
	public bool loaded = false;

	void Awake ()
	{
		Collider[] colliders = GetComponents<Collider> ();
		foreach (Collider col in colliders) {
			col.enabled = col.isTrigger;
		}
		GetComponent<Renderer> ().material = openMat;
		instances.Add (this);
	}


	void OnTriggerExit (Collider other)
	{
		if (other.tag.Equals ("Player")) {
			if (playOnFirstEnter) {
				GameObject.FindGameObjectWithTag ("GameController").GetComponent<Narrator> ().Play (narration);
			}
			Close ();
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<SaveLoadManager> ().SaveCheckpoint (other.transform, transform);
		}
	}

	public void Close ()
	{
		Collider[] colliders = GetComponents<Collider> ();
		foreach (Collider col in colliders) {
			col.enabled = !col.isTrigger;
		}
		GetComponent<Renderer> ().material = closedMat;
	}

	void OnDestroy ()
	{
		instances.Remove (this);
	}
}
