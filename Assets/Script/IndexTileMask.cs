
public class IndexTileMask : TileMask<TileIndex>
{
    public IndexTileMask()
    {
        for (int i = 0; i < Length; i++)
        {
            Mask[i] = new TileIndex(i);
        }
    }
}
