using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{

	public void SaveCheckpoint (Transform player, Transform checkpoint)
	{
		CheckpointData data = new CheckpointData (player, checkpoint, SceneManager.GetActiveScene ().buildIndex);
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/checkpointdata.dat", FileMode.OpenOrCreate);
		bf.Serialize (file, data);
		file.Close ();
	}

	public void LoadCheckpoint ()
	{
		if (File.Exists (Application.persistentDataPath + "/checkpointdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/checkpointdata.dat", FileMode.Open);
			CheckpointData data = (CheckpointData)bf.Deserialize (file);
			file.Close ();
			Narrator narrator = GameObject.FindGameObjectWithTag ("GameController").GetComponent<Narrator> ();
			foreach (OneWayDoor door in OneWayDoor.instances) {
				door.loaded = false;
			}
			if (data.checkpoint) {
				Transform player = GameObject.FindGameObjectWithTag ("Player").transform;
				player.position = data.GetPlayerPosition ();
				player.GetComponent<PlayerController> ().SetRotation (data.GetPlayerRotation ());
				foreach (OneWayDoor door in OneWayDoor.instances) {
					if (door.transform.position.Equals (data.GetCheckpoinPosition ())) {
						door.Close ();
						door.loaded = true;
						if (door.playOnRestart) {
							narrator.Play (door.narration);
						}
						break;
					} 
				}
			} else {
				if (narrator.playOnLoad != null) {
					narrator.Play (narrator.playOnLoad);
				}
			}
		}
	}


	public int GetCurrentSavedScene ()
	{
		try {
			if (File.Exists (Application.persistentDataPath + "/checkpointdata.dat")) {
				BinaryFormatter bf = new BinaryFormatter ();
				FileStream file = File.Open (Application.persistentDataPath + "/checkpointdata.dat", FileMode.Open);
				CheckpointData data = (CheckpointData)bf.Deserialize (file);
				file.Close ();

				return data.sceneIndex;
			}
		} catch {
		}
		return -1;
	}

	public void ClearCheckpoint (int sceneIndex)
	{
		CheckpointData data = new CheckpointData (sceneIndex);
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/checkpointdata.dat", FileMode.OpenOrCreate);
		bf.Serialize (file, data);
		file.Close ();
	}

}
