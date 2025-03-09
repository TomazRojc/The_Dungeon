using Code.Utils;
using UnityEngine;

namespace Code
{
	public class PlayerController : MonoBehaviour
	{
		private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
		private static readonly int AlbedoColor = Shader.PropertyToID("_Color");

		[Header("References")]
		[SerializeField]
		private BoxCollider2D _boxCollider;
		[SerializeField]
		private Rigidbody2D rigidBody;
		[SerializeField]
		private TrailRenderer trailRenderer;
		[SerializeField]
		private SpriteRenderer spriteRenderer;
		[SerializeField]
		private Transform _raycastPosition;
		
		[Header("Parameters")]
		[SerializeField]
		private float _myGravityScale;
		[SerializeField]
		private float moveSpeed = 15f;
		[SerializeField]
		private float jumpVelocity = 30f;
		[SerializeField]
		private float speedChangeFactor = 5f;
		[SerializeField]
		private float superSonicSpeedChangeFactor = 5f;
		[SerializeField]
		private float dashSpeed = 100f;
		[SerializeField]
		private float dashFullTime = 0.1f;
		[SerializeField]
		private float dashCooldown = 0.4f;
		[SerializeField]
		private AnimationCurve _dashSpeedCurve;
		[SerializeField]
		private LayerMask _raycastLayer;
		
		public bool CanTeleport => _justTeleportedToPortal == null;
		private bool IsDashing => _dashTimer.IsPlaying();
		
		private bool _doubleJumped = false;
		private bool stunned = false;
		private SimpleTimer _dashTimer;
		private bool _canDash = true;
		private float _currentDashCooldown;
		private bool _grounded;
		private Portal _justTeleportedToPortal;
		private Color _blackZeroAlpha = new Color(0f, 0f, 0f, 0f);
		
		// control variables (on keypress)
		private Vector2 _currentMoveInput;

		private Vector2 _myVectorUp = new Vector2(0, 1);
		private Vector2 _myVectorRight = new Vector2(1, 0);
		
		private Vector2 _boxCastSize;

		private void Awake()
		{
			_boxCastSize = new Vector2(_boxCollider.bounds.size.x * 0.99f, 0.01f);
		}

		public void Init(Color color)
		{
			spriteRenderer.color = color;
			trailRenderer.material.SetColor(EmissionColor, color);
			trailRenderer.material.SetColor(AlbedoColor, _blackZeroAlpha);
			
			_dashTimer.OnUpdate += UpdateDashing;
			_dashTimer.OnStart += () => {
				_canDash = false;
				trailRenderer.material.SetColor(AlbedoColor, Color.black);
			};
			_dashTimer.OnComplete += StopDashing;
		}
		
		public void OnSpawn()
		{
			SwitchGravity("down");
		}

		void FixedUpdate()
		{
			_dashTimer.Update(Time.fixedDeltaTime);
			_currentDashCooldown -= Time.fixedDeltaTime;
			
			_grounded = IsGrounded();
			if (_grounded)
			{
				// jump available
				_doubleJumped = false;
			}
			if (_grounded && !IsDashing && _currentDashCooldown <= 0f)
			{
				// dash available
				_canDash = true;
			}
			
			if (!IsDashing)
			{
				UpdateLeftRightMovement();
			}


			// Gravity
			rigidBody.AddForce(-_myVectorUp * _myGravityScale, ForceMode2D.Force);

			UpdatePortalCheck();
		}
		
		private void UpdateDashing(float normalizedTime) {
			//TODO: tomazr what to do if gravity is switched?
			// rigidBody.velocity = _myVectorUp * dashSpeed;
			
			var eval  = _dashSpeedCurve.Evaluate(normalizedTime);
			rigidBody.velocity = _currentMoveInput.normalized * (dashSpeed * eval);
			
			var currentColor = Color.Lerp(Color.black, _blackZeroAlpha, normalizedTime);
			trailRenderer.material.SetColor(AlbedoColor, currentColor);
		}

		private void StopDashing() {
			_dashTimer.Stop();
			trailRenderer.material.SetColor(AlbedoColor, _blackZeroAlpha);
		}

		private void UpdateLeftRightMovement() {
			var factor = speedChangeFactor;
			var wantsToStop = Mathf.Abs(_currentMoveInput.x) > 0.1f && !MathUtils.HasSameSign(_currentMoveInput.x, (rigidBody.velocity * _myVectorRight).x);
			var isMovingFast = (rigidBody.velocity * _myVectorRight).magnitude > moveSpeed;
			if (isMovingFast && !wantsToStop) {
				factor =  superSonicSpeedChangeFactor;
			}
			var targetVelocity = (_myVectorRight * (_currentMoveInput.x * moveSpeed)) + (VecAbs(_myVectorUp) * rigidBody.velocity);
			rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, targetVelocity, Time.deltaTime * factor);
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
				Jump();
			}

			if (!_grounded && !_doubleJumped && !stunned)
			{
				DoubleJump();
			}
		}
		
		public void HandleDashInput()
		{
			if (_canDash && !IsDashing && _currentMoveInput != Vector2.zero && !stunned) {
				_dashTimer.Start(dashFullTime);
				_currentDashCooldown = dashCooldown;
			}
		}
		
		public void HandleMoveInput(Vector2 direction)
		{
			if (!IsDashing)
			{
				_currentMoveInput = direction;
			}
			else if (stunned)
			{
				_currentMoveInput = Vector2.zero;
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

		private void Jump() {
			rigidBody.velocity = _myVectorUp * jumpVelocity;
		}
		
		private void DoubleJump() {
			_doubleJumped = true;
			Jump();
		}

		private bool IsGrounded()
		{
			RaycastHit2D raycast = Physics2D.BoxCast(_raycastPosition.position, _boxCastSize, 0f,
				-_myVectorUp, 0, _raycastLayer);
			return raycast.collider != null;
		}

		private Vector2 VecAbs(Vector2 a)
		{
			a.x = Mathf.Abs(a.x);
			a.y = Mathf.Abs(a.y);
			return a;
		}

		public void TeleportPlayer(Vector3 portal1Direction, Vector3 portal2Direction, Portal portal2)
		{
			StopDashing();
			_canDash = true;
			_doubleJumped = false;
			_justTeleportedToPortal = portal2;
			
			var angle1 = Mathf.Atan2(portal1Direction.y, portal1Direction.x) * Mathf.Rad2Deg;
			var angle2 = Mathf.Atan2(portal2Direction.y, portal2Direction.x) * Mathf.Rad2Deg;
			var rotationDiff = angle2 - angle1;

			var velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, 0);
			velocity = Quaternion.Euler(0, 0, rotationDiff + 180) * velocity; // rotate velocity vector for portal rotation diff
			velocity = Vector2.Reflect(velocity, Quaternion.Euler(0f, 0f, 90f) * portal2Direction); // reflect velocity vector over direction vector of portal2
			rigidBody.velocity = velocity;
		}
	}
}