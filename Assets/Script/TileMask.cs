using System;

public class TileMask<T>
{
    T[] mask;
    public int Length => mask.Length;


    public TileMask()
    {
        mask = new T[64];
    }

    public T[] Mask
    {
        get => mask;
        //set => mask = value;
    } 

    public T this[int index]
    {
        get => mask[index];
        set => mask[index] = value;
    }

    public T this[TileIndex index]
    {
        get => mask[Utils.Index2DToIndex(index)];
        set => mask[Utils.Index2DToIndex(index)] = value;
    }

    public void Clear() 
    { 
        Array.Clear(mask, 0, Length); 
    }

}
