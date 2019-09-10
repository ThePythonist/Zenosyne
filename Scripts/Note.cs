using UnityEngine;

public class Note : MonoBehaviour
{

	public Narration narration;
	Clickable clickable;

	void Start ()
	{
		clickable = GetComponent<Clickable> ();
	}

	void Update ()
	{
		if (clickable.IsClicked ()) {
			if (narration != null) {
				FindObjectOfType<Narrator> ().PlayNext (narration);
			}
		}
	}
}
