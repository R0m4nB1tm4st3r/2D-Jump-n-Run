using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Respawner : MonoBehaviour
{
	const byte RespawnPositionChildIndex = 0;
	const string PlayerTag = "Player";

	Vector2 respawnPosition = Vector2.zero;
	GameObject player = null;
	@_2DJumpnRun inputAction = null;

	void Awake()
	{
		respawnPosition = transform.GetChild(RespawnPositionChildIndex).position;
		player = GameObject.FindGameObjectWithTag(PlayerTag);
		inputAction = new _2DJumpnRun();
	}

	void OnEnable()
	{
		inputAction.Enable();
		inputAction.Player.Respawn.performed += RespawnPlayer;
	}

	void OnDisable()
	{
		inputAction.Player.Respawn.performed -= RespawnPlayer;
		inputAction.Disable();
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag(PlayerTag))
		{
			player.transform.position = respawnPosition;
		}
	}

	void RespawnPlayer(InputAction.CallbackContext context)
	{
		player.transform.position = respawnPosition;
	}
}
