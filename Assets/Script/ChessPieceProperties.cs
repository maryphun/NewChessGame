using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceProperties : MonoBehaviour
{
    private SpriteRenderer renderer;
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
}
