using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
	const int QuitDialoguePanelIndex = 1;

	@_2DJumpnRun inputAction = null;
	GameObject quitDialoguePanel = null;

	void Awake()
	{
		inputAction = new _2DJumpnRun();
	}

	private void Start()
	{
		quitDialoguePanel = FindObjectOfType<Canvas>().transform.GetChild(QuitDialoguePanelIndex).gameObject;
	}

	void OnEnable()
	{
		inputAction.Enable();
		inputAction.UI.Quit.performed += QuitGame;
	}

	void OnDisable()
	{
		inputAction.UI.Quit.performed -= QuitGame;
		inputAction.Disable();
	}

	void QuitGame(InputAction.CallbackContext context)
	{
		// Application.Quit();
		quitDialoguePanel.SetActive(true);
	}
}
