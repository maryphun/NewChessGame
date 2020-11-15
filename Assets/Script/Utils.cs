using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int Index2DToIndex(int row, int column)
    {
        if (row > 7 || column > 7 || row < 0 || column < 0)
        {
            Debug.LogError("Index Out of Range");
            return -1;
        }
        return column + row * 8;
    }

    public static int Index2DToIndex(TileIndex index)
    {
        return Index2DToIndex(index.row, index.col);
    }

    public static TileIndex IndexToTileIndex(int index)
    {
        if (index > 63 || index < 0)
        {
            Debug.LogError("Index Out of Range");
            return TileIndex.Null;
        }
        return new TileIndex(index/8, index%8);
    }
}
