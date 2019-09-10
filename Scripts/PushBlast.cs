using UnityEngine;

public class PushBlast : Blast
{
	[HideInInspector]
	public float explosionForce = 10;

	[HideInInspector]
	public float delay;
	[HideInInspector]
	public GameObject destroyBlast;
	float age = 0;
	bool createdDestroyBlast = false;

	public override void Update ()
	{
		base.Update ();
		age += myDeltaTime;
		if (age >= delay && !createdDestroyBlast) {
			createdDestroyBlast = true;
			DestroyBlast myDestroyBlast = Instantiate (destroyBlast).GetComponent<DestroyBlast> ();
			myDestroyBlast.transform.position = transform.position;
			myDestroyBlast.minRadius = minRadius;
			myDestroyBlast.maxRadius = maxRadius;
			myDestroyBlast.lifespan = lifespan;
			myDestroyBlast.timeRate = timeRate;
		}
	}

	public override void OnCollide (Blastable other)
	{
		if (other.pushable) {
			Rigidbody rb = other.GetComponent<Rigidbody> ();

			Vector3 force = (other.transform.position - transform.position).normalized * explosionForce * myFixedDeltaTime * 60;
			rb.AddForce (force, ForceMode.Impulse);
		}
	}
}
