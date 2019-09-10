using UnityEngine;

public abstract class Blast : Process
{

	[HideInInspector]
	public float minRadius = 1;
	[HideInInspector]
	public float maxRadius = 5;
	[HideInInspector]
	public float lifespan = 1;
	
	float life = 0;
	float radius;

	public virtual void Update ()
	{
		life += myDeltaTime / lifespan;
		radius = Mathf.Lerp (minRadius, maxRadius, life);
		SetRadius (radius);
	}

	public virtual void FixedUpdate ()
	{
		foreach (Blastable blastable in Blastable.blastables) {
			if (CollidesWith (blastable)) {
				OnCollide (blastable);
			}
		}
	}

	void LateUpdate ()
	{
		if (life >= 1) {
			Disappear ();
		}
	}

	void SetRadius (float radius)
	{
		transform.localScale = new Vector3 (2 * radius, 2 * radius, 2 * radius);
	}

	void Disappear ()
	{
		Destroy (gameObject);
	}

	bool CollidesWith (Blastable blastable)
	{
		Collider[] inside = Physics.OverlapSphere (transform.position, radius);
		if (inside != null) {
			foreach (Collider collider in inside) {
				if (collider.gameObject == blastable.gameObject) {
					return true;
				}
			}
		}
		return false;
	}

	public virtual void OnCollide (Blastable blastable)
	{
		
	}
}
