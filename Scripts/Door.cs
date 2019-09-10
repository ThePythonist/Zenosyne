using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(MeshCollider))]
public class Door : MonoBehaviour
{

	public GameObject normalCube;
	public GameObject lockedCube;
	public GameObject unlockedCube;
	public GameObject keyParticleSystem;

	MeshCollider meshCollider;

	bool isFullyOpen = false;

	public enum MovementType
	{
		NONE,
		LEFT,
		RIGHT,
		UP,
		FORWARDS,
		BACKWARDS,
		DOWN

	}

	public enum CubeType
	{
		NORMAL,
		LOCKED,
		UNLOCKED

	}

	public enum OpenType
	{
		FADE_UP,
		FADE_DOWN,
		FADE_LEFT,
		FADE_RIGHT,
		FADE_FORWARD,
		FADE_BACKWARD,

		FOLD_UP,
		FOLD_DOWN,
		FOLD_LEFT,
		FOLD_RIGHT,
		FOLD_FORWARD,
		FOLD_BACKWARD
	}

	bool opening = false;
	bool madeStumpCollider = false;

	public CubeType cubeType = CubeType.NORMAL;
	public Vector3 addCubeAtPosition = Vector3.zero;
	public MovementType movement = MovementType.NONE;

	public OpenType openType = OpenType.FADE_UP;
	public float openSpeed = 1f;

	public void AddNewCube ()
	{
		GameObject cubeToInstantiate = normalCube;
		if (cubeType == CubeType.LOCKED) {
			cubeToInstantiate = lockedCube;
		} else if (cubeType == CubeType.UNLOCKED) {
			cubeToInstantiate = unlockedCube;
		}
		GameObject newCube = (GameObject)Instantiate (cubeToInstantiate, transform);
		newCube.transform.localPosition = addCubeAtPosition;
		if (movement == MovementType.LEFT) {
			addCubeAtPosition += new Vector3 (-1, 0, 0);
		} else if (movement == MovementType.RIGHT) {
			addCubeAtPosition += new Vector3 (1, 0, 0);
		} else if (movement == MovementType.UP) {
			addCubeAtPosition += new Vector3 (0, 1, 0);
		} else if (movement == MovementType.DOWN) {
			addCubeAtPosition += new Vector3 (0, -1, 0);
		} else if (movement == MovementType.FORWARDS) {
			addCubeAtPosition += new Vector3 (0, 0, 1);
		} else if (movement == MovementType.BACKWARDS) {
			addCubeAtPosition += new Vector3 (0, 0, -1);
		}
	}

	public void RemoveAllCubes ()
	{
		Transform[] children = GetComponentsInChildren<Transform> ();
		for (int i = children.Length - 1; i >= 0; i--) {
			if (children [i] != transform) {
				DestroyImmediate (children [i].gameObject);
			}
		}
	}

	bool Approximately (Vector3 a, Vector3 b)
	{
		return Mathf.Approximately (a.x, b.x) && Mathf.Approximately (a.y, b.y) && Mathf.Approximately (a.z, b.z);
	}

