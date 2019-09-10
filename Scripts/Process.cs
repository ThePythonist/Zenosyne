using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public abstract class Process : MonoBehaviour
{

	public static List<Process> processes = new List<Process> ();

	public float timeRate = 1f;

	void Awake ()
	{
		processes.Add (this);
	}

	public float myDeltaTime {
		get {
			return timeRate * Time.deltaTime;
		}
	}

	public float myFixedDeltaTime {
		get {
			return timeRate * Time.fixedDeltaTime;
		}
	}

	public virtual void OnDestroy ()
	{
		processes.Remove (this);
	}
}
