using UnityEngine;

public class Clickable : MonoBehaviour
{

	bool clicked = false;
	[HideInInspector]
	public PlayerController player;
	public bool pickupable;

	public bool IsClicked ()
	{
		bool isClicked = clicked;
		clicked = false;
		return isClicked;
	}

	public void SetClicked (bool clicked)
	{
		this.clicked = clicked;
	}
}
