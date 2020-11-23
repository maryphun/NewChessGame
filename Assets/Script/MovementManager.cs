using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    [HideInInspector] public MovementLogic logic;
    private BoardArray board;

    protected override void Awake()
    {
        base.Awake();
        logic = new MovementLogic();
        board = BoardArray.Instance();
    }

    /// <summary>
    /// this only move the chess piece on array side but doesn't move the chess piece visual. 
    /// This function return the new position this chess should move to.
    /// </summary>
    public Vector2 MoveChessPiece(GameObject chess, TileIndex origin, TileIndex target)
    {
        if (board.GetTilePiecePropertiesAt(target) != null)
        {
            //Taking Piece
            board.ClearTrackerAt(target.row, target.col);
            
        }
        //If valid set the flag for a pawn double move
        logic.CheckDoubleMoveFlag(origin, target);

        // replace and update the data on the array side
        board.RemoveTilePieceAt(origin.row, origin.col);
        board.SetTilePieceAt(target.row, target.col, chess);
        // update
        logic.UpdateValidMoves();
        //Reset pawn Double Move Flag
        logic.ResetDoubleMoveFlag(target);
        // return new position of the chess piece to move
        return board.GetTileCenter(target.row, target.col);
    }

    public void ReplaceChessPiece(GameObject chess, TileIndex targetIndex)
    {
        board.RemoveTilePieceAt(targetIndex.row, targetIndex.col);
        board.SetTilePieceAt(targetIndex.row, targetIndex.col, chess);
        // update
        logic.UpdateValidMoves();
    }
}
