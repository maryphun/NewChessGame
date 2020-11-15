using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveListTileMask : TileMask<List<TileIndex>>
{
    public MoveListTileMask()
    {
        for(int i = 0; i<Length; i++)
        {
            Mask[i] = new List<TileIndex>();
        }
    }
}
