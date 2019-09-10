using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	public SaveLoadManager saveLoad;

	public void Restart ()
	{
		int sceneIndex = SceneManager.GetActiveScene ().buildIndex + 1;
		saveLoad.ClearCheckpoint (sceneIndex);
		SceneManager.LoadScene (sceneIndex);
	}

	public void Play ()
	{
		int sceneIndex = saveLoad.GetCurrentSavedScene ();
		if (sceneIndex < 0) {
			Restart ();
		} else {
			SceneManager.LoadScene (sceneIndex);
		}
	}

	public void Quit ()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit ();
	}
}
