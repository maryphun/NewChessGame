using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardArray : Singleton<BoardArray>
{
    private Vector2[] tileCenterPosition; //this is worldspace position
    private GameObject[] pieces;

    private const float tileSize = 1f; //changes spacing of tile centers

    protected override void Awake()
    {
        base.Awake();
        tileCenterPosition = new Vector2[64];
        pieces = new GameObject[64];
        UpdateTileCenters();
    }

    //Sets the object reference held at the provided index
    public void SetTilePieceAt(int row, int column, GameObject obj)
    {
        pieces[Index2DToIndex(row, column)] = obj;
    }

    //Get the object reference held at the provided index
    public GameObject GetTilePieceAt(int row, int column)
    {
        return pieces[Index2DToIndex(row, column)];
    }
    
    //Adjusts the Tile centers in the case that the board has moved.
    public void UpdateTileCenters()
    {
        Vector2 firstCell = new Vector2(transform.position.x - tileSize * 7 / 2, transform.position.y - tileSize * 7 / 2);
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                tileCenterPosition[Index2DToIndex(row, col)] = firstCell + new Vector2(col * tileSize, row * tileSize);
                //Debug.Log(tileCenterPosition[Index2DToIndex(row, col)]);
            }
        }
    }

    //Returns Tile center position from provided row and column index
    public Vector2 GetTileCenter(int row, int column)
    {
        return tileCenterPosition[Index2DToIndex(row, column)];
    }

    //Converts Index from row and column format to array format
    private int Index2DToIndex(int row, int column)
    {
        if (row > 7 || column > 7 || row < 0 || column < 0)
        {
            Debug.LogError("Index Out of Range");
            return -1;
        }
        return column + row * 8;
    }

}
