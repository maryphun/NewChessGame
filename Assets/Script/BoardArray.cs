using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PieceID
{
    None = -1,
    APawn = 0,
    BPawn = 1,
    CPawn = 2,
    DPawn = 3,
    EPawn = 4,
    FPawn = 5,
    GPawn = 6,
    HPawn = 7,
    QueenSideRook = 8,
    KingSideRook = 9,
    QueenSideKnight = 10,
    KingSideKnight = 11,
    QueenSideBishop = 12,
    KingSideBishop = 13,
    Queen = 14,
    King = 15,
}



public class BoardArray
{

    private TileIndex[] wPieceLocations;
    private TileIndex[] bPieceLocations;

    private IndexTileMask _indicies;
    public TileIndex[] Indicies {
        get
        {
            return _indicies.Mask;
        } 
    }

    public TileIndex wKingIndex { get; private set; }
    public TileIndex bKingIndex { get; private set; }

    public List<TileIndex> wKingThreats;
    public List<TileIndex> bKingThreats;

    private const float tileSize = 1f; //changes spacing of tile centers

    private Vector2[] tileCenterPosition; //this is worldspace position
    private ChessPieceProperties[] pieces;

    public BoardArray(Vector2 boardCenterPosition)
    {
        wPieceLocations = new TileIndex[16];
        bPieceLocations = new TileIndex[16];
        _indicies = new IndexTileMask();
        tileCenterPosition = new Vector2[64];
        pieces = new ChessPieceProperties[64];
        wKingThreats = new List<TileIndex>();
        bKingThreats = new List<TileIndex>();
        UpdateTileCenters(boardCenterPosition);
    }
    
    //Sets the object reference held at the provided index
    public void SetTilePieceAt(int row, int column, GameObject obj, PieceID id = PieceID.None, bool isInitial = false)
    {
        ChessPieceProperties properties = obj.GetComponent<ChessPieceProperties>();
        pieces[this.Index2DToIndex(row, column)] = properties;
        
        if(isInitial)
            properties.SetId(id);
        

        if(properties.Team == Team.White) 
            wPieceLocations[(int)properties.Id] = new TileIndex(row, column);
        if (properties.Team == Team.Black)
            bPieceLocations[(int)properties.Id] = new TileIndex(row, column);

        //Debug.Log("Piece: " + obj.name + 
        //    " ID: " + properties.Id.ToString() + 
        //    " Location: " + Utils.Index2DToIndex(properties.Team==Team.White ? wPieceLocations[(int)properties.Id]:bPieceLocations[(int)properties.Id]));

        if (properties.Type == PieceType.King)
        {
            if (properties.Team == Team.White)
                wKingIndex = new TileIndex(row, column);
            else if (properties.Team == Team.Black)
                bKingIndex = new TileIndex(row, column);
            else
                Debug.LogError("Team tag for Object named King did not match.");
        }

        properties.isHasMoved = !isInitial;

    }

    //unregister the piece in this index
    public void RemoveTilePieceAt(int row, int column)
    {
        
        pieces[this.Index2DToIndex(row, column)] = null;
    }

    //Remove pieces tracking position
    public void ClearTrackerAt(int row, int column)
    {
        ChessPieceProperties piece = GetTilePiecePropertiesAt(row, column);
        if (piece != null) {
            if (piece.Team == Team.White)
                wPieceLocations[(int)piece.Id] = TileIndex.Null;
            else if (piece.Team == Team.Black)
                bPieceLocations[(int)piece.Id] = TileIndex.Null;
            else
                Debug.LogError("Attempted to stop tracking piece without a team"); 
        }
        else
        {
            Debug.LogError("Attempted to stop tracking on tile with no piece");
        }
        if (piece.Team == Team.Black) Debug.Log("Cleared " + piece.Team + " " + piece.Id + " Tracker. Now " + bPieceLocations[(int)piece.Id].row + ", " + bPieceLocations[(int)piece.Id].col);
        if (piece.Team == Team.White) Debug.Log("Cleared "+ piece.Team + " "+ piece.Id + " Tracker. Now " + wPieceLocations[(int)piece.Id].row + ", "+ wPieceLocations[(int)piece.Id].col);
    }

    public void ResetBoard()
    {
        
    }

    //Returns the specified pieces location on the board.
    public TileIndex GetPieceLocation(Team team, PieceID id)
    {
        if (team == Team.White)
            return wPieceLocations[(int)id];
        else if (team == Team.Black)
            return bPieceLocations[(int)id];
        else
            return TileIndex.Null;
    }

    public TileIndex GetPieceLocation(ChessPieceProperties properties)
    {
        return GetPieceLocation(properties.Team, properties.Id);
    }

    //Returns properties of the specified piece 
    public ChessPieceProperties GetPieceProperties(Team team, PieceID id)
    {
        TileIndex location = GetPieceLocation(team, id);
        if (location == TileIndex.Null)
            return null;

        return GetTilePiecePropertiesAt(location);
    }

    //Get the object reference held at the provided index
    public GameObject GetTilePieceAt(int row, int column)
    {
        if (pieces[this.Index2DToIndex(row, column)] == null) 
            return null;
        
        return pieces[this.Index2DToIndex(row, column)].gameObject;
    }

    public GameObject GetTilePieceAt(TileIndex index)
    {
        return GetTilePieceAt(index.row, index.col);
    }

    public ChessPieceProperties[] GetAllTiles()
    {
        return pieces.Clone() as ChessPieceProperties[];
    }


    //Get the Properties script held at the provided index
    public ChessPieceProperties GetTilePiecePropertiesAt(int row, int column)
    {
        return pieces[this.Index2DToIndex(row, column)];
    }
    public ChessPieceProperties GetTilePiecePropertiesAt(TileIndex index)
    {
        return GetTilePiecePropertiesAt(index.row, index.col);
    }


    //Adjusts the Tile centers in the case that the board has moved.
    public void UpdateTileCenters(Vector2 center)
    {
        Vector2 firstCell = new Vector2(center.x - tileSize * 7 / 2, center.y - tileSize * 7 / 2);
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                tileCenterPosition[this.Index2DToIndex(row, col)] = firstCell + new Vector2(col * tileSize, row * tileSize);
                //Debug.Log(tileCenterPosition[Index2DToIndex(row, col)]);
            }
        }
    }

    //Returns Tile center position from provided row and column index
    public Vector2 GetTileCenter(int row, int column)
    {
        return tileCenterPosition[this.Index2DToIndex(row, column)];
    }

    //------POSITIONAL CHECKS--------

    //returns true if the target tile is occupied by a Enemy piece
    public bool IsOccupiedByEnemy(TileIndex index, Team team)
    {
        if (GetTilePiecePropertiesAt(index) == null)
            return false;

        return GetTilePiecePropertiesAt(index).Team == team;
    }

    //returns true if the target tile is occupied by a Freindly piece
    public bool IsOccupiedByFriendly(TileIndex index, Team team)
    {
        if (GetTilePiecePropertiesAt(index) == null)
            return false;

        return GetTilePiecePropertiesAt(index).Team == team;
    }

    //Converts Index from row and column format to array format
    public int Index2DToIndex(int row, int column)
    {
        return Utils.Index2DToIndex(row, column);
    }
}
