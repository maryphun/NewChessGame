
public struct TileIndex
{
    public int row;
    public int col;

    public static TileIndex Null => new TileIndex(-1, -1);

    public TileIndex(int r, int c)
    {
        row = r;
        col = c;
    }

    public TileIndex(TileIndex index)
    {
        row = index.row;
        col = index.col;
    }

    public TileIndex(int i)
    {
        this = new TileIndex(Utils.IndexToTileIndex(i));
    }

    public static TileIndex operator -(TileIndex a)
        => new TileIndex(-a.row, -a.col);

    public static TileIndex operator +(TileIndex a, TileIndex b)
        => new TileIndex(a.row + b.row, a.col + b.col);

    public static TileIndex operator -(TileIndex a, TileIndex b)
        => a + (-b);

    public static bool operator ==(TileIndex a, TileIndex b)
        => (a.row == b.row && a.col == b.col);
    public static bool operator !=(TileIndex a, TileIndex b)
        => !(a == b);


}
