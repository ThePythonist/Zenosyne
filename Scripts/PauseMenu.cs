using UnityEngine;

public class PauseMenu : MonoBehaviour
{

	public GameObject mainPauseMenu;
	public GameObject settingsMenu;

	void OnEnable ()
	{
		mainPauseMenu.SetActive (true);
		settingsMenu.SetActive (false);
	}
}
