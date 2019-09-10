using UnityEngine;

public class FireBowl : MonoBehaviour
{

	void OnCollisionEnter (Collision collision)
	{
		Flammable flammable = collision.gameObject.GetComponent<Flammable> ();
		if (flammable != null) {
			flammable.SetOnFire (collision.contacts [0].point);
		}
	}

}
