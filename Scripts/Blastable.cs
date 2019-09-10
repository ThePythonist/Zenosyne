using System.Collections.Generic;
using UnityEngine;

public class Blastable : MonoBehaviour
{

	public static List<Blastable> blastables = new List<Blastable> ();

	public bool pushable = true;
	public bool destroyable = true;

	void Awake ()
	{
		blastables.Add (this);
	}

	void OnDestroy ()
	{
		blastables.Remove (this);
	}

}
