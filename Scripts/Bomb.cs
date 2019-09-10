using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Clickable))]
public class Bomb : Process
{

	public float radius = 1;
	public float life = 0;
	public float lifeSpan = 3;
	public bool primed = false;

	MeshRenderer meshRenderer;
	Clickable clickable;

	public GameObject pushBlast;
	public GameObject destroyBlast;

	public float blastRadius = 5;
	public float blastLifespan = 1;

	public float blastDelay = 0.5f;

	public float explosionForce = 1000;

	SceneManagerController smc;

	bool isQuitting = false;

	void Start ()
	{
		smc = FindObjectOfType<SceneManagerController> ();
		clickable = GetComponent<Clickable> ();
		meshRenderer = GetComponent<MeshRenderer> ();
		SetScale ();
		UpdateMaterial ();
	}

	void Update ()
	{
		if (clickable.IsClicked ()) {
			primed = true;
		}
		if (primed) {
			life += myDeltaTime / lifeSpan;
		}
		UpdateMaterial ();
	}

	void LateUpdate ()
	{
		if (life >= 1) {
			Destroy (gameObject);
		}
	}

	void UpdateMaterial ()
	{
		meshRenderer.material.SetFloat ("_Life", life);
	}

	void OnApplicationQuit ()
	{
		isQuitting = true;
	}

	void OnValidate ()
	{
		SetScale ();
	}

	void SetScale ()
	{
		transform.localScale = new Vector3 (2 * radius, 2 * radius, 2 * radius);
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();
		if (!isQuitting && !smc.isSceneChanging) {
			PushBlast myPushBlast = Instantiate (pushBlast).GetComponent<PushBlast> ();
			myPushBlast.transform.position = transform.position;
			myPushBlast.minRadius = radius;
			myPushBlast.maxRadius = blastRadius;
			myPushBlast.lifespan = blastLifespan;
			myPushBlast.timeRate = timeRate;
			myPushBlast.explosionForce = explosionForce;

			myPushBlast.delay = blastDelay;
			myPushBlast.destroyBlast = destroyBlast;
		}
	}

	void OnClick ()
	{
		primed = true;
	}
}
