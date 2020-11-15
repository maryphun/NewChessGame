using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLogic
{
    //TODOList:
    //-Castling Logic
    //-Check Logic
    //-Pinned Logic
    
    private TileMask<bool> allThreatenedTileMaskW = new TileMask<bool>();
    private TileMask<bool> allThreatenedTileMaskB = new TileMask<bool>();
    private MoveListTileMask allMoves = new MoveListTileMask(); //TODO: Accessors to this will allow quick display of possible moves

    private enum AlignmentMode
    {
        None,
        Cardinal,
        Diagonal,
        Both,
    }

    public List<TileIndex> GetValidMoves(TileIndex index)
    {
        return allMoves[index];
    }

    //Call this after a move is made
    public void UpdateValidMoves()
    {
        FlagPinnedPieces();
        CalcMovesAndFlagThreatenedTiles();
        //Needs list of threatened tile mask for calculating king moves
        //Removes King moving into check and any moves not preventing check if in check
        RemoveInvalidCheckMoves();
    }

    private void CalcMovesAndFlagThreatenedTiles()
    {
        //Clear all current Flags
        allThreatenedTileMaskB.Clear();
        allThreatenedTileMaskW.Clear();
        //Clear all move Lists
        for (int i = 0; i < 64; i++)
        {
            allMoves[i].Clear();
        }

        foreach (TileIndex index in BoardArray.Instance().Indicies)
        {
            if (BoardArray.Instance().GetTilePiecePropertiesAt(index) != null)
            {
                bool isWhiteTeam = BoardArray.Instance().GetTilePiecePropertiesAt(index).Team == Team.White;
                (List<TileIndex> moveList, List<TileIndex> threatenedTiles) = CalcTilesThreatenedAndMovesByPiece(index.row, index.col);
                allMoves[index] = moveList;
                foreach (var threat in threatenedTiles)
                {
                    if (isWhiteTeam)
                    {
                        allThreatenedTileMaskW[threat] = true;
                    }
                    else
                    {
                        allThreatenedTileMaskB[threat] = true;
                    }
                }

            }
        }

    }

    //Sets piece flags for weather piece is pinned or not
    private void FlagPinnedPieces()
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
                    while (IsIndexOnBoard(new TileIndex(checkRow, checkCol)))
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

    private void RemoveInvalidCheckMoves()
    {
        TileIndex wKing = BoardArray.Instance().wKingIndex;
        TileIndex bKing = BoardArray.Instance().bKingIndex;
        bool isKingCheckW = allThreatenedTileMaskB[wKing];
        bool isKingCheckB = allThreatenedTileMaskW[bKing];
        if (isKingCheckW && isKingCheckB) Debug.LogException(new Exception("Both kings flaged as in check"));
        
        //TODO: in check logic
        if (isKingCheckW)
        {
            Debug.LogError("Check Situation Not Yet Accounted For");
        }
        if (isKingCheckB)
        {
            Debug.LogError("Check Situation Not Yet Accounted For");
        }

        //TODO: Prevent king moving into check
        Debug.LogError("Moving into Check Not Yet Accounted For");
    }

    //Calculates a list of threatened tiles and possible moves for the piece on the cell provided
    //Note: Excludes in King moving into check situation as enemy threats are needed
    //Returns (movesList, threatList)
    private (List<TileIndex>, List<TileIndex>) CalcTilesThreatenedAndMovesByPiece(int row, int col)
    {
        //TODO: Cashe Each pieces Tiles for movement calculation
        List<TileIndex> moveList = new List<TileIndex>();
        List<TileIndex> threatList = new List<TileIndex>();

        ChessPieceProperties piece = BoardArray.Instance().GetTilePiecePropertiesAt(row, col);
        int side = piece.CompareTag("Player Piece") ? 1 : -1; //Enemy pawns move down
        switch (piece.Type)
        {
            case PieceType.Pawn:
                AddMoveIfEnemy(new TileIndex(row + 1 * side, col - 1));
                AddMoveIfEnemy(new TileIndex(row + 1 * side, col + 1));
                if (AddMoveIfNotBlocked(new TileIndex(row + 1 * side, col), false))
                {
                    TileIndex tile = new TileIndex(row + 2 * side, col);
                    if (BoardArray.Instance().GetTilePiecePropertiesAt(tile).isHasMoved)
                        AddMoveIfNotBlocked(tile, false);
                }
                //en passant move
                AddMoveIfEnPassant();
                AddMoveIfEnPassant();
                break;
            case PieceType.Knight:
                for (int i = -1; i <= 1; i += 2)
                {
                    AddMoveIfNotBlocked(new TileIndex(row + 2, col + i));
                    AddMoveIfNotBlocked(new TileIndex(row + i, col + 2));
                    AddMoveIfNotBlocked(new TileIndex(row - 2, col + i));
                    AddMoveIfNotBlocked(new TileIndex(row - i, col + 2));
                }
                break;
            case PieceType.Bishop:
                AddDiagonalMovesAndThreats();
                break;
            case PieceType.Rook:
                AddCardinalMovesAndThreats();
                break;
            case PieceType.Queen:
                AddCardinalMovesAndThreats();
                AddDiagonalMovesAndThreats();
                break;
            case PieceType.King:
                AddMoveIfNotBlocked(new TileIndex(row + 1, col + 1));
                AddMoveIfNotBlocked(new TileIndex(row + 1, col));
                AddMoveIfNotBlocked(new TileIndex(row + 1, col - 1));
                AddMoveIfNotBlocked(new TileIndex(row, col + 1));
                AddMoveIfNotBlocked(new TileIndex(row, col - 1));
                AddMoveIfNotBlocked(new TileIndex(row - 1, col + 1));
                AddMoveIfNotBlocked(new TileIndex(row - 1, col));
                AddMoveIfNotBlocked(new TileIndex(row - 1, col - 1));
                break;
            default:
                Debug.LogError("GameObject.name did not match the name of a piece");
                break;
        }

        if (piece.isPinned)
        {
            //TODO remove all moves that do not align with pin
            Debug.LogError("Pin Situation Not Yet Accounted For");
        }

        return (moveList, threatList);

        void AddMoveIfEnPassant()
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
                            AddMove(new TileIndex(index.row + 1 * side, index.col));
                        }
                    }
                }
            }
        }

        void AddCardinalMovesAndThreats()
        {
            for (int i = row + 1; i < 8; i++)
            {
                if (!AddMoveIfNotBlocked(new TileIndex(i, col))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (!AddMoveIfNotBlocked(new TileIndex(i, col))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col + 1; i < 8; i++)
            {
                if (!AddMoveIfNotBlocked(new TileIndex(row, i))) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (!AddMoveIfNotBlocked(new TileIndex(row, i))) //If Blocked break loop
                {
                    break;
                }
            }
        }

        void AddDiagonalMovesAndThreats()
        {
            int diff = 0;
            for (int i = col + 1; i < 8; i++)//cycle columns left
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row + diff, col + diff);
                if (!AddMoveIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col + 1; i < 8; i++)//cycle columns left
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row - diff, col + diff);
                if (!AddMoveIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)//cycle columns right
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row + diff, col + diff);
                if (!AddMoveIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
            for (int i = col - 1; i >= 0; i--)//cycle columns right
            {
                diff = i - col;
                TileIndex checkIndex = new TileIndex(row - diff, col + diff);
                if (!AddMoveIfNotBlocked(checkIndex)) //If Blocked break loop
                {
                    break;
                }
            }
        }

        //Adds move if enemy present, returns true when added
        bool AddMoveIfEnemy(TileIndex checkIndex, bool isAddThreat = true)
        {
            if (IsIndexOnBoard(checkIndex))
                return false;
            ChessPieceProperties checkPiece;
            checkPiece = BoardArray.Instance().GetTilePiecePropertiesAt(checkIndex);

            if (isAddThreat) AddThreat(checkIndex); //Squares Always threatened but move only valid when piece present;

            if (checkPiece != null)
            {
                if (checkPiece.Team != piece.Team) //if enemy piece, add then break
                {
                    AddMove(checkIndex);
                    return true;
                }
            }
            return false;
        }

        //Adds move if not blocked by piece, Returns false if should break loop due to block
        bool AddMoveIfNotBlocked(TileIndex checkIndex, bool isAddThreat = true)
        {
            if (IsIndexOnBoard(checkIndex))
                return false;
            ChessPieceProperties checkPiece;
            checkPiece = BoardArray.Instance().GetTilePiecePropertiesAt(checkIndex);

            //Can threaten squares occupied by friendly but not move there
            if (isAddThreat) AddThreat(checkIndex);

            if (checkPiece != null)
            {
                if (checkPiece.Team != piece.Team) //if enemy piece blocking, add then break
                {
                    AddMove(checkIndex);
                }

                return false;
            }
            AddMove(checkIndex);
            return true;
        }

        //performs Any final checks and adds to list
        bool AddMove(TileIndex index)
        {
            bool isValid = true;
            moveList.Add(index);
            return isValid;
        }

        void AddThreat(TileIndex index)
        {
            threatList.Add(index);
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

//castling: king has moved, king moves through check, castle has moved TODO
//  -has moved as flags
//  -king checks same bitmask as above along castle manuver vector

//}
