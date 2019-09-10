using UnityEngine;

public class Flammable : Process
{

	public bool onFire;
	public float timeLeftToBurn = 1f;
	public new GameObject particleSystem;
	public GameObject puffOfSmoke;
	public float flameScale = 1f;

	void Start ()
	{
		if (onFire) {
			onFire = false;
			SetOnFire (Vector3.zero);
		}
	}

	void Update ()
	{
		if (onFire) {
			timeLeftToBurn -= myDeltaTime;
			if (timeLeftToBurn <= 0) {
				if (puffOfSmoke != null) {
					GameObject puffOfSmokeObj = (GameObject)Instantiate (puffOfSmoke, transform.position, Quaternion.Euler (Vector3.zero));
					Destroy (puffOfSmokeObj, puffOfSmokeObj.GetComponent<AudioSource> ().clip.length);
				}
				Destroy (gameObject);
			}
		}
	}

	public void SetOnFire (Vector3 point)
	{
		if (!onFire) {
			onFire = true;
			GameObject psGO = (GameObject)GameObject.Instantiate (particleSystem);
			psGO.transform.parent = transform;
			psGO.transform.position = point;
			psGO.transform.localScale = flameScale * psGO.transform.lossyScale;
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if (onFire) {
			Flammable flammable = collision.gameObject.GetComponent<Flammable> ();
			if (flammable != null) {
				flammable.SetOnFire (collision.contacts [0].point);
			}
		}
	}

}
