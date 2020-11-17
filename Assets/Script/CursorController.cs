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

    private Vector2Int currentPosition, moveVector;
    private BoardArray board;
    private bool buttonClicked;
    private float buttonHoldTime;
    private CursorInput input = null;
    private ChessPieceProperties lockedOnPiece;   // store the chess piece that you have locked to move or attack
    private TileIndex lockedOnPieceIndex;
    private List<Transform> validMoveVisualList;
    private Transform hoveringValidMove;

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
        validMoveVisualList = new List<Transform>();

        // register movement key
        Input.Default.MoveVertical.performed += ctx => MoveCursor(new Vector2Int(0, (int)ctx.ReadValue<float>()));
        Input.Default.MoveHorizontal.performed += ctx => MoveCursor(new Vector2Int((int)ctx.ReadValue<float>(), 0));
        Input.Default.MoveVertical.canceled += _ => buttonClicked = false;
        Input.Default.MoveHorizontal.canceled += _ => buttonClicked = false;
        Input.Default.Confirm.performed += _ => Confirm();
        Input.Default.Cancel.performed += _ => Cancel();

        if (!ControlOn)
        {
            Input.Disable();
        }

        currentPosition = new Vector2Int(4, 4);
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
        // check if player is trying to move locked-on chess piece to this index
        if (hoveringValidMove != null)
        {
            // move the selected chess piece to this position index
            Vector2 newPiecePosition = MovementManager.Instance().MoveChessPiece(lockedOnPiece.gameObject, lockedOnPieceIndex,
                new TileIndex(currentPosition.y, currentPosition.x));

            // move the chess piece graphic
            lockedOnPiece.transform.DOMove(newPiecePosition, cursorMoveSpeed, false);

            // canccel the lock on
            lockedOnPiece.LockOn(false);

            // remove all visualization of valid move
            foreach (Transform validMoveVisual in validMoveVisualList)
            {
                // remove it immediately only if it's not the moving target. 
                float removeTime = validMoveVisual == hoveringValidMove ? cursorMoveSpeed : 0f;
                Destroy(validMoveVisual.gameObject, removeTime);
            }

            // reset list 
            validMoveVisualList.Clear();
            hoveringValidMove = null;

            // clear memory
            lockedOnPiece = null;

            // end of this function
            return;
        }

        // check if there is a chess piece on selected index
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
                lockedOnPieceIndex = new TileIndex(currentPosition.y, currentPosition.x);

                // sound effect
                AudioManager.Instance.PlaySFX("lockOn", 0.75f);

                // get all valid move for this chess piece
                var validMoves = MovementManager.Instance().logic.GetValidMoves(new TileIndex(currentPosition.y, currentPosition.x));

                Debug.Log("there are " + validMoves.Count + " moves. [" + lockedOnPiece.gameObject.name + " at " +  currentPosition + "]");

                // visualize valid move
                foreach (TileIndex validMove in validMoves)
                {
                    var validMoveVisual = new GameObject(lockedOnPiece.gameObject.name + "'s possible move");
                    validMoveVisual.transform.position = board.GetTileCenter(currentPosition.y, currentPosition.x) + lockedOnPiece.GraphicPosition;
                    validMoveVisual.transform.DOMove(board.GetTileCenter(validMove.row, validMove.col) + lockedOnPiece.GraphicPosition, cursorMoveSpeed, false);
                    var newRenderer = validMoveVisual.AddComponent<SpriteRenderer>();
                    var originRenderer = lockedOnPiece.SpriteRenderer;
                    newRenderer.sprite = originRenderer.sprite;
                    newRenderer.color = new Color(originRenderer.color.r, originRenderer.color.g, originRenderer.color.b, originRenderer.color.a / 4f);
                    newRenderer.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
                    newRenderer.sortingOrder = originRenderer.sortingOrder;

                    newRenderer.transform.localScale = originRenderer.transform.localScale;
                    validMoveVisualList.Add(validMoveVisual.transform);
                    hoveringValidMove = null;
                }
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

        // remove all visualization of valid move
        foreach (Transform validMoveVisual in validMoveVisualList)
        {
            validMoveVisual.DOMove(((Vector2)lockedOnPiece.transform.position) + lockedOnPiece.GraphicPosition, cursorMoveSpeed, false);
            Destroy(validMoveVisual.gameObject, cursorMoveSpeed);
        }

        // reset this list 
        validMoveVisualList.Clear();
        hoveringValidMove = null;

        // clear memory
        lockedOnPiece = null;

        // sound effect
        AudioManager.Instance.PlaySFX("cancellockon", 0.75f);
    }

    private void ButtonReleased()
    {
        buttonClicked = false;
        buttonHoldTime = 0.0f;
        moveVector = Vector2Int.zero;
    }

    private void MoveCursor(Vector2Int value)
    {
        buttonClicked = true;
        moveVector = value;
        buttonHoldTime = 0.0f;

        // move position of the cursor
        Vector2Int lastPosition = currentPosition;
        currentPosition = currentPosition + value;
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, 7);
        currentPosition.y = Mathf.Clamp(currentPosition.y, 0, 7);

        Vector2 newPos = board.GetTileCenter(currentPosition.y, currentPosition.x);
        transform.DOMove(newPos, cursorMoveSpeed, false);

        // sound effect
        AudioManager.Instance.PlaySFX("cursor", 0.05f);

        // select anad unselect pieces
        if (lastPosition != currentPosition)
        {
            GameObject piece = board.GetTilePieceAt(lastPosition.y, lastPosition.x);
            if (piece != null)
            {
                piece.GetComponent<ChessPieceProperties>().Unselect(cursorMoveSpeed);
            }

            piece = board.GetTilePieceAt(currentPosition.y, currentPosition.x);
            if (piece != null)
            {
                piece.GetComponent<ChessPieceProperties>().Select(cursorMoveSpeed);
            }
        }

        // if this is a movable cell for locked-on chess
        if (lockedOnPiece != null)
        {
            if (hoveringValidMove != null)
            {
                var renderer = hoveringValidMove.GetComponent<SpriteRenderer>().color;
                renderer.a = lockedOnPiece.SpriteRenderer.color.a / 4f;
                hoveringValidMove.GetComponent<SpriteRenderer>().color = renderer;
                hoveringValidMove = null;
            }

            foreach (Transform validMoveVisual in validMoveVisualList)
            {
                // check for new cell (new position index)
                Vector2 tmp = board.GetTileCenter(currentPosition.y, currentPosition.x) + lockedOnPiece.GraphicPosition;
                if (tmp == ((Vector2)validMoveVisual.position))
                {
                    hoveringValidMove = validMoveVisual;
                    var renderer = hoveringValidMove.GetComponent<SpriteRenderer>().color;
                    renderer.a = lockedOnPiece.SpriteRenderer.color.a / 2f;
                    hoveringValidMove.GetComponent<SpriteRenderer>().color = renderer;
                    break;
                }
            }
        }
    }
}
