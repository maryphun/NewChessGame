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
    private SpriteRenderer renderer;

    [SerializeField] private PieceType _type;
    public PieceType Type { get { return _type; } }

    [SerializeField] private Team _team;
    public Team Team { get { return _team; } }
    
    //Flags for positioning
    [HideInInspector] public bool isPinned;
    [HideInInspector] public TileIndex pinningPieceIndex;
    [HideInInspector] public bool isHasMoved;
    [HideInInspector] public bool isHasJustDoubleMoved;


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer == null)
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
        renderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10f);
        return renderer.sortingOrder;
    }


    public void CasheValidMoves()
    {

    }
}
