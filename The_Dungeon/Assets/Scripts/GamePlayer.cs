using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    public float moveSpeed = 15f;
	public float myGravityScale;
    public float jumpVelocity = 30f;

    public bool doubleJumped = false;
	public bool stunned = false;
    public float dashSpeed = 100f;
    public float dashFullTime = 0.1f;
	public float dashCooldown = 0.4f;
    public float dashTime;
	private bool canDash = true;
    private bool isDashing = false;
	private bool grounded = false;
	private float lastMoveDirection = 1f;

	// control variables (on keypress)
	private bool shouldJump = false;
	private bool shouldDoubleJump = false;
	private bool shouldDash = false;
	private float moveLeftRight = 0f;

	private Vector2 myVectorUp = new Vector2(0, 1);
	private Vector2 myVectorRight = new Vector2(1, 0);
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private GameObject cameraObject = null;

    // Start is called before the first frame update
    void Start()
    {
		
		if(!hasAuthority) { return; }
		
		rigidBody = this.GetComponent<Rigidbody2D>();
		myGravityScale = rigidBody.mass * 50f;		// F = m * g
		boxCollider = this.GetComponent<BoxCollider2D>();

		SwitchGravity("down");
        dashTime = dashFullTime;
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
				dashTime = dashFullTime;
				isDashing = true;
				shouldDash = false;
			} else if (!isDashing) {
				rigidBody.velocity = (myVectorRight * moveLeftRight) + (VecAbs(myVectorUp) * rigidBody.velocity);
			}

			
			// handle dashing
			dashTime -= Time.fixedDeltaTime;
			if (isDashing) {
				if (dashTime <= 0) {	// dash just ended
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
		

		if (cameraObject == null)
		{
			cameraObject = GameObject.Find("Camera");
			if (cameraObject != null) cameraObject.GetComponent<CameraFollow>().FollowPlayer(this.GetComponent<Transform>());
		}
		
	}

	private void HandleInput() {
		// Input
		if (grounded && !stunned && Input.GetKeyDown(KeyCode.Space))									shouldJump = true;
		if (!grounded && !doubleJumped && !stunned && Input.GetKeyDown(KeyCode.Space))					shouldDoubleJump = true;
		if (canDash && dashTime <= -dashCooldown && !stunned && Input.GetKeyDown(KeyCode.LeftControl))	shouldDash = true;
		if (!isDashing && !stunned)					moveLeftRight = Input.GetAxis("HorizontalAD") * moveSpeed; else moveLeftRight = 0;


		// Player movement logic
		grounded = IsGrounded();
		if (grounded) {								// jump/dash available
			doubleJumped = false;
			canDash = true;
		}
		if (shouldDoubleJump) {						// double jump
			doubleJumped = true;
			dashTime = -dashCooldown;
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
        int layerMask = ~(1 << 8);
		RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, -myVectorUp, 0.1f, layerMask);
        return (raycast.collider != null);
    }

	private Vector2 VecAbs(Vector2 a) {
		a.x = Mathf.Abs(a.x);
		a.y = Mathf.Abs(a.y);
		return a;
	}




	[SyncVar]
	private string displayName = "Player";

	[SyncVar]
	private Color displayColor;


	[Server]
	public void SetNameAndColor(string name, Color color)
	{
		this.displayName = name;
		this.displayColor = color;
		this.GetComponent<SpriteRenderer>().color = color;
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

	

}