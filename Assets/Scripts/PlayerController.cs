using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	#region Inspector Settings
	[SerializeField] 
        float moveSpeed = 10, jumpStrength = 9;
	[SerializeField]
	    Rigidbody2D rigidBody = null;
	[SerializeField]
		bool multiJumpsAllowed = false;
	[SerializeField, Range(1, 3)]
		int extraJumps = 1;
	#endregion

	#region Const Members
	string GROUND_TAG = "Ground";
	#endregion

	#region Private Members
	bool _isOnGround, _isJumping = false;
	float _xDelta;
    @_2DJumpnRun _inputAction = null;
	int _jumpCount = 2;
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
	}

    // Update is called once per frame
    void Update()
    {
		rigidBody.velocity = new Vector2(_xDelta, rigidBody.velocity.y);
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
		_xDelta = context.ReadValue<Vector2>().x * moveSpeed;
	}

	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		_xDelta = 0;
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (_isOnGround || (multiJumpsAllowed && _jumpCount > 0))
		{
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, context.ReadValue<Vector2>().y * jumpStrength);
			_jumpCount--;
			_isJumping = true;
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
			_jumpCount = multiJumpsAllowed ? extraJumps : 1;
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(GROUND_TAG))
		{
			_isOnGround = false;
			if (!_isJumping) _jumpCount--;
		}
	}
	#endregion
}

