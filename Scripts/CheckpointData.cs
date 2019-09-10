using UnityEngine;

[System.Serializable]
public class CheckpointData
{

	float x = 0, y = 0, z = 0, rotX = 0, rotY = 0, rotZ = 0;
	public int sceneIndex;
	public bool checkpoint = true;
	public float checkpointX, checkpointY, checkpointZ;

	public CheckpointData (Transform player, Transform checkpoint, int sceneIndex)
	{
		x = player.position.x;
		y = player.position.y;
		z = player.position.z;

		rotX = player.eulerAngles.x;
		rotY = player.eulerAngles.y;
		rotZ = player.eulerAngles.z;

		if (checkpoint == null) {
			this.checkpoint = false;
		} else {
			checkpointX = checkpoint.position.x;
			checkpointY = checkpoint.position.y;
			checkpointZ = checkpoint.position.z;
		}

		this.sceneIndex = sceneIndex;
	}

	public CheckpointData (int sceneIndex)
	{
		checkpoint = false;
		this.sceneIndex = sceneIndex;
	}

	public Vector3 GetPlayerPosition ()
	{
		return new Vector3 (x, y, z);
	}

	public Quaternion GetPlayerRotation ()
	{
		return Quaternion.Euler (new Vector3 (rotX, rotY, rotZ));
	}

	public Vector3 GetCheckpoinPosition ()
	{
		return new Vector3 (checkpointX, checkpointY, checkpointZ);
	}

}
