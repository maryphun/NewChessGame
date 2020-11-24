using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class CursorController : MonoBehaviour
{
    [SerializeField] private float cursorMoveSpeed = 0.25f;

    [Header("Debug")]
    [SerializeField] private Vector2Int initialPosition = new Vector2Int(4, 4);
    [SerializeField] private bool canMoveEnemyChess = false;

    private BoardArray board;
    private ChessPieceProperties lockedOnPiece;   // store the chess piece that you have locked to move or attack
    private TileIndex lockedOnPieceIndex;
    private List<Transform> validMoveVisualList;
    private List<ChessPieceProperties> threateningChess;
    private Transform hoveringValidMove;

    public bool isInTurn;
    public Team cursorTeam;
    public GameStateManager gamestate;


    // get only
    private Vector2Int currentPosition, moveVector;
    public Vector2Int CurrentPosition { get { return currentPosition; } }
    public Vector2Int MoveVector { get { return moveVector; } }
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

    private void Awake()
    {
        board = BoardArray.Instance();
        validMoveVisualList = new List<Transform>();
        threateningChess = new List<ChessPieceProperties>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        currentPosition = initialPosition;
        lockedOnPiece = null;
    }


    public void Confirm()
    {
        if (!isInTurn)  // not your turn
            return;

        // check if player is trying to move locked-on chess piece to this index
        if (hoveringValidMove != null)
        {
            MoveChess();

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
            else if (piece.Team == cursorTeam || canMoveEnemyChess)  // last condition: this chess is belong to this player
            {
                LockOnChess(piece, new TileIndex(currentPosition.y, currentPosition.x));

                // sound effect
                AudioManager.Instance.PlaySFX("lockOn", 0.75f);
            }
        }
    }

    private void LockOnChess(ChessPieceProperties piece, TileIndex index)
    {
        piece.LockOn(true);
        // store this piece into memory
        lockedOnPiece = piece;
        lockedOnPieceIndex = index;

        // get all valid move for this chess piece
        var validMoves = MovementManager.Instance().logic.GetValidMoves(index);

        // visualize valid move
        foreach (TileIndex validMove in validMoves)
        {
            var validMoveVisual = new GameObject(lockedOnPiece.gameObject.name + "'s possible move");
            validMoveVisual.transform.position = board.GetTileCenter(index.row, index.col) + lockedOnPiece.GraphicPosition;
            validMoveVisual.transform.DOMove(board.GetTileCenter(validMove.row, validMove.col) + lockedOnPiece.GraphicPosition, cursorMoveSpeed, false);
            var newRenderer = validMoveVisual.AddComponent<SpriteRenderer>();
            var originRenderer = lockedOnPiece.SpriteRenderer;
            newRenderer.sprite = originRenderer.sprite;
            newRenderer.color = new Color(originRenderer.color.r, originRenderer.color.g, originRenderer.color.b, originRenderer.color.a / 4f);
            newRenderer.sortingLayerID = SpriteRenderer.sortingLayerID; // same layer with cursor
            newRenderer.sortingOrder = originRenderer.sortingOrder;

            newRenderer.transform.localScale = originRenderer.transform.localScale;
            validMoveVisualList.Add(validMoveVisual.transform);
            hoveringValidMove = null;

            // check if this is an attack move
            var attackTarget = board.GetTilePiecePropertiesAt(validMove);
            if (attackTarget != null
                && attackTarget.Team != cursorTeam)
            {
                newRenderer.sprite = null;
                attackTarget.Threatened(true);
                threateningChess.Add(attackTarget);
            }
        }
    }

    private void MoveChess()
    {
        TileIndex moveTargetIndex = new TileIndex(currentPosition.y, currentPosition.x);

        // check if this is a normal move or attacking
        GameObject targetChess = board.GetTilePieceAt(moveTargetIndex.row, moveTargetIndex.col);
        bool isAttackMove = false;

        if (targetChess != null)
        {
            isAttackMove = true;
        }
        else if (IsEnPassant(lockedOnPieceIndex, moveTargetIndex, lockedOnPiece))
        {
            isAttackMove = true;
            targetChess = board.GetTilePieceAt(lockedOnPieceIndex.row, moveTargetIndex.col);
        }
        else if (IsCastling(lockedOnPieceIndex, moveTargetIndex, lockedOnPiece))
        {
            //Castling!!!!!!!!!
            Castling(moveTargetIndex);
        }

        if (isAttackMove)
        {
            // this is an attack
            targetChess.GetComponent<ChessPieceProperties>().Attacked(0.15f);
            Destroy(targetChess, 1f);
            AudioManager.Instance.PlaySFX("attack", 0.5f);
            AudioManager.Instance.PlaySFX("compact", 0.5f);
        }

        // move the selected chess piece to this position index
        Vector2 newPiecePosition = MovementManager.Instance().MoveChessPiece(lockedOnPiece.gameObject, lockedOnPieceIndex, moveTargetIndex);

        // move the chess piece graphic
        lockedOnPiece.Move(newPiecePosition);

        // canccel the lock on
        lockedOnPiece.LockOn(false);

        // remove all visualization of valid move
        foreach (Transform validMoveVisual in validMoveVisualList)
        {
            // remove it immediately only if it's not the moving target. 
            float removeTime = validMoveVisual == hoveringValidMove ? cursorMoveSpeed : 0f;
            Destroy(validMoveVisual.gameObject, removeTime);
        }

        // remove all visualization of threateningChess
        foreach (ChessPieceProperties attackingChess in threateningChess)
        {
            attackingChess.Threatened(false);
        }
        threateningChess.Clear();

        // reset list 
        validMoveVisualList.Clear();
        hoveringValidMove = null;

        // is this a pawn promotion?
        if (lockedOnPiece.GetComponent<ChessPieceProperties>().Type == PieceType.Pawn
            && (moveTargetIndex.row == 7 || moveTargetIndex.row == 0))
        {
            gamestate.Promotion(moveTargetIndex);
        }
        else
        {
            // pass the turn to other side
            gamestate.Turn();
        }

        // clear memory
        lockedOnPiece = null;
    }

    public void Cancel()
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

        // remove all visualization of threateningChess
        foreach (ChessPieceProperties attackingChess in threateningChess)
        {
            attackingChess.Threatened(false);
        }
        threateningChess.Clear();

        // reset this list 
        validMoveVisualList.Clear();
        hoveringValidMove = null;

        // clear memory
        lockedOnPiece = null;

        // sound effect
        AudioManager.Instance.PlaySFX("cancellockon", 0.75f);
    }

    public void MoveCursor(Vector2Int value)
    {
        moveVector = value;

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

    public void MoveVectorReset()
    {
        moveVector = Vector2Int.zero;
    }

    public void ResetPosition()
    {
        transform.position = board.GetTileCenter(currentPosition.y, currentPosition.x);
    }

    private bool IsEnPassant(TileIndex moveOrigin, TileIndex moveTarget, ChessPieceProperties movingChess)
    {
        return (movingChess.Type == PieceType.Pawn  // check if En passant is happening if it's a pawn
            && moveOrigin.col != moveTarget.col);  // this pawn is moving diagonally
    }

    private bool IsCastling(TileIndex moveOrigin, TileIndex moveTarget, ChessPieceProperties movingChess)
    {
        bool ret = false;

        // check on the king piece
        if (movingChess.Type == PieceType.King
            && movingChess.isHasMoved == false
            && Mathf.Abs(moveOrigin.col - moveTarget.col) == 2)
        {
            // check on the rook piece
            // determine which rook to check 
            int targetArray;
            if (moveTarget.col > moveOrigin.col)
            {
                targetArray = 7;
            }
            else
            {
                targetArray = 0;
            }

            ChessPieceProperties rook;
            rook = board.GetTilePiecePropertiesAt(new TileIndex(moveOrigin.row, targetArray));

            if (rook != null)
            {
                if (rook.Type == PieceType.Rook
                    && rook.isHasMoved == false)
                {
                    // Conclusion : castling!
                    ret = true;
                }
            }
        }

        return ret;
    }

    private void Castling(TileIndex kingMove)
    {
        // Determine the new location for rook
        TileIndex rookMoveTarget = kingMove;
        // Get Rook reference
        int targetArray = -1;

        if (kingMove.col <= 2)
        {
            rookMoveTarget.col = kingMove.col + 1;
            targetArray = 0;
        }
        else if (kingMove.col >= 5)
        {
            rookMoveTarget.col = kingMove.col - 1;
            targetArray = 7;
        }

        ChessPieceProperties rook;
        rook = board.GetTilePiecePropertiesAt(new TileIndex(rookMoveTarget.row, targetArray));

        if (rook != null
            && board.GetTilePiecePropertiesAt(rookMoveTarget) == null) // target location is available to move too, just to double check
        {
            Debug.Log("from " + new TileIndex(rookMoveTarget.row, targetArray) + " move to " + rookMoveTarget);
            // move the selected chess piece to this position index
            Vector2 newPiecePosition = MovementManager.Instance().MoveChessPiece(rook.gameObject, 
                new TileIndex(rookMoveTarget.row, targetArray), rookMoveTarget);

            // move the chess piece graphic
            rook.Move(newPiecePosition);

            // Castling end.
            return;
        }

        Debug.LogWarning("Castling Error: no refernece found for rook");
    }
}
