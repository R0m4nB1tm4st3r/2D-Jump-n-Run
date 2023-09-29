using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] 
        float moveSpeed = 10, jumpStrength = 9;
	[SerializeField]
	    private Rigidbody2D rigidBody = null;

	bool isOnGround;
	float xDelta;
    @_2DJumpnRun inputAction = null;

	private void Awake()
	{
		inputAction = new();
	}

	// Start is called before the first frame update
	void Start()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
		rigidBody.velocity = new Vector2 (xDelta, rigidBody.velocity.y);
	}

	private void OnEnable()
	{
		inputAction.Enable();
        inputAction.Player.Move.performed += OnMove;
		inputAction.Player.Move.canceled += OnMoveCanceled;
		inputAction.Player.Jump.performed += OnJump;
	}

	private void OnDisable()
	{
        inputAction.Player.Move.performed -= OnMove;
		inputAction.Player.Move.canceled -= OnMoveCanceled;
		inputAction.Player.Move.performed -= OnJump;
		inputAction.Disable();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Debug.Log($"MOOVIIINNGG!!");
		
		xDelta = context.ReadValue<Vector2>().x * moveSpeed;
	}

	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		Debug.Log($"MOOVEE CANCEELEEDD!!");

		xDelta = 0;
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		Debug.Log("JUUMMPIIINNG!!");

		if (isOnGround)
			rigidBody.velocity += new Vector2(0, context.ReadValue<Vector2>().y * jumpStrength);
	}
     
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Ground"))
        {
			isOnGround = true;
            Debug.Log("Player has landed on ground.");
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
        isOnGround = false;
        Debug.Log("Player has left the ground.");
	}
}
