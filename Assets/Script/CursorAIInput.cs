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
        if (operating || !active)
            return false;

        StartCoroutine(MoveToTargetArray(index.row, index.col, moveInterval));
        operating = true;

        return true;
    }

    public void Click()
    {
        if (operating || !active)
            return;

        controller.Confirm();
    }

    public void Cancel()
    {
        if (operating || !active)
            return;

        controller.Cancel();
    }

    private IEnumerator MoveToTargetArray(int row, int col, float interval)
    {
        // move Y axis first
        Vector2Int moveVector = new Vector2Int(col, row) - controller.CurrentPosition;
        moveVector.y = Mathf.Clamp(moveVector.y, -1, 1);
        moveVector.x = Mathf.Clamp(moveVector.x, -1, 1);

        if (!(moveVector.y == 0))
        {
            moveVector.x = 0;
        }

        if (moveVector.magnitude > 0)
        {
            controller.MoveCursor(moveVector);
            yield return new WaitForSeconds(interval);
            StartCoroutine(MoveToTargetArray(row, col, moveInterval));
        }
        else
        {
            // Finish move, call for next action
            operating = false;
        }

        yield return null;
    }

    private void Awake()
    {
        // initialization
        controller = GetComponent<CursorController>();

        operating = false;
    }
}
