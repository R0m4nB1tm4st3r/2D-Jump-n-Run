using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	#region Inspector Settings
	[SerializeField] 
        float moveSpeed = 10, jumpStrength = 12;
	[SerializeField, Range(0.02f, 0.06f)]
		float accelerationFactor = 0.05f;
	[SerializeField]
	    Rigidbody2D rigidBody = null;
	[SerializeField]
		bool multiJumpsAllowed = false;
	[SerializeField, Range(1, 3)]
		int extraJumps = 1;
	#endregion

	#region Const Members

	const string GROUND_TAG = "Ground";
	const int MOVEMENT_SCALE_FACTOR = 35;
	const float COYOTE_TIME = 0.5f;

	#endregion

	#region Private Members

	bool _isOnGround, _isJumping = false, _isMoving = false;
	float _targetXVelocity, _currentXVelocity = 0.0f;
    @_2DJumpnRun _inputAction = null;
	int _jumpCount = 2;
	IEnumerator _coroutine;

	#endregion

	#region Lifecycle Methods
	private void Awake()
	{
		_inputAction = new();
	}

	// Start is called before the first frame update
	void Start()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
		_coroutine = FallAfterCoyoteTime();
	}

	private void FixedUpdate()
	{
		_currentXVelocity = _isMoving ? _currentXVelocity + _targetXVelocity * accelerationFactor : rigidBody.velocity.x;
		_currentXVelocity = _isMoving ? Mathf.Clamp(_currentXVelocity, Mathf.Min(_targetXVelocity, 0), Mathf.Max(_targetXVelocity, 0)) : _currentXVelocity;
		rigidBody.velocity = new Vector2(_currentXVelocity, rigidBody.velocity.y);
	}

	private void OnEnable()
	{
		_inputAction.Enable();
        _inputAction.Player.Move.performed += OnMove;
		_inputAction.Player.Move.canceled += OnMoveCanceled;
		_inputAction.Player.Jump.performed += OnJump;
	}

	private void OnDisable()
	{
        _inputAction.Player.Move.performed -= OnMove;
		_inputAction.Player.Move.canceled -= OnMoveCanceled;
		_inputAction.Player.Jump.performed -= OnJump;
		_inputAction.Disable();
	}
	#endregion

	#region Action Handlers
	public void OnMove(InputAction.CallbackContext context)
	{
		_targetXVelocity = context.ReadValue<Vector2>().x * moveSpeed * Time.fixedDeltaTime * MOVEMENT_SCALE_FACTOR;
		_isMoving = true;
	}

	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		_targetXVelocity = 0;
		_isMoving= false;
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (_isOnGround || (multiJumpsAllowed && _jumpCount > 0))
		{
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, context.ReadValue<Vector2>().y * jumpStrength * Time.fixedDeltaTime * MOVEMENT_SCALE_FACTOR);
			_jumpCount--;
			_isJumping = true;

			if (_isOnGround)
				Debug.Log("Jumping from ground!!!");

			_isOnGround = false;
			StopCoroutine(_coroutine);
		}
	}
	#endregion

	#region Collision Handlers
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag(GROUND_TAG))
        {
			_isOnGround = true;
			_isJumping = false;
			_jumpCount = multiJumpsAllowed ? extraJumps + 1 : 1;
			StopCoroutine(_coroutine);
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(GROUND_TAG) && !_isJumping)
		{
			_coroutine = FallAfterCoyoteTime();
			StartCoroutine(_coroutine);
		}
			
	}
	#endregion

	#region Coroutines

	IEnumerator FallAfterCoyoteTime()
	{
		Debug.Log("Start Coyote Time!!");
		yield return new WaitForSeconds(COYOTE_TIME);
		Debug.Log("End Coyote Time!!");
		
		if (_isOnGround)
		{
			_isOnGround = false;
			_jumpCount--;
		}
	}

	#endregion
}

