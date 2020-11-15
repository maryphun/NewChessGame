using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King,
}

public enum Team
{
    None,
    Black,
    White,
}

public class ChessPieceProperties : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private PieceType _type = default;
    public PieceType Type { get { return _type; } }

    [SerializeField] private Team _team = default;
    public Team Team { get { return _team; } }
    
    //Flags for positioning
    [HideInInspector] public bool isPinned = false;
    [HideInInspector] public TileIndex pinningPieceIndex = TileIndex.Null;
    [HideInInspector] public bool isHasMoved = false;
    [HideInInspector] public bool isHasJustDoubleMoved = false;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Graphic reference not found in this chess piece! [" + gameObject.name + "]");
        }

        UpdateRenderOrder();
    }

    /// <summary>
    /// Update it's rendering order base on its Y position. Return sorting order as an Integer
    /// </summary>
    /// <returns></returns>
    public int UpdateRenderOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10f);
        return spriteRenderer.sortingOrder;
    }


    public void CasheValidMoves()
    {

    }
}
