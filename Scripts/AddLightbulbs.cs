using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class AddLightbulbs : MonoBehaviour
{

	public Vector3 scale = Vector3.one;
	public Material material;
	public Color lightColor = Color.white;
	public float range = 50;

	public Vector2 count;
	public Vector2 padding;
	public float yOffset = 0.5f;

	public float intensity = 5000;

	public void Generate ()
	{
		Material myMaterial = new Material (material);
		Vector3 effectiveHalfSize = new Vector2 (0.5f, 0.5f) - padding;
		for (int x = 0; x < count.x; x++) {
			for (int z = 0; z < count.y; z++) {
				float lightX = 0;
				if (count.x != 1) {
					lightX = Mathf.Lerp (-effectiveHalfSize.x, effectiveHalfSize.x, x / (count.x - 1f));
				}
				float lightZ = 0;
				if (count.y != 1) {
					lightZ = Mathf.Lerp (-effectiveHalfSize.y, effectiveHalfSize.y, z / (count.y - 1f));
				}
				float lightY = yOffset - 0.5f - scale.y / 2;
				Vector3 pos = new Vector3 (lightX, lightY, lightZ);
				GameObject newLightbulb = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				newLightbulb.transform.localScale = scale;
				newLightbulb.name = "Generated Lightbulb";
				newLightbulb.GetComponent<Renderer> ().sharedMaterial = myMaterial;
				newLightbulb.transform.parent = transform;
				newLightbulb.transform.localPosition = pos;

				GameObject light = new GameObject ("Point Light");
				Light lightComp = light.AddComponent<Light> ();
				lightComp.type = LightType.Point;
				lightComp.range = range;
				lightComp.intensity = intensity / (4 * Mathf.PI);
				lightComp.bounceIntensity = 0;
				lightComp.color = lightColor;

				light.transform.parent = newLightbulb.transform;
				light.transform.localPosition = new Vector3 (0, -1f, 0);

				newLightbulb.tag = "Generated Lightbulb";
			}
		}
	}

	public void Remove ()
	{
		List<GameObject> children = new List<GameObject> ();
		foreach (Transform child in transform) {
			if (child.tag.Equals ("Generated Lightbulb")) {
				children.Add (child.gameObject);
			}
		}
		foreach (GameObject child in children) {
			DestroyImmediate (child);
		}
	}
}
