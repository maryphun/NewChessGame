using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class BoardArray : Singleton<BoardArray>
{


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

    protected override void Awake()
    {
        
        base.Awake();
        _indicies = new IndexTileMask();
        tileCenterPosition = new Vector2[64];
        pieces = new ChessPieceProperties[64];
        wKingThreats = new List<TileIndex>();
        bKingThreats = new List<TileIndex>();
        UpdateTileCenters();
    }
    
    //Sets the object reference held at the provided index
    public void SetTilePieceAt(int row, int column, GameObject obj, bool isInitial = false)
    {
        ChessPieceProperties properties = obj.GetComponent<ChessPieceProperties>();
        pieces[this.Index2DToIndex(row, column)] = properties;
        if(properties.Type == PieceType.King)
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

    // unregister the piece in this index
    public void RemoveTilePieceAt(int row, int column)
    {
        pieces[this.Index2DToIndex(row, column)] = null;
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
    public void UpdateTileCenters()
    {
        Vector2 firstCell = new Vector2(transform.position.x - tileSize * 7 / 2, transform.position.y - tileSize * 7 / 2);
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
