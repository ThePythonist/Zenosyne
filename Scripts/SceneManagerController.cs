using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SceneManagerController : MonoBehaviour
{
	public SaveLoadManager saveLoad;

	[HideInInspector]
	public bool isSceneChanging = false;


	void Start ()
	{
		saveLoad.LoadCheckpoint ();
	}



	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			RestartFromCheckpoint ();
		}
	}

	public void Restart ()
	{
		saveLoad.ClearCheckpoint (SceneManager.GetActiveScene ().buildIndex);
		RestartFromCheckpoint ();
	}

	public void RestartFromCheckpoint ()
	{
		isSceneChanging = true;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}


	public void GoToMainMenu ()
	{
		isSceneChanging = true;
		SceneManager.LoadScene (0);
	}


}
