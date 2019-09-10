using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Clickable))]
[RequireComponent (typeof(MeshRenderer))]
public class TimeSphere : MonoBehaviour
{

	public static List<TimeSphere> timeSpheres = new List<TimeSphere> ();

	public float timeRate = 1;
	public float effectRadius = 10;

	[HideInInspector]
	public bool active = false;

	public Transform radiusSphere;

	Clickable clickable;

	void Awake ()
	{
		timeSpheres.Add (this);
	}

	void OnDestroy ()
	{
		timeSpheres.Remove (this);
	}

	void Start ()
	{
		clickable = GetComponent<Clickable> ();
		UpdateValues ();
	}

	void TransferColor (MeshRenderer myRenderer, MeshRenderer radiusRenderer, string colorName)
	{
		radiusRenderer.material.SetColor (colorName, myRenderer.material.GetColor (colorName));
	}

	void Update ()
	{
		if (clickable.IsClicked ()) {
			active = !active;
		}
		radiusSphere.gameObject.SetActive (active);
	}

	public bool CollidesWith (Process process)
	{
		Collider[] inside = Physics.OverlapSphere (transform.position, effectRadius);
		if (inside != null) {
			foreach (Collider collider in inside) {
				try {
					if (collider.gameObject == process.gameObject) {
						return true;
					}
				} catch (MissingReferenceException) {
				}
			}
		}
		return false;
	}

	public bool PlayerInside ()
	{
		Vector3 eyePos = Camera.main.transform.position;
		float dist = Vector3.Distance (transform.position, eyePos);
		return dist <= effectRadius;
	}

	void OnValidate ()
	{
		if (timeRate < 0) {
			timeRate = 0;
		}
	}

	public void UpdateValues ()
	{
		radiusSphere.localScale = new Vector3 (2 * effectRadius, 2 * effectRadius, 2 * effectRadius);
		MeshRenderer myRenderer = GetComponent<MeshRenderer> ();
		MeshRenderer radiusRenderer = radiusSphere.GetComponent<MeshRenderer> ();

		myRenderer.material.SetFloat ("_TimeRate", timeRate);
		radiusRenderer.material.SetFloat ("_TimeRate", timeRate);

		TransferColor (myRenderer, radiusRenderer, "_SlowColor");
		TransferColor (myRenderer, radiusRenderer, "_NeutralColor");
		TransferColor (myRenderer, radiusRenderer, "_FastColor");
		TransferColor (myRenderer, radiusRenderer, "_SlowGlowColor");
		TransferColor (myRenderer, radiusRenderer, "_FastGlowColor");

		radiusRenderer.material.SetFloat ("_MaxTimeRate", myRenderer.material.GetFloat ("_MaxTimeRate"));

		radiusSphere.gameObject.SetActive (active);
	}
}
