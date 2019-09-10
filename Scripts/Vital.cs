using UnityEngine;

public class Vital : MonoBehaviour
{

	bool isQuitting = false;

	void OnApplicationQuit ()
	{
		isQuitting = true;
	}

	void OnDestroy ()
	{
		
		SceneManagerController smc = FindObjectOfType<SceneManagerController> ();
		if (smc != null) {
			if (!isQuitting && !smc.isSceneChanging) {
				smc.RestartFromCheckpoint ();
			}
		}
	}

}
