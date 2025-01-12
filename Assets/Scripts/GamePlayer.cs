using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
	public float moveSpeed = 15f;
	public float jumpVelocity = 30f;

	private bool doubleJumped = false;
	private bool stunned = false;
	public float dashSpeed = 100f;
	public float dashFullTime = 0.1f;
	public float dashCooldown = 0.4f;
	
	private float myGravityScale;
	private float currentDashTime;
	private bool canDash = true;
	private bool isDashing = false;
	private bool grounded = false;
	private float lastMoveDirection = 1f;
	public bool wasJustTeleported { get; set; }

	// control variables (on keypress)
	private bool shouldJump = false;
	private bool shouldDoubleJump = false;
	private bool shouldDash = false;
	private float moveLeftRight = 0f;

	private Vector2 myVectorUp = new Vector2(0, 1);
	private Vector2 myVectorRight = new Vector2(1, 0);
	public Rigidbody2D rigidBody;
	private BoxCollider2D boxCollider;
	private GameObject cameraObject = null;

	// Start is called before the first frame update
	void Start()
	{
		GetComponent<SpriteRenderer>().color = displayColor;
		if(!hasAuthority) { return; }
		
		myGravityScale = rigidBody.mass * 50f;		// F = m * g
		boxCollider = GetComponent<BoxCollider2D>();

		SwitchGravity("down");
		currentDashTime = dashFullTime;
	}

	// FixedUpdate is called once per delta t
	void FixedUpdate()
	{
			if (!hasAuthority)
			{
				return;
			}

			if (shouldJump) {
				rigidBody.velocity = myVectorUp * jumpVelocity;
				shouldJump = false;
			}
			if (shouldDoubleJump) {
				rigidBody.velocity = myVectorUp * jumpVelocity;
				shouldDoubleJump = false;
			}

			if (shouldDash) {
				if (lastMoveDirection > 0) rigidBody.velocity = myVectorRight * dashSpeed;
				if (lastMoveDirection < 0) rigidBody.velocity = -myVectorRight * dashSpeed;
				currentDashTime = dashFullTime;
				isDashing = true;
				shouldDash = false;
			} else if (!isDashing) {
				rigidBody.velocity = (myVectorRight * moveLeftRight) + (VecAbs(myVectorUp) * rigidBody.velocity);
			}

			
			// handle dashing
			currentDashTime -= Time.fixedDeltaTime;
			if (isDashing) {
				if (currentDashTime <= 0) {	// dash just ended
					isDashing = false;
					rigidBody.velocity = VecAbs(myVectorUp) * rigidBody.velocity;
				}
			}

			// Gravity
			rigidBody.AddForce(-myVectorUp * myGravityScale, ForceMode2D.Force);
		
	}

	// Update is called once per frame
	
	[Client]
	void Update() {
		
		if (!hasAuthority) { return; }
		
		HandleInput();
		
		if (cameraObject == null) GameObject.Find("Camera")?.GetComponent<CameraFollow>().FollowPlayer(GetComponent<Transform>());		// attach camera to local player
		
	}

	private void HandleInput() {
		// Input
		if (grounded && !stunned && Input.GetKeyDown(KeyCode.Space))									shouldJump = true;
		if (!grounded && !doubleJumped && !stunned && Input.GetKeyDown(KeyCode.Space))					shouldDoubleJump = true;
		if (canDash && currentDashTime <= -dashCooldown && !stunned && Input.GetKeyDown(KeyCode.LeftControl))	shouldDash = true;
		if (!isDashing && !stunned)					moveLeftRight = Input.GetAxis("HorizontalAD") * moveSpeed; else moveLeftRight = 0;


		// Player movement logic
		grounded = IsGrounded();
		if (grounded) {								// jump/dash available
			doubleJumped = false;
			canDash = true;
		}
		if (shouldDoubleJump) {						// double jump
			doubleJumped = true;
			currentDashTime = -dashCooldown;
			canDash = true;
		}
		if (shouldDash) {							// dash
			canDash = false;
		}

		if (moveLeftRight != 0) {					// last direction
			lastMoveDirection = moveLeftRight;
		}

		// Gravity
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			SwitchGravity("down");
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			SwitchGravity("left");
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			SwitchGravity("up");
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			SwitchGravity("right");
		}
	}

	private void SwitchGravity(string direction) {
		switch(direction) {
			case "down":
				myVectorUp = new Vector2(0, 1);
				myVectorRight = new Vector2(1, 0);
				break;
			case "left":
				myVectorUp = new Vector2(1, 0);
				myVectorRight = new Vector2(0, -1);
				break;
			case "up":
				myVectorUp = new Vector2(0, -1);
				myVectorRight = new Vector2(-1, 0);
				break;
			case "right":
				myVectorUp = new Vector2(-1, 0);
				myVectorRight = new Vector2(0, 1);
				break;
  			default:
				// Invalid function input
				break;
		}
	}

	private bool IsGrounded() {
		int layerMask = ~(3 << 8); // 0000000011 -> 1100000000
		RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, -myVectorUp, 0.1f, layerMask);
		return (raycast.collider != null);
	}

	private Vector2 VecAbs(Vector2 a) {
		a.x = Mathf.Abs(a.x);
		a.y = Mathf.Abs(a.y);
		return a;
	}

	public void PortalChangeVelocityDirection(Quaternion portal1Rotation, Quaternion portal2Rotation)
	{
		var rotationDiff = Quaternion.Inverse(portal1Rotation) * portal2Rotation;
		var tempVelocity = rigidBody.velocity;
		rigidBody.velocity = rotationDiff * tempVelocity;
	}

	public void SetDoubleJumped(bool value)
	{
		doubleJumped = value;
	}

#region NETWORKING
	[SyncVar]
	private string displayName = "Player";

	[SyncVar]
	private Color displayColor;


	[Server]
	public void SetNameAndColor(string name, Color color)
	{
		displayName = name;
		displayColor = color;
	}

	private MyNetworkManager room;
	private MyNetworkManager Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as MyNetworkManager;
		}
	}

	public override void OnStartClient()
	{
		DontDestroyOnLoad(gameObject);

		if (Room != null) Room.GamePlayers.Add(this);
	}

	public override void OnStopClient()
	{
		if (Room != null) Room.GamePlayers.Remove(this);
	}

	[TargetRpc]
	public void TargetTeleportPlayer(NetworkConnection conn, float new_x, float new_y)
	{
		if (!hasAuthority) { return; }
		transform.position = new Vector3(new_x, new_y, 0);
	}
#endregion
}