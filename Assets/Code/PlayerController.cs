using System.Collections.Generic;
using UnityEngine;

namespace Code
{
	public class PlayerController : MonoBehaviour
	{
		[Header("References")]
		[SerializeField]
		private BoxCollider2D _boxCollider;
		[SerializeField]
		private Rigidbody2D rigidBody;
		
		[Header("Parameters")]
		[SerializeField]
		private float _myGravityScale;
		
		public bool CanTeleport => _justTeleportedToPortal == null;
		
		public float moveSpeed = 15f;
		public float jumpVelocity = 30f;
		public float speedChangeFactor = 5f;

		private bool _doubleJumped = false;
		private bool stunned = false;
		public float dashSpeed = 100f;
		public float dashFullTime = 0.1f;
		public float dashCooldown = 0.4f;

		private float _currentDashTime;
		private bool _canDash = true;
		private bool _isDashing;
		private bool _grounded;
		private float _lastMoveDirection = 1f;
		private Portal _justTeleportedToPortal;
		
		// control variables (on keypress)
		private bool _shouldJump;
		private bool _shouldDoubleJump;
		private bool _shouldDash;
		private float _moveLeftRight;

		private Vector2 _myVectorUp = new Vector2(0, 1);
		private Vector2 _myVectorRight = new Vector2(1, 0);

		void Start()
		{
			// GetComponent<SpriteRenderer>().color = displayColor;

			SwitchGravity("down");
			_currentDashTime = dashFullTime;
		}

		void FixedUpdate()
		{
			_grounded = IsGrounded();
			if (_grounded)
			{
				// jump/dash available
				_doubleJumped = false;
				_canDash = true;
			}
			
			if (_shouldJump)
			{
				rigidBody.velocity = _myVectorUp * jumpVelocity;
				_shouldJump = false;
			}

			if (_shouldDoubleJump)
			{
				rigidBody.velocity = _myVectorUp * jumpVelocity;
				_shouldDoubleJump = false;
				_doubleJumped = true;
				_canDash = true;
			}

			if (_shouldDash)
			{
				if (_lastMoveDirection > 0) rigidBody.velocity = _myVectorRight * dashSpeed;
				if (_lastMoveDirection < 0) rigidBody.velocity = -_myVectorRight * dashSpeed;
				_currentDashTime = dashFullTime;
				_isDashing = true;
				_shouldDash = false;
				_canDash = false;
			}
			else if (!_isDashing)
			{
				//TODO: tomazr slowly decrease left/right speed if controls are not being touched, otherwise give full control to player
				var targetVelocity = (_myVectorRight * _moveLeftRight) + (VecAbs(_myVectorUp) * rigidBody.velocity);
				rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, targetVelocity, Time.deltaTime * speedChangeFactor);
			}


			UpdateDashing();

			// Gravity
			rigidBody.AddForce(-_myVectorUp * _myGravityScale, ForceMode2D.Force);

			UpdatePortalCheck();
		}
		
		private void UpdateDashing() {
			// handle dashing
			_currentDashTime -= Time.fixedDeltaTime;
			if (_isDashing)
			{
				if (_currentDashTime <= 0)
				{
					// dash just ended
					_isDashing = false;
					rigidBody.velocity = VecAbs(_myVectorUp) * rigidBody.velocity;
				}
			}
		}

		private void UpdatePortalCheck() {
			if (_justTeleportedToPortal == null) return;
			
			if (!_boxCollider.bounds.Intersects(_justTeleportedToPortal.BoxCollider.bounds))
			{
				_justTeleportedToPortal = null;
			}
		}
		
		public void HandleJumpInput()
		{
			if (_grounded && !stunned)
			{
				_shouldJump = true;
			}

			if (!_grounded && !_doubleJumped && !stunned)
			{
				_shouldDoubleJump = true;
			}
		}
		
		public void HandleDashInput()
		{
			if (_canDash && _currentDashTime <= -dashCooldown && !stunned)
			{
				_shouldDash = true;
			}
		}
		
		public void HandleMoveInput(Vector2 direction)
		{
			if (!_isDashing && !stunned)
			{
				_moveLeftRight = direction.x * moveSpeed;
				_lastMoveDirection = _moveLeftRight;
			}
			else
			{
				_moveLeftRight = 0;
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
					_myVectorUp = new Vector2(0, 1);
					_myVectorRight = new Vector2(1, 0);
					break;
				case "left":
					_myVectorUp = new Vector2(1, 0);
					_myVectorRight = new Vector2(0, -1);
					break;
				case "up":
					_myVectorUp = new Vector2(0, -1);
					_myVectorRight = new Vector2(-1, 0);
					break;
				case "right":
					_myVectorUp = new Vector2(-1, 0);
					_myVectorRight = new Vector2(0, 1);
					break;
				default:
					// Invalid function input
					break;
			}
		}

		private bool IsGrounded()
		{
			int layerMask = ~(3 << 8); // 0000000011 -> 1100000000
			RaycastHit2D raycast = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f,
				-_myVectorUp, 0.1f, layerMask);
			return (raycast.collider != null);
		}

		private Vector2 VecAbs(Vector2 a)
		{
			a.x = Mathf.Abs(a.x);
			a.y = Mathf.Abs(a.y);
			return a;
		}

		public void TeleportPlayer(Vector3 portal1Direction, Vector3 portal2Direction, Portal portal2)
		{
			_justTeleportedToPortal = portal2;
			
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
			_doubleJumped = value;
		}
	}
}