using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{


    private enum AlignmentMode
    {
        None,
        Cardinal,
        Diagonal,
        Both,
    }



    public void CheckValidMove()
    {

    }

    public void MovePiece()
    {

    }

    //Sets piece flags for weather piece is pinned or not
    public void FlagPinnedPieces()
    {
        //-send vectors from kings position
        //-if hit enemy piece stop
        //-if hit friendly count and continue
        //-if hit another friendly stop
        //-if hit enemy queen bishop or rook get the alignment type
        //-set pinned if correct type
        TileIndex wKing = BoardArray.Instance().wKingIndex;
        TileIndex bKing = BoardArray.Instance().wKingIndex;
        FlagPinnedPieces(wKing);
        FlagPinnedPieces(bKing);

        //Flags any pieces pinned to the provided tileIndex piece
        void FlagPinnedPieces(TileIndex king)
        {
            Team team = BoardArray.Instance().GetTilePiecePropertiesAt(king).Team;
            for (int colDir = -1; colDir < 2; colDir++)
            {
                for (int rowDir = -1; rowDir < 2; rowDir++)
                {
                    int checkCol = king.col + colDir;
                    int checkRow = king.row + rowDir;
                    int teamPieceCount = 0;
                    TileIndex potentialPin = new TileIndex(-1, -1); //no pin state
                    while (checkCol < 8 && checkCol >= 0 && checkRow < 8 && checkRow >= 0)
                    {
                        ChessPieceProperties checkPiece = BoardArray.Instance().GetTilePiecePropertiesAt(checkRow, checkCol);
                        if (!ReferenceEquals(checkPiece, null))
                        {
                            if (checkPiece.Team == team)
                            {
                                teamPieceCount++; //Count friendly pieces
                                if (teamPieceCount == 2) //if a second piece is found can stop checking for pin
                                    break;
                                potentialPin = new TileIndex(checkRow, checkCol);
                            }
                            else
                            {
                                //Enemy Piece found
                                if (teamPieceCount == 1)
                                {
                                    //Check 

                                    if (potentialPin.row != -1) //if there is a potential pin
                                    {
                                        AlignmentMode mode = GetAlignmentMode(potentialPin, king);
                                        switch (checkPiece.Type) //if aligned correctly for piece type pin the unit
                                        {
                                            case PieceType.Queen:
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).isPinned = true;
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).pinningPieceIndex = new TileIndex(checkRow, checkCol);
                                                break;
                                            case PieceType.Bishop:
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).isPinned = mode == AlignmentMode.Diagonal;
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).pinningPieceIndex = new TileIndex(checkRow, checkCol);
                                                break;
                                            case PieceType.Rook:
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).isPinned = mode == AlignmentMode.Diagonal;
                                                BoardArray.Instance().GetTilePiecePropertiesAt(potentialPin).pinningPieceIndex = new TileIndex(checkRow, checkCol);
                                                break;
                                        }
                                    }
                                }
                                break; //Can stop checking in direction when a opposing piece is found
                            }

                        }

                        checkCol += colDir;
                        checkRow += rowDir;
                    }
                }
            }
        }
    }

    //returns all the valid moves of the piece at the tile index provided 
    //public List<TileIndex> GetValidMovesAtTile(int row, int col)
    //{
        //ChessPieceProperties piece = BoardArray.Instance().GetTilePiecePropertiesAt(row, col);



        //When is a move invalid?:

        //If pinned to king All moves not along alignment become invalid: partial TODO

        //Line of sight Blocked by other piece (excludes knight, pawn (except double move) and king): done
        //-Check squares from in to out
        //-When piece is found check team, if enemy count square and stop, otherwise discount square and stop.

        //end position blocked by friendly piece: for knight, pawn and king check if each position is occupied by friendly. done.
        //-Compare possible moves to Bit Mask

        //King moving into check: TODO
        //-Bit Mask for threatened squares this turn, king checks adjacent bits

        //If King is in check and move does not prevent check: TODO
        //-Get pieces threatening king
        //-if more than one force king move
        //-if only one check move is between king and attacking piece or taking attacking piece, 

        //Double pawn move: has already moved
        // -just a flag in properties

        //en passant: pawn has not passed last turn. partial TODO
        // -flag in pawn properties for used double move last turn
        // -If pawn directly to the right or left of piece has used enpassant avalible.

        //castling: king has moved, king moves through check, castle has moved
        //  -has moved as flags
        //  -king checks same bitmask as above along castle manuver vector

    //}

    //Calculates a list of possible moves for the piece on the cell provided
    List<TileIndex> CalcPossiblePieceMoves(int row, int col)
    {
        List<TileIndex> moveList = new List<TileIndex>();

        ChessPieceProperties piece = BoardArray.Instance().GetTilePiecePropertiesAt(row, col);
        int side = piece.CompareTag("Player Piece") ? 1 : -1; //Enemy pawns move down


        if (!piece.isPinned)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    AddIfEnemy(new TileIndex(row + 1 * side, col - 1));
                    AddIfEnemy(new TileIndex(row + 1 * side, col + 1));
                    if (AddIfNotBlocked(new TileIndex(row + 1 * side, col)))
                    {
                        TileIndex tile= new TileIndex(row + 2 * side, col);
                        if(BoardArray.Instance().GetTilePiecePropertiesAt(tile).isHasMoved)
                            AddIfNotBlocked(tile);
                    }
                    //en passant move
                    AddIfEnPassant();
                    AddIfEnPassant();
                    break;
                case PieceType.Knight:
                    for (int i = -1; i <= 1; i += 2)
                    {
                        AddIfNotBlocked(new TileIndex(row + 2, col + i));
                        AddIfNotBlocked(new TileIndex(row + i, col + 2));
                        AddIfNotBlocked(new TileIndex(row - 2, col + i));
                        AddIfNotBlocked(new TileIndex(row - i, col + 2));
                    }
                    break;
                case PieceType.Bishop:
                    AddDiagonalMoves();
                    break;
                case PieceType.Rook:
                    AddCardinalMoves();
                    break;
                case PieceType.Queen:
                    AddCardinalMoves();
                    AddDiagonalMoves();
                    break;
                case PieceType.King:
                    AddIfNotBlocked(new TileIndex(row + 1, col + 1));
                    AddIfNotBlocked(new TileIndex(row + 1, col));
                    AddIfNotBlocked(new TileIndex(row + 1, col - 1));
                    AddIfNotBlocked(new TileIndex(row, col + 1));
                    AddIfNotBlocked(new TileIndex(row, col - 1));
                    AddIfNotBlocked(new TileIndex(row - 1, col + 1));
                    AddIfNotBlocked(new TileIndex(row - 1, col));
                    AddIfNotBlocked(new TileIndex(row - 1, col - 1));
                    break;
                default:
                    Debug.LogError("GameObject.name did not match the name of a piece");
                    break;
            }
            
        }
        return moveList;

        void AddIfEnPassant()
        {
            AddIfEnPassantConditions(new TileIndex(row, col + 1));
            AddIfEnPassantConditions(new TileIndex(row, col - 1));
           
            void AddIfEnPassantConditions(TileIndex index)
            {
                if (IsIndexOnBoard(index))
                    return;
                var target = BoardArray.Instance().GetTilePiecePropertiesAt(index);
                if (target != null)
                {
                    if (target.Type == PieceType.Pawn) 
                    {
                        if (target.isHasJustDoubleMoved) 
                        {
                            AddIfValid(index);
                        }
                    }
                }
            }
        }

        void AddCardinalMoves()
        {
            for (int i = row + 1; i < 8; i++)
            {
                if (!AddIfNotBlocked(new TileIndex(i, col))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (!AddIfNotBlocked(new TileIndex(i, col))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col + 1; i < 8; i++)
            {
                if (!AddIfNotBlocked(new TileIndex(row, i))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (!AddIfNotBlocked(new TileIndex(row, i))) //If Blocked break loop
                {
                    break;
                }
            }
        }
        
        void AddDiagonalMoves()
        {
            int diff = 0;
            for (int i = col + 1; i < 8; i++)//cycle columns left
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row + diff, col + diff);
                if (!AddIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col + 1; i < 8; i++)//cycle columns left
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row - diff, col + diff);
                if (!AddIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)//cycle columns right
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row + diff, col + diff);
                if (!AddIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)//cycle columns right
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row - diff, col + diff);
                if (!AddIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
        }

        //Adds move if enemy present, returns true when added
        bool AddIfEnemy(TileIndex checkIndex)
        {
            if (IsIndexOnBoard(checkIndex))
                return false;
            ChessPieceProperties checkPiece;
            checkPiece = BoardArray.Instance().GetTilePiecePropertiesAt(checkIndex);
            if (checkPiece != null)
            {
                if (checkPiece.Team != piece.Team) //if enemy piece, add then break
                {
                    AddIfValid(checkIndex);
                    return true;
                }
            }
            return false;
        }

        //Adds move if not blocked by piece, Returns false if should break loop due to block
        bool AddIfNotBlocked(TileIndex checkIndex)
        {
            if (IsIndexOnBoard(checkIndex))
                return false;
            ChessPieceProperties checkPiece;
            checkPiece = BoardArray.Instance().GetTilePiecePropertiesAt(checkIndex);
            if (checkPiece != null)
            {
                if (checkPiece.Team != piece.Team) //if enemy piece blocking, add then break
                {
                    AddIfValid(checkIndex);
                }
                return false;
            }
            AddIfValid(checkIndex);
            return true;
        }

        //performs Any final checks and adds to list
        bool AddIfValid(TileIndex index)
        {
            bool isValid = true;
            moveList.Add(index);
            return isValid;
        }
    }

    AlignmentMode GetAlignmentMode(TileIndex a, TileIndex b)
    {
        if (a.row == b.row || a.col == b.col)
            return AlignmentMode.Cardinal;
        if (Math.Abs(a.row - b.row) == Math.Abs(a.col - b.col))
            return AlignmentMode.Cardinal;
        return AlignmentMode.None;
    }

    //Returns true index A and B are aligned along cardinals or diagonals or both 
    bool IsAligned(TileIndex a, TileIndex b, AlignmentMode mode)
    {
        //check aligned vertically or horizontally
        if (mode == AlignmentMode.Cardinal || mode == AlignmentMode.Both)
        {
            if (a.row == b.row) return true;
            if (a.col == b.col) return true;
        }
        if (mode == AlignmentMode.Diagonal || mode == AlignmentMode.Both)
        {
            if (Math.Abs(a.row - b.row) == Math.Abs(a.col - b.col)) return true;
        }
        return false;
    }
    //Returns true if index A and B and C are aligned along cardinals or diagonals or both 
    bool IsAligned(TileIndex a, TileIndex b, TileIndex c, AlignmentMode mode)
    {
        //check aligned vertically or horizontally
        if (mode == AlignmentMode.Cardinal || mode == AlignmentMode.Both)
        {
            if (a.row == b.row && a.row == c.row) return true;
            if (a.col == b.col && a.row == c.row) return true;
        }
        if (mode == AlignmentMode.Diagonal || mode == AlignmentMode.Both)
        {
            //Needs to check all 3 on the same diagonal so cannot use abs
            //(y2-y1)/(x2-x1) = 1 or -1
            //Positive diagonal:
            if (b.row - a.row == b.col - a.col)
                if (c.row - a.row == c.col - a.col)
                    return true;
            //Negative diagonal:
            if (b.row - a.row == a.col - b.col)
                if (c.row - a.row == a.col - c.col)
                    return true;
        }
        return false;
    }

    //Returns true index A is aligned between index B and index C along cardinals or diagonals or both 
    bool IsAlignedBetween(TileIndex a, TileIndex b, TileIndex c, AlignmentMode mode)
    {
        if (!IsAligned(a, b, c, mode)) return false;

        if (mode == AlignmentMode.Cardinal || mode == AlignmentMode.Both)
        {
            if (c.row == b.row)
                return a.col < Math.Max(c.col, b.col) && a.col > Math.Min(c.col, b.col);
            if (c.col == b.col)
                return a.row < Math.Max(c.row, b.row) && a.row > Math.Min(c.row, b.row);
        }
        if (mode == AlignmentMode.Diagonal || mode == AlignmentMode.Both)
        {
            //Already must be aligned diagonally if reached here
            return a.row < Math.Max(c.row, b.row) && a.row > Math.Min(c.row, b.row);
        }
        return false;
    }

    //returns true if the index is within board bounds
    bool IsIndexOnBoard(TileIndex index)
    {
        return index.row < 8 && index.row >= 0 && index.col < 8 && index.col >= 0;
    }

}
