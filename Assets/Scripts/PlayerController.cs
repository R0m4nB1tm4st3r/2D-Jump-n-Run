using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	#region Const Members

	const string _GroundTag = "Ground";
	const string _CoyoteTimeStartMsg = "Start Coyote Time!!";
	const string _CoyoteTimeEndMsg = "End Coyote Time!!";
	const float _CoyoteTime = 0.2f;

	#endregion

	#region Private Members

	[SerializeField]
	float moveSpeed = 10, jumpStrength = 10;
	[SerializeField, Range(0.002f, 0.01f)]
	float accelerationFactor = 0.01f;
	[SerializeField]
	Rigidbody2D rigidBody = null;
	[SerializeField]
	bool multiJumpsAllowed = false;
	[SerializeField, Range(1, 3)]
	int extraJumps = 1;

	bool _isOnGround, _isJumping = false;
	float _targetXVelocity, _currentXVelocity = 0.0f;
    @_2DJumpnRun _inputAction = null;
	int _jumpCount = 2;
	IEnumerator _moveCoroutine = null;
	IEnumerator _coyoteCoroutine = null;

	#endregion

	#region Lifecycle Methods

	void Awake()
	{
		_inputAction = new();
	}

	void Start()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
	}

	void OnEnable()
	{
		_inputAction.Enable();
        _inputAction.Player.Move.performed += OnMove;
		_inputAction.Player.Move.canceled += OnMoveCanceled;
		_inputAction.Player.Jump.performed += OnJump;
	}

	void OnDisable()
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
		_targetXVelocity = context.ReadValue<Vector2>().x * moveSpeed;
		_moveCoroutine = MovePlayer();
		StartCoroutine(_moveCoroutine);
	}

	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (_isOnGround || (multiJumpsAllowed && _jumpCount > 0))
		{
			rigidBody.velocity = new Vector2(
				x: rigidBody.velocity.x, 
				y: context.ReadValue<Vector2>().y * jumpStrength);
			_jumpCount--;
			_isJumping = true;

			_isOnGround = false;

			if (_coyoteCoroutine != null)
				StopCoroutine(_coyoteCoroutine);
		}
	}

	#endregion

	#region Collision Handlers

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag(_GroundTag))
        {
			_isOnGround = true;
			_isJumping = false;
			_jumpCount = multiJumpsAllowed ? extraJumps + 1 : 1;

			if (_coyoteCoroutine != null)
				StopCoroutine(_coyoteCoroutine);
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(_GroundTag) && !_isJumping)
		{
			_coyoteCoroutine = FallAfterCoyoteTime();
			StartCoroutine(_coyoteCoroutine);
		}
			
	}

	#endregion

	#region Coroutines

	IEnumerator MovePlayer()
	{
		float newVelocityX;
		while (true)
		{
			newVelocityX = _currentXVelocity + _targetXVelocity * accelerationFactor;
			_currentXVelocity = Mathf.Clamp(newVelocityX, Mathf.Min(_targetXVelocity, 0), Mathf.Max(_targetXVelocity, 0));
			rigidBody.velocity = new Vector2(_currentXVelocity, rigidBody.velocity.y);
			yield return null;
		}
	}

	IEnumerator FallAfterCoyoteTime()
	{
		Debug.Log(_CoyoteTimeStartMsg);
		yield return new WaitForSeconds(_CoyoteTime);
		Debug.Log(_CoyoteTimeEndMsg);
		
		if (_isOnGround)
		{
			_isOnGround = false;
			_jumpCount--;
		}
	}

	#endregion
}

