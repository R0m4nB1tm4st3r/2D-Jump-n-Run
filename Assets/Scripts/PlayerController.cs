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
	#endregion

	#region Const Members
	// const members
	#endregion

	#region Private Members
	bool _isOnGround;
	float _xDelta;
    @_2DJumpnRun _inputAction = null;
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
		if (_isOnGround)
			rigidBody.velocity += new Vector2(0, context.ReadValue<Vector2>().y * jumpStrength);
	}
	#endregion

	#region Collision Handlers
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Ground"))
        {
			_isOnGround = true;
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
        _isOnGround = false;
	}
	#endregion
}

