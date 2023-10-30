using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitDialogueHandler : MonoBehaviour
{
	@_2DJumpnRun inputAction = null;

	void Awake()
	{
		inputAction = new _2DJumpnRun();
	}

	void OnEnable()
	{
		inputAction.Enable();
		inputAction.UI.ConfirmQuit.performed += ConfirmQuitDialogue;
		inputAction.UI.CancelQuit.performed += CancelQuitDialogue;
	}

	void OnDisable()
	{
		inputAction.UI.ConfirmQuit.performed -= ConfirmQuitDialogue;
		inputAction.UI.CancelQuit.performed -= CancelQuitDialogue;
		inputAction.Disable();
	}

	void ConfirmQuitDialogue(InputAction.CallbackContext context)
	{
		Application.Quit();
	}

	void CancelQuitDialogue(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
	}

	public void OnClickNo()
    {
        gameObject.SetActive(false);
    }

    public void OnClickYes()
    {
        Application.Quit();
    }
}
