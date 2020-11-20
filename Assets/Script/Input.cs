using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script need CursorController to work
[RequireComponent(typeof(CursorController))]
public class Input : MonoBehaviour
{
    [SerializeField] private float buttonHoldMoveInterval = 0.15f;
    [SerializeField] private float buttonHoldMoveTime = 0.8f;

    [SerializeField] private bool controlOn = true;

    private bool buttonClicked;
    private CursorInput input = null;
    private float buttonHoldTime;
    CursorController controller;

    private CursorInput KeyInput
    {
        get
        {
            if (input != null) { return input; }
            return input = new CursorInput();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // register movement key
        KeyInput.Default.MoveVertical.performed += ctx => MoveButtonClicked(new Vector2Int(0, (int)ctx.ReadValue<float>()));
        KeyInput.Default.MoveHorizontal.performed += ctx => MoveButtonClicked(new Vector2Int((int)ctx.ReadValue<float>(), 0));
        KeyInput.Default.MoveVertical.canceled += _ => MoveButtonReleased();
        KeyInput.Default.MoveHorizontal.canceled += _ => MoveButtonReleased();
        KeyInput.Default.Confirm.performed += _ => controller.Confirm();
        KeyInput.Default.Cancel.performed += _ => controller.Cancel();

        if (!controlOn)
        {
            KeyInput.Disable();
        }

        controller = GetComponent<CursorController>();
    }

    private void OnEnable()
    {
        // ControlOn is used for debugging.
        if (controlOn)
        {
            KeyInput.Enable();
        }
    }

    private void OnDisable()
    {
        KeyInput.Disable();
    }

    /// <summary>
    /// enable and disable key control to take effect pon this cursor
    /// </summary>
    public void SetControlActive(bool boolean)
    {
        if (boolean)
        {
            KeyInput.Enable();
            return;
        }
        KeyInput.Disable();
    }


    private void Update()
    {
        if (buttonClicked)
        {
            buttonHoldTime += Time.deltaTime;

            if (buttonHoldTime > buttonHoldMoveTime)
            {
                controller.MoveCursor(controller.MoveVector);
                buttonHoldTime = buttonHoldMoveTime - buttonHoldMoveInterval;
            }
        }
    }

    private void MoveButtonReleased()
    {
        buttonClicked = false;
        buttonHoldTime = 0.0f;
        controller.MoveVectorReset();
    }

    /// <summary>
    /// arrow key or WASD or any that move the cursor only
    /// </summary>
    private void MoveButtonClicked(Vector2Int value)
    {
        buttonClicked = true;
        buttonHoldTime = 0.0f;
        controller.MoveCursor(value);
    }
}
