using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class ConveyorBelt : Process
{

	public Texture2D texture;

	public float baseSpeed = 1;
	public float pushForceCoeff = 1;

	Vector3 oldScale;
	MeshRenderer meshRenderer;

	void  Start ()
	{
		oldScale = transform.lossyScale;
		meshRenderer = GetComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial.SetFloat ("_BaseSpeed", baseSpeed);
		UpdateTexture ();
	}

	void Update ()
	{
		if (transform.lossyScale != oldScale) {
			oldScale = transform.lossyScale;
			UpdateTexture ();
		}
		meshRenderer.material.SetFloat ("_TimeRate", timeRate);
	}

	void FixedUpdate ()
	{
		Collider[] colliders = Physics.OverlapBox (transform.position + new Vector3 (0, 0.5f * transform.lossyScale.y, 0), 0.5f * transform.lossyScale + new Vector3 (0, 0.1f, 0));
		if (colliders != null) {
			foreach (Collider col in colliders) {
				if (col.attachedRigidbody != null) {
					if (!col.attachedRigidbody.isKinematic) {
						col.attachedRigidbody.velocity += (-transform.forward * baseSpeed * myFixedDeltaTime * 60 * pushForceCoeff);
					}
				}
			}
		}
	}

	void UpdateTexture ()
	{
		Texture2D fullTex = new Texture2D (texture.width, Mathf.CeilToInt (transform.lossyScale.z / transform.lossyScale.x) * texture.width);
		for (int y = 0; y < fullTex.height; y++) {
			for (int x = 0; x < fullTex.width; x++) {
				fullTex.SetPixel (x, y, texture.GetPixel (x, y % texture.height));

			}
		}
		fullTex.Apply ();
		meshRenderer.sharedMaterial.SetFloat ("_ScaleRatio", transform.lossyScale.x / transform.lossyScale.z);
		meshRenderer.sharedMaterial.SetTexture ("_ConveyorTexture", fullTex);
	}

}
