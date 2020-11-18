using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script need CursorController to work
[RequireComponent(typeof(CursorController))]
public class CursorAIInput : MonoBehaviour
{
    [SerializeField] public bool active;
    [SerializeField] private float moveInterval = 0.15f;
    private bool operating; // it is already under an operate and should not be interrupted is this is true
    private CursorController controller;

    // move the cursor to specific tileindex
    public bool MoveTo(TileIndex index)
    {
        if (!operating || active)
            return false;

        StartCoroutine(MoveToTargetArray(index.row, index.col, moveInterval));

        return true;
    }

    private IEnumerator MoveToTargetArray(int row, int col, float interval)
    {
        // move Y axis first
        if (controller.CurrentPosition.y != row)   
        {

        }
        else if (controller.CurrentPosition.x != col)
        {

        }
        else
        {

        }

        yield return null;
    }

    private void Awake()
    {
        // initialization
        controller = GetComponent<CursorController>();
    }
}
