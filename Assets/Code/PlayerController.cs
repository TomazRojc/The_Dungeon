using UnityEngine;

namespace Code
{
	public class PlayerController : MonoBehaviour
	{
		public float moveSpeed = 15f;
		public float jumpVelocity = 30f;
		public float speedChangeFactor = 5f;

		private bool doubleJumped = false;
		private bool stunned = false;
		public float dashSpeed = 100f;
		public float dashFullTime = 0.1f;
		public float dashCooldown = 0.4f;

		private float myGravityScale;
		private float currentDashTime;
		private bool canDash = true;
		private bool isDashing;
		private bool grounded;
		private float lastMoveDirection = 1f;

		// control variables (on keypress)
		private bool shouldJump;
		private bool shouldDoubleJump;
		private bool shouldDash;
		private float moveLeftRight;

		private Vector2 myVectorUp = new Vector2(0, 1);
		private Vector2 myVectorRight = new Vector2(1, 0);
		public Rigidbody2D rigidBody;
		private BoxCollider2D boxCollider;
		private GameObject cameraObject = null;

		void Start()
		{
			// GetComponent<SpriteRenderer>().color = displayColor;

			myGravityScale = rigidBody.mass * 50f; // F = m * g
			boxCollider = GetComponent<BoxCollider2D>();

			SwitchGravity("down");
			currentDashTime = dashFullTime;
		}

		void FixedUpdate()
		{
			grounded = IsGrounded();
			if (grounded)
			{
				// jump/dash available
				doubleJumped = false;
				canDash = true;
			}
			
			if (shouldJump)
			{
				rigidBody.velocity = myVectorUp * jumpVelocity;
				shouldJump = false;
			}

			if (shouldDoubleJump)
			{
				rigidBody.velocity = myVectorUp * jumpVelocity;
				shouldDoubleJump = false;
				doubleJumped = true;
				canDash = true;
			}

			if (shouldDash)
			{
				if (lastMoveDirection > 0) rigidBody.velocity = myVectorRight * dashSpeed;
				if (lastMoveDirection < 0) rigidBody.velocity = -myVectorRight * dashSpeed;
				currentDashTime = dashFullTime;
				isDashing = true;
				shouldDash = false;
				canDash = false;
			}
			else if (!isDashing)
			{
				//TODO: tomazr slowly decrease left/right speed if controls are not being touched, otherwise give full control to player
				var targetVelocity = (myVectorRight * moveLeftRight) + (VecAbs(myVectorUp) * rigidBody.velocity);
				rigidBody.velocity =
					Vector2.Lerp(rigidBody.velocity, targetVelocity, Time.deltaTime * speedChangeFactor);
			}


			// handle dashing
			currentDashTime -= Time.fixedDeltaTime;
			if (isDashing)
			{
				if (currentDashTime <= 0)
				{
					// dash just ended
					isDashing = false;
					rigidBody.velocity = VecAbs(myVectorUp) * rigidBody.velocity;
				}
			}

			// Gravity
			rigidBody.AddForce(-myVectorUp * myGravityScale, ForceMode2D.Force);

		}

		public void HandleJumpInput()
		{
			if (grounded && !stunned)
			{
				shouldJump = true;
			}

			if (!grounded && !doubleJumped && !stunned)
			{
				shouldDoubleJump = true;
			}
		}
		
		public void HandleDashInput()
		{
			if (canDash && currentDashTime <= -dashCooldown && !stunned)
			{
				shouldDash = true;
			}
		}
		
		public void HandleMoveInput(Vector2 direction)
		{
			if (!isDashing && !stunned)
			{
				moveLeftRight = direction.x * moveSpeed;
				lastMoveDirection = moveLeftRight;
			}
			else
			{
				moveLeftRight = 0;
			}
		}
		
		public void HandleLookInput(Vector2 direction)
		{
			if (direction.x < -0.9)
			{
				SwitchGravity("left");
			}
			else if (direction.x > 0.9)
			{
				SwitchGravity("right");
			}
			else if (direction.y < -0.9)
			{
				SwitchGravity("down");
			}
			else if (direction.y > 0.9)
			{
				SwitchGravity("up");
			}
		}

		private void SwitchGravity(string direction)
		{
			switch (direction)
			{
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

		private bool IsGrounded()
		{
			int layerMask = ~(3 << 8); // 0000000011 -> 1100000000
			RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f,
				-myVectorUp, 0.1f, layerMask);
			return (raycast.collider != null);
		}

		private Vector2 VecAbs(Vector2 a)
		{
			a.x = Mathf.Abs(a.x);
			a.y = Mathf.Abs(a.y);
			return a;
		}

		public void PortalChangeVelocityDirection(Vector3 portal1Direction, Vector3 portal2Direction)
		{
			var angle1 = Mathf.Atan2(portal1Direction.y, portal1Direction.x) * Mathf.Rad2Deg;
			var angle2 = Mathf.Atan2(portal2Direction.y, portal2Direction.x) * Mathf.Rad2Deg;
			var rotationDiff = angle2 - angle1;

			var velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, 0);
			velocity = Quaternion.Euler(0, 0, rotationDiff + 180) *
			           velocity; // rotate velocity vector for portal rotation diff
			velocity = Vector2.Reflect(velocity,
				Quaternion.Euler(0f, 0f, 90f) *
				portal2Direction); // reflect velocity vector over direction vector of portal2
			rigidBody.velocity = velocity;
		}

		public void SetDoubleJumped(bool value)
		{
			doubleJumped = value;
		}
	}
}