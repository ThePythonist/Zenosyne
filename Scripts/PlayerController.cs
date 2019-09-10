using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

	public float speed = 350f;
	public Transform eye;

	public float reachDistance = 5;

	public Texture2D crosshair;

	float yaw = 0;
	float pitch = 0;

	public Transform groundChecker;
	public LayerMask whatIsGround;
	public float jumpForce = 2500;
	public float groundRadius = 0.2f;

	public float holdingDistance = 1;
	public float throwForce = 200;

	public LayerMask canClickLayerMask;

	Transform holding = null;

	Rigidbody rb;

	bool pressing0 = false;
	bool pressing1 = false;

	public float jumpCooldown = 0f;

	public float droppingSkin = 0.01f;

	float timeSinceLastJump;

	int holdingLayer;

	bool canDrop = false;
	bool canThrow = false;

	bool jumping = false;

	BoxCollider holdingCollider;

	public GameObject pauseMenu;

	public float fallMultiplier = 2.5f;

	enum GameState
	{
		NORMAL,
		PAUSED
	}

	GameState gameState;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		timeSinceLastJump = jumpCooldown;
		yaw = transform.localEulerAngles.y;
	}

	public void Pause ()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		gameState = GameState.PAUSED;
		pauseMenu.SetActive (true);
	}

	public void Unpause ()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		gameState = GameState.NORMAL;
		pauseMenu.SetActive (false);
	}

	void Update ()
	{
		if (rb.velocity.y < 0) {
			rb.velocity += Physics.gravity.y * Vector3.up * (fallMultiplier - 1f) * Time.deltaTime;
		}
		float panSpeed;
		if (PlayerPrefs.HasKey ("MouseSensitivity")) {
			panSpeed = PlayerPrefs.GetFloat ("MouseSensitivity");
		} else {
			panSpeed = 2f;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (gameState == GameState.NORMAL) {
				Pause ();
			} else if (gameState == GameState.PAUSED) {
				Unpause ();
			}
		}

		if (holding != null) {
			Vector3 center = holding.GetComponent<MeshFilter> ().mesh.bounds.center;
			center.Scale (holding.lossyScale);
			float totalHoldingDistance = holdingDistance + holding.GetComponent<MeshFilter> ().mesh.bounds.extents.z * holding.lossyScale.z - center.z;
			holding.transform.localEulerAngles = transform.localEulerAngles;
			holding.transform.position = transform.position + totalHoldingDistance * transform.forward - center.x * transform.right - center.y * transform.up;
		}


		eye.localEulerAngles = new Vector3 (pitch, 0, 0);
		transform.localEulerAngles = new Vector3 (0, yaw, 0);

		if (gameState == GameState.NORMAL) {
			yaw += panSpeed * Input.GetAxis ("Mouse X");
			pitch -= panSpeed * Input.GetAxis ("Mouse Y");
			pitch = Mathf.Clamp (pitch, -80, 80);
			if (Input.GetMouseButtonDown (0)) {
				if (!pressing0) {
					Clickable clickable = CastRay ();
					if (clickable != null) {
						clickable.SetClicked (true);
					}
				}
				pressing0 = true;
			} else {
				pressing0 = false;
			}

			if (Input.GetMouseButton (1)) {
				if (!pressing1) {
					if (holding == null) {
						Clickable clickable = CastRay ();
						if (clickable != null) {
							if (clickable.pickupable) {
								holding = clickable.transform;
								clickable.player = this;
								MeshFilter meshFilter = holding.GetComponent<MeshFilter> ();
								float colXScale = meshFilter.mesh.bounds.extents.x * 2f / transform.lossyScale.x * holding.lossyScale.x;

								float colYScale = meshFilter.mesh.bounds.extents.y * 2f / transform.lossyScale.y * holding.lossyScale.y;

								float colZScale = meshFilter.mesh.bounds.extents.z * 2f / transform.lossyScale.z * holding.lossyScale.z;
								float colZPos = (holdingDistance + meshFilter.mesh.bounds.extents.z * holding.lossyScale.z) / transform.lossyScale.z;

								holdingCollider = gameObject.AddComponent<BoxCollider> ();
								holdingCollider.size = new Vector3 (colXScale, colYScale, colZScale);
								holdingCollider.center = new Vector3 (0, 0, colZPos);

								holdingLayer = holding.gameObject.layer;
								holding.gameObject.layer = LayerMask.NameToLayer ("Holding");
							}
						}
					} else {
						if (canDrop) {
							Rigidbody rb = holding.GetComponent<Rigidbody> ();
							rb.velocity = this.rb.velocity;
							rb.angularVelocity = this.rb.angularVelocity;
							LetGo ();
						}
					}
				}
				pressing1 = true;
			} else {
				pressing1 = false;
			}
		}
	}

	void FixedUpdate ()
	{
		GetComponent<Rigidbody> ().centerOfMass = Vector3.zero;
		if (gameState == GameState.NORMAL) {
			Vector3 motion = Vector3.zero;
			Vector3 horizontalMotion = transform.right * Input.GetAxisRaw ("Horizontal");
			Vector3 verticalMotion = transform.forward * Input.GetAxisRaw ("Vertical");

			motion = (horizontalMotion + verticalMotion).normalized * speed * Time.fixedDeltaTime;

			Collider[] colliders = Physics.OverlapSphere (groundChecker.position, groundRadius, whatIsGround);

			if (colliders != null && colliders.Length > 0) {
				if (Input.GetAxis ("Jump") != 0 && timeSinceLastJump >= jumpCooldown) {
					if (!jumping) {
						rb.AddForce (transform.up * jumpForce * rb.mass * rb.drag, ForceMode.Impulse);
						timeSinceLastJump = 0;
						jumping = true;
					}
				} else {
					jumping = false;
				}
			}

			rb.velocity += motion;

			canDrop = false;
			canThrow = false;
			if (holding != null) {
				canDrop = true;
				canThrow = true;
				Vector3 exts = holding.GetComponent<MeshFilter> ().mesh.bounds.extents;
				Vector3 center = holding.GetComponent<MeshFilter> ().mesh.bounds.center;
				center.Scale (holding.lossyScale);
				exts.Scale (holding.lossyScale);
				Collider[] droppingCollisions = Physics.OverlapBox (holding.position + center, exts - droppingSkin * Vector3.one, holding.rotation);
				if (droppingCollisions != null) {
					foreach (Collider collider in droppingCollisions) {
						if (collider.transform != holding && collider.gameObject.layer != LayerMask.NameToLayer ("Player")) {
							canDrop = false;
							break;
						}
					}
				}

				float totalHoldingDistance = holdingDistance + holding.GetComponent<MeshFilter> ().mesh.bounds.extents.z * holding.lossyScale.z - center.z;
				Vector3 throwStartPos = eye.position + totalHoldingDistance * eye.forward - center.x * eye.right - center.y * eye.up;
				droppingCollisions = Physics.OverlapBox (throwStartPos, exts);
				if (droppingCollisions != null) {
					foreach (Collider collider in droppingCollisions) {
						if (collider.transform != holding && collider.gameObject.layer != LayerMask.NameToLayer ("Player")) {
							canThrow = false;
							break;
						}
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				if (holding != null) {
					if (canThrow) {
						Vector3 center = holding.GetComponent<MeshFilter> ().mesh.bounds.center;
						center.Scale (holding.lossyScale);
						float totalHoldingDistance = holdingDistance + holding.GetComponent<MeshFilter> ().mesh.bounds.extents.z * holding.lossyScale.z - center.z;
						Vector3 throwStartPos = eye.position + totalHoldingDistance * eye.forward - center.x * eye.right - center.y * eye.up;
						holding.transform.position = throwStartPos;
						Rigidbody rb = holding.GetComponent<Rigidbody> ();
						rb.AddForce (eye.forward * throwForce, ForceMode.Impulse);
						LetGo ();
					}
				}
			}
		}
		timeSinceLastJump += Time.fixedDeltaTime;
		if (rb.velocity.magnitude < 0.01f) {
			rb.velocity = Vector3.zero;
		}
	}

	void OnGUI ()
	{
		if (gameState == GameState.NORMAL) {
			GUI.DrawTexture (new Rect ((Screen.width - crosshair.width) / 2, (Screen.height - crosshair.height) / 2, crosshair.width, crosshair.height), crosshair);
		}
	}

	Clickable CastRay ()
	{
		RaycastHit hit;
		Vector3 rayOrigin = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
		if (Physics.Raycast (rayOrigin, eye.forward, out hit, reachDistance, canClickLayerMask, QueryTriggerInteraction.UseGlobal)) {
			return hit.collider.GetComponent<Clickable> ();
		}
		return null;
	}

	public void LetGo ()
	{
		holding.GetComponent<Clickable> ().player = null;
		holding.gameObject.layer = holdingLayer;
		holding = null;
		Destroy (holdingCollider);
		holdingCollider = null;
	}

	public bool isHolding (Transform t)
	{
		return holding == t;
	}

	public void SetRotation (Quaternion rotation)
	{
		transform.rotation = rotation;
		yaw = rotation.eulerAngles.y;
	}
}
