using UnityEngine;

public class LocklessDoor : Process
{

	Clickable clickable;

	public float minAngle = 0;
	public float maxAngle = 90;
	public float angle = 0;
	public float speed = 100;
	public Vector3 hinge = new Vector3 (-0.5f, 0.5f, 0);
	public Vector3 axis = new Vector3 (0, 1f, 0);
	AudioSource audioSource;

	Vector3 realHinge;

	public bool forward = false;

	void Start ()
	{
		clickable = GetComponent<Clickable> ();
		audioSource = GetComponent<AudioSource> ();
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		realHinge = hinge;
		realHinge.Scale (meshFilter.mesh.bounds.size);
		realHinge = transform.right * realHinge.x + transform.up * realHinge.y + transform.forward * realHinge.z;
		realHinge += transform.position;
	}

	void Update ()
	{
		if (clickable.IsClicked ()) {
			forward = !forward;
			audioSource.Play ();
		}

		float deltaAngle = speed * myDeltaTime;
		if (forward) {
			if (angle + deltaAngle > maxAngle) {
				deltaAngle = maxAngle - angle;
			}
		} else {
			deltaAngle *= -1;
			if (angle + deltaAngle < minAngle) {
				deltaAngle = minAngle - angle;
			}
		}

		transform.RotateAround (realHinge, axis, deltaAngle);
		angle += deltaAngle;
	}
}
