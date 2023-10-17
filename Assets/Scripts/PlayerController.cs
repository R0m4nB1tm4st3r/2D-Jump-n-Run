using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	#region Const Members

	const string GroundTag = "Ground";
	const string CoyoteTimeStartMsg = "Start Coyote Time!!";
	const string CoyoteTimeEndMsg = "End Coyote Time!!";
	const string AnimationParamIsRunning = "isRunning";
	const string AnimationParamIsJumping = "isJumping";
	const string AnimationParamIsInMidAir = "isInMidAir";
	const string AnimationStateJumping = "Player_Jumping";
	const float CoyoteTime = 0.2f;
	const float DefaultDrag = 1.0f;
	const float StopMovementDrag = 200.0f;

	#endregion

	#region Private Members

	[SerializeField]
	float moveSpeed = 10, jumpStrength = 10;
	[SerializeField, Range(0.09f, 0.4f)]
	float accelerationFactor = 0.2f;
	[SerializeField]
	Rigidbody2D rigidBody = null;
	[SerializeField]
	bool multiJumpsAllowed = false;
	[SerializeField, Range(1, 3)]
	int extraJumps = 1;

	bool isOnGround, isJumping = false;
	float targetXVelocity;
    @_2DJumpnRun inputAction = null;
	int jumpCount = 2;
	IEnumerator moveCoroutine = null;
	IEnumerator coyoteCoroutine = null;
	Animator animator = null;
	SpriteRenderer spriteRenderer = null;

	#endregion

	#region Lifecycle Methods

	void Awake()
	{
		inputAction = new();
	}

	void Start()
    {
        if (rigidBody == null) rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		inputAction.Enable();
        inputAction.Player.Move.performed += OnMove;
		inputAction.Player.Move.canceled += OnMoveCanceled;
		inputAction.Player.Jump.performed += OnJump;
	}

	void OnDisable()
	{
        inputAction.Player.Move.performed -= OnMove;
		inputAction.Player.Move.canceled -= OnMoveCanceled;
		inputAction.Player.Jump.performed -= OnJump;
		inputAction.Disable();
	}

	#endregion

	#region Action Handlers

	public void OnMove(InputAction.CallbackContext context)
	{
		animator.SetBool(AnimationParamIsRunning, true);

		targetXVelocity = context.ReadValue<Vector2>().x * moveSpeed;
		spriteRenderer.flipX = targetXVelocity < 0;

		moveCoroutine = MovePlayer();
		StartCoroutine(moveCoroutine);
	}

	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		if (moveCoroutine != null) StopCoroutine(moveCoroutine);

		Vector2 velocity = rigidBody.velocity;
		velocity.x = targetXVelocity * Time.fixedDeltaTime;
		rigidBody.velocity = velocity;

		animator.SetBool(AnimationParamIsRunning, false);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (isOnGround || (multiJumpsAllowed && jumpCount > 0))
		{
			animator.SetBool(AnimationParamIsInMidAir, true);
			animator.Play(AnimationStateJumping);

			rigidBody.velocity = new Vector2(
				x: rigidBody.velocity.x, 
				y: context.ReadValue<Vector2>().y * jumpStrength);
			jumpCount--;
			isJumping = true;

			isOnGround = false;

			if (coyoteCoroutine != null)
				StopCoroutine(coyoteCoroutine);
		}
	}

	#endregion

	#region Collision Handlers

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag(GroundTag))
        {
			animator.SetBool(AnimationParamIsInMidAir, false);

			isOnGround = true;
			isJumping = false;
			jumpCount = multiJumpsAllowed ? extraJumps + 1 : 1;

			if (coyoteCoroutine != null)
				StopCoroutine(coyoteCoroutine);
        }
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(GroundTag) && !isJumping)
		{
            animator.SetBool(AnimationParamIsInMidAir, true);

            coyoteCoroutine = FallAfterCoyoteTime();
			StartCoroutine(coyoteCoroutine);
		}
			
	}

	#endregion

	#region Coroutines

	IEnumerator MovePlayer()
	{
		Vector2 velocity;
		while (true)
		{
			velocity = rigidBody.velocity;
			velocity.x = Mathf.Clamp(	velocity.x + targetXVelocity * accelerationFactor,
										Mathf.Min(targetXVelocity, 0), Mathf.Max(targetXVelocity, 0));
			rigidBody.velocity = velocity;
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator FallAfterCoyoteTime()
	{
		Debug.Log(CoyoteTimeStartMsg);
		yield return new WaitForSeconds(CoyoteTime);
		Debug.Log(CoyoteTimeEndMsg);
		
		if (isOnGround)
		{
			isOnGround = false;
			jumpCount--;
		}
	}

	#endregion
}

