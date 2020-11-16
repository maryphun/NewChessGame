using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CursorController : MonoBehaviour
{
    [SerializeField] private float cursorMoveSpeed = 0.25f;
    [SerializeField] private float buttonHoldMoveInterval = 0.15f;
    [SerializeField] private float buttonHoldMoveTime = 0.8f;

    [Header("Debug")]
    [SerializeField] private bool ControlOn = true;

    private Vector2 currentPosition, moveVector;
    private BoardArray board;
    private bool buttonClicked;
    private float buttonHoldTime;
    private CursorInput input = null;
    private ChessPieceProperties lockedOnPiece;   // store the chess piece that you have locked to move or attack

    private CursorInput Input
    {
        get
        {
            if (input != null) { return input; }
            return input = new CursorInput();
        }
    }

    private void Awake()
    {
        board = BoardArray.Instance();

        // register movement key
        Input.Default.MoveVertical.performed += ctx => MoveCursor(new Vector2(0f, ctx.ReadValue<float>()));
        Input.Default.MoveHorizontal.performed += ctx => MoveCursor(new Vector2(ctx.ReadValue<float>(), 0f));
        Input.Default.MoveVertical.canceled += _ => buttonClicked = false;
        Input.Default.MoveHorizontal.canceled += _ => buttonClicked = false;
        Input.Default.Confirm.performed += _ => Confirm();
        Input.Default.Cancel.performed += _ => Cancel();

        if (!ControlOn)
        {
            Input.Disable();
        }

        currentPosition = new Vector2(4, 4);
        lockedOnPiece = null;
    }

    private void OnEnable()
    {
        // ControlOn is used for debugging.
        if (ControlOn)
        {
            Input.Enable();
        }
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    private void Update()
    {
        if (buttonClicked)
        {
            buttonHoldTime += Time.deltaTime;

            if (buttonHoldTime > buttonHoldMoveTime)
            {
                MoveCursor(moveVector);
                buttonHoldTime = buttonHoldMoveTime - buttonHoldMoveInterval;
            }
        }
    }

    private void Confirm()
    {
        GameObject tmp = board.GetTilePieceAt((int)currentPosition.y, (int)currentPosition.x);
        if (tmp != null)
        {
            ChessPieceProperties piece = tmp.GetComponent<ChessPieceProperties>();
            // check if the player have already locked on a chess piece
            if (lockedOnPiece != null)
            {
                // check if it is the same chess piece
                if (piece == lockedOnPiece)
                {
                    CancelLockOn(piece);
                }
            }
            else
            {
                piece.LockOn(true);
                // store this piece into memory
                lockedOnPiece = piece;

                // sound effect
                AudioManager.Instance.PlaySFX("lockOn", 0.75f);
            }
        }
    }

    private void Cancel()
    {
        if (lockedOnPiece != null)
        {
            CancelLockOn(lockedOnPiece);
        }
    }

    private void CancelLockOn(ChessPieceProperties target)
    {
        // cancel lock on
        target.LockOn(false);
        // clear memory
        lockedOnPiece = null;

        // sound effect
        AudioManager.Instance.PlaySFX("cancellockon", 0.75f);
    }

    private void ButtonReleased()
    {
        buttonClicked = false;
        buttonHoldTime = 0.0f;
        moveVector = Vector2.zero;
    }

    private void MoveCursor(Vector2 value)
    {
        buttonClicked = true;
        moveVector = value;
        buttonHoldTime = 0.0f;

        // move position of the cursor
        Vector2 lastPosition = currentPosition;
        currentPosition = currentPosition + value;
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, 7);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, 7);
        
        Vector2 newPos = board.GetTileCenter((int)currentPosition.y, (int)currentPosition.x);
        transform.DOMove(newPos, cursorMoveSpeed, false);

        // sound effect
        AudioManager.Instance.PlaySFX("cursor", 0.05f);

        // select anad unselect pieces
        if (lastPosition != currentPosition)
        {
            GameObject piece = board.GetTilePieceAt((int)lastPosition.y, (int)lastPosition.x);
            if (piece != null)
            {
                piece.GetComponent<ChessPieceProperties>().Unselect(cursorMoveSpeed);
            }

            piece = board.GetTilePieceAt((int)currentPosition.y, (int)currentPosition.x);
            if (piece != null)
            {
                piece.GetComponent<ChessPieceProperties>().Select(cursorMoveSpeed);
            }
        }
    }
}
