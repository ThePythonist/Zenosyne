using UnityEngine;

public class Lock : MonoBehaviour
{

	Door door;

	public Color lockColor = Color.red;

	public bool locked = true;

	void Start ()
	{
		door = transform.parent.GetComponent<Door> ();
		GetComponent<Renderer> ().material.SetColor ("_LockColor", lockColor);

	}

	void OnTriggerEnter (Collider other)
	{
		if (locked) {
			Key key = other.gameObject.GetComponent<Key> ();
			if (key != null) {
				Color[] validOnLockColors = key.validOnLockColors;
				bool valid = false;
				foreach (Color i in validOnLockColors) {
					if (i.r == lockColor.r && i.g == lockColor.g && i.b == lockColor.b) {
						valid = true;
					}
					break;
				}
				if (validOnLockColors.Length == 0) {
					valid = true;
				}
				if (valid) {
					key.inLock = true;
					key.inLockColor = lockColor;
					GameObject unlocked = door.unlockedCube;
					GameObject newBlock = (GameObject)Instantiate (unlocked, transform.position, transform.rotation, transform.parent);
					Lock newBlockLock = newBlock.GetComponent<Lock> ();
					newBlockLock.lockColor = lockColor;

					Vector3 keyToLock = (transform.position - other.transform.position).normalized;
					float fromLeft = (new Vector3 (1, 0, 0) - keyToLock).magnitude;
					float fromRight = (new Vector3 (-1, 0, 0) - keyToLock).magnitude;
					float fromBack = (new Vector3 (0, 0, 1) - keyToLock).magnitude;
					float fromFront = (new Vector3 (0, 0, -1) - keyToLock).magnitude;

					float smallestError = Mathf.Min (new float[] { fromLeft, fromRight, fromBack, fromFront });
					Vector3 offset;
					Vector3 rotation = Vector3.zero;

					Mesh keyMesh = other.gameObject.GetComponent<MeshFilter> ().mesh;

					Vector3 keyExtents = keyMesh.bounds.extents;
					keyExtents.Scale (other.transform.lossyScale);

					Vector3 keyCenter = keyMesh.bounds.center;
					keyCenter.Scale (other.transform.lossyScale);

					Vector3 centerToUse = keyCenter;

					float sinkDepth = 0.5f;
					if (smallestError == fromLeft) {
						offset = new Vector3 (-0.5f * transform.lossyScale.x - keyExtents.z + sinkDepth, 0, 0);
						centerToUse = new Vector3 (keyCenter.z, keyCenter.y, -keyCenter.x);
						rotation = new Vector3 (0, 90, 0);
					} else if (smallestError == fromRight) {
						offset = new Vector3 (0.5f * transform.lossyScale.x + keyExtents.z - sinkDepth, 0, 0);
						centerToUse = new Vector3 (-keyCenter.z, keyCenter.y, keyCenter.x);
						rotation = new Vector3 (0, 270, 0);
					} else if (smallestError == fromBack) {
						offset = new Vector3 (0, 0, -0.5f * transform.lossyScale.z - keyExtents.z + sinkDepth);
					} else {
						offset = new Vector3 (0, 0, 0.5f * transform.lossyScale.z + keyExtents.z - sinkDepth);
						centerToUse = new Vector3 (keyCenter.x, keyCenter.y, -keyCenter.z);
						rotation = new Vector3 (0, 180, 0);
					}



					other.transform.position = transform.position - centerToUse + offset;
					other.transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles + rotation);
					other.transform.parent = transform.parent;

					Clickable keyClickable = other.GetComponent<Clickable> ();
					if (keyClickable.player != null) {
						if (keyClickable.player.isHolding (other.transform)) {
							keyClickable.player.LetGo ();
						}
					}

					keyClickable.pickupable = false;

					other.GetComponent<Rigidbody> ().isKinematic = true;

					Destroy (gameObject);
				}
			}
		}
	}
}