	public void GenerateCollider ()
	{
		meshCollider = GetComponent<MeshCollider> ();
		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();
		int i = 0;
		List<Transform> checkedTransforms = new List<Transform> ();
		foreach (Transform child in transform) {
			bool isClone = false;
			foreach (Transform checkedTransform in checkedTransforms) {
				if (Approximately (checkedTransform.position, child.position) && Approximately (checkedTransform.lossyScale, child.lossyScale)) {
					isClone = true;
					break;
				}
			}
			if (isClone) {
				continue;
			}
			checkedTransforms.Add (child);
			Bounds bounds = child.GetComponent<MeshFilter> ().sharedMesh.bounds;
			int[][] tris = new int[][] { new int[]{ 1, 3, 0 },
				new int[]{ 3, 2, 0 },
				new int[]{ 3, 7, 2 },
				new int[]{ 3, 6, 2 },
				new int[]{ 4, 6, 7 },
				new int[]{ 4, 7, 5 },
				new int[]{ 0, 4, 5 },
				new int[]{ 0, 5, 1 },
				new int[]{ 0, 2, 6 },
				new int[]{ 0, 6, 4 },
				new int[]{ 7, 3, 1 },
				new int[]{ 5, 7, 1 }
			};
			for (int j = 0; j < tris.Length; j++) {
				triangles.Add (i + tris [j] [0]);
				triangles.Add (i + tris [j] [1]);
				triangles.Add (i + tris [j] [2]);
			}
			for (int x = -1; x <= 1; x += 2) {
				for (int y = -1; y <= 1; y += 2) {
					for (int z = -1; z <= 1; z += 2) {
						Vector3 v = new Vector3 (x * bounds.extents.x, y * bounds.extents.y, z * bounds.extents.z);
						v.Scale (child.localScale);
						v += child.localPosition;
						vertices.Add (v);
						i++;
					}
				}
			}
		}

		List<Vector3> vertsNoClones = new List<Vector3> ();
		int removed = 0;
		for (i = 0; i < vertices.Count; i++) {
			if (vertsNoClones.Contains (vertices [i])) {
				int realIndex = vertsNoClones.IndexOf (vertices [i]);				
				for (int j = 0; j < triangles.Count; j++) {
					if (triangles [j] == i - removed) {
						triangles [j] = realIndex;
					} else if (triangles [j] > i - removed) {
						triangles [j]--;
					}
				}
				removed++;
			} else {
				vertsNoClones.Add (vertices [i]);
			}
		}

		List<int> trisNoClones = new List<int> ();

		for (i = 0; i < triangles.Count; i += 3) {
			int[] tri = new int[]{ triangles [i], triangles [i + 1], triangles [i + 2] };
			bool clone = false;
			for (int j = 0; j < triangles.Count; j += 3) {
				if (i != j) {
					int[] otherTri = new int[] { triangles [j], triangles [j + 1], triangles [j + 2] };
					if (areSameTriangle (tri, otherTri)) {
						clone = true;
						break;
					}
				}
			}
			if (!clone) {
				trisNoClones.Add (tri [0]);
				trisNoClones.Add (tri [1]);
				trisNoClones.Add (tri [2]);
			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertsNoClones.ToArray ();
		mesh.triangles = trisNoClones.ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		meshCollider.sharedMesh = mesh;
	}

	bool areSameTriangle (int[] tri0, int[] tri1)
	{
		return vertsAreContainedBy (tri0, tri1) && vertsAreContainedBy (tri1, tri0);
	}

	bool vertsAreContainedBy (int[] tri0, int[] tri1)
	{
		foreach (int i in tri0) {
			bool found = false;
			foreach (int j in tri1) {
				if (i == j) {
					found = true;
					break;
				}
			}
			if (!found) {
				return false;
			}
		}
		return true;
	}

	void Update ()
	{
		if (isFullyOpen && !madeStumpCollider) {
			madeStumpCollider = true;
			GetComponent<MeshCollider> ().enabled = true;
			GenerateCollider ();
		}
		bool unlocked = true;
		foreach (Transform child in transform) {
			Lock childLock = child.gameObject.GetComponent<Lock> ();
			if (childLock != null) {
				if (childLock.locked) {
					unlocked = false;
					break;
				}
			}
		}
		if (unlocked) {
			if (!opening) {
				opening = true;
				GetComponent<MeshCollider> ().enabled = false;

				Transform[] children = GetComponentsInChildren<Transform> ();
				for (int i = children.Length - 1; i >= 0; i--) {
					Key key = children [i].GetComponent<Key> ();
					if (key != null) {
						GameObject psGO = (GameObject)Instantiate (keyParticleSystem);
						psGO.transform.position = children [i].transform.position;
						ParticleSystem ps = psGO.GetComponent<ParticleSystem> ();
						ParticleSystemRenderer psRenderer = psGO.GetComponent<ParticleSystemRenderer> ();
						AudioSource audioSource = psGO.GetComponent<AudioSource> ();
						psRenderer.material.SetColor ("_Tint", key.currentColor);
						ps.Play ();
						Destroy (children [i].gameObject);
						Destroy (psGO, Mathf.Max (ps.main.duration, audioSource.clip.length));
					}
				}
				children = GetComponentsInChildren<Transform> ();
				for (int i = children.Length - 1; i >= 0; i--) {
					Lock myLock = children [i].GetComponent<Lock> ();
					if (myLock != null) {
						Instantiate (normalCube, children [i].position, children [i].rotation, transform);
						Destroy (children [i].gameObject);
					}
				}
			} else if (!isFullyOpen) {
				isFullyOpen = true;
				if (openType == OpenType.FADE_UP) {
					float highestY = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.y > highestY) {
							highestY = child.position.y;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newY = child.position.y + distToMove;
						if (newY > highestY) {
							newY = highestY;
						}
						child.transform.position = new Vector3 (child.transform.position.x, newY, child.transform.position.z);
						if (!Mathf.Approximately (child.position.y, highestY)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FADE_DOWN) {
					float lowestY = float.MaxValue;
					foreach (Transform child in transform) {
						if (child.position.y < lowestY) {
							lowestY = child.position.y;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newY = child.position.y - distToMove;
						if (newY < lowestY) {
							newY = lowestY;
						}
						child.transform.position = new Vector3 (child.transform.position.x, newY, child.transform.position.z);
						if (!Mathf.Approximately (child.position.y, lowestY)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FADE_RIGHT) {
					float highestX = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.x > highestX) {
							highestX = child.position.x;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newX = child.position.x + distToMove;
						if (newX > highestX) {
							newX = highestX;
						}
						child.transform.position = new Vector3 (newX, child.transform.position.y, child.transform.position.z);
						if (!Mathf.Approximately (child.position.x, highestX)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FADE_LEFT) {
					float lowestX = float.MaxValue;
					foreach (Transform child in transform) {
						if (child.position.x < lowestX) {
							lowestX = child.position.x;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newX = child.position.x - distToMove;
						if (newX < lowestX) {
							newX = lowestX;
						}
						child.transform.position = new Vector3 (newX, child.transform.position.y, child.transform.position.z);
						if (!Mathf.Approximately (child.position.x, lowestX)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FADE_FORWARD) {
					float highestZ = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.z > highestZ) {
							highestZ = child.position.z;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newZ = child.position.z + distToMove;
						if (newZ > highestZ) {
							newZ = highestZ;
						}
						child.transform.position = new Vector3 (child.transform.position.x, child.transform.position.y, newZ);
						if (!Mathf.Approximately (child.position.z, highestZ)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FADE_BACKWARD) {
					float lowestZ = float.MaxValue;
					foreach (Transform child in transform) {
						if (child.position.z < lowestZ) {
							lowestZ = child.position.z;
						}
					}
					float distToMove = openSpeed * Time.deltaTime;
					foreach (Transform child in transform) {
						float newZ = child.position.z - distToMove;
						if (newZ < lowestZ) {
							newZ = lowestZ;
						}
						child.transform.position = new Vector3 (child.transform.position.x, child.transform.position.y, newZ);
						if (!Mathf.Approximately (child.position.z, lowestZ)) {
							isFullyOpen = false;
						}
					}
				} else if (openType == OpenType.FOLD_UP) {
					isFullyOpen = false;
					float lowestY = float.MaxValue;
					float highestY = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.y < lowestY) {
							lowestY = child.position.y;
						}
						if (child.position.y > highestY) {
							highestY = child.position.y;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newLowestY = lowestY + distToMove;
					if (newLowestY > highestY) {
						newLowestY = highestY;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.y < newLowestY) {
							child.transform.position = new Vector3 (child.transform.position.x, newLowestY, child.transform.position.z);
						}
					}
				} else if (openType == OpenType.FOLD_DOWN) {
					isFullyOpen = false;
					float lowestY = float.MaxValue;
					float highestY = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.y < lowestY) {
							lowestY = child.position.y;
						}
						if (child.position.y > highestY) {
							highestY = child.position.y;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newHighestY = highestY - distToMove;
					if (newHighestY < lowestY) {
						newHighestY = lowestY;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.y > newHighestY) {
							child.transform.position = new Vector3 (child.transform.position.x, newHighestY, child.transform.position.z);
						}
					}
				} else if (openType == OpenType.FOLD_RIGHT) {
					isFullyOpen = false;
					float lowestX = float.MaxValue;
					float highestX = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.x < lowestX) {
							lowestX = child.position.x;
						}
						if (child.position.x > highestX) {
							highestX = child.position.x;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newLowestX = lowestX + distToMove;
					if (newLowestX > highestX) {
						newLowestX = highestX;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.x < newLowestX) {
							child.transform.position = new Vector3 (newLowestX, child.transform.position.y, child.transform.position.z);
						}
					}
				} else if (openType == OpenType.FOLD_LEFT) {
					isFullyOpen = false;
					float lowestX = float.MaxValue;
					float highestX = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.x < lowestX) {
							lowestX = child.position.x;
						}
						if (child.position.x > highestX) {
							highestX = child.position.x;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newHighestX = highestX - distToMove;
					if (newHighestX < lowestX) {
						newHighestX = lowestX;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.x > newHighestX) {
							child.transform.position = new Vector3 (newHighestX, child.transform.position.y, child.transform.position.z);
						}
					}
				} else if (openType == OpenType.FOLD_FORWARD) {
					isFullyOpen = false;
					float lowestZ = float.MaxValue;
					float highestZ = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.z < lowestZ) {
							lowestZ = child.position.z;
						}
						if (child.position.z > highestZ) {
							highestZ = child.position.z;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newLowestZ = lowestZ + distToMove;
					if (newLowestZ > highestZ) {
						newLowestZ = highestZ;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.z < newLowestZ) {
							child.transform.position = new Vector3 (child.transform.position.x, child.transform.position.y, newLowestZ);
						}
					}
				} else if (openType == OpenType.FOLD_BACKWARD) {
					isFullyOpen = false;
					float lowestZ = float.MaxValue;
					float highestZ = float.MinValue;
					foreach (Transform child in transform) {
						if (child.position.z < lowestZ) {
							lowestZ = child.position.z;
						}
						if (child.position.z > highestZ) {
							highestZ = child.position.z;
						}
					}

					float distToMove = openSpeed * Time.deltaTime;
					float newHighestZ = highestZ - distToMove;
					if (newHighestZ < lowestZ) {
						newHighestZ = lowestZ;
						isFullyOpen = true;
					}
					foreach (Transform child in transform) {
						if (child.position.z > newHighestZ) {
							child.transform.position = new Vector3 (child.transform.position.x, child.transform.position.y, newHighestZ);
						}
					}
				}
					
			}
		}
	}

}
