using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChessManager : MonoBehaviour
{
    [SerializeField] private GameObject wPawnPrefab, wKnightPrefab, wRookPrefab, wBishopPrefab, wQueenPrefab, wKingPRefab;
    [SerializeField] private GameObject bPawnPrefab, bKnightPrefab, bRookPrefab, bBishopPrefab, bQueenPrefab, bKingPRefab;

    private GameObject[] wPawns, wKnights, wRooks, wBishops;
    private GameObject[] bPawns, bKnights, bRooks, bBishops;
    private GameObject wKing, wQueen, bKing, bQueen;

    private void Awake()
    {
        wPawns = new GameObject[8];
        wKnights = new GameObject[2];
        wRooks = new GameObject[2];
        wBishops = new GameObject[2];

        bPawns = new GameObject[8];
        bKnights = new GameObject[2];
        bRooks = new GameObject[2];
        bBishops = new GameObject[2];
    }

    public IEnumerator InitiateChess()
    {
        // Instantiate pawn
        for (int i = 0; i < 8; i++)
        {
            wPawns[i] = Instantiate(wPawnPrefab, new Vector2(0f, -6f), Quaternion.identity);
            bPawns[i] = Instantiate(bPawnPrefab, new Vector2(0f, 6f), Quaternion.identity);
        }

        // Instantiate Rook, Knight and Bishop
        for (int i = 0; i < 2; i++)
        {
            wRooks[i] = Instantiate(wRookPrefab, new Vector2(0f, -6f), Quaternion.identity);
            wKnights[i] = Instantiate(wKnightPrefab, new Vector2(0f, -6f), Quaternion.identity);
            wBishops[i] = Instantiate(wBishopPrefab, new Vector2(0f, -6f), Quaternion.identity);

            bRooks[i] = Instantiate(bRookPrefab, new Vector2(0f, 6f), Quaternion.identity);
            bKnights[i] = Instantiate(bKnightPrefab, new Vector2(0f, 6f), Quaternion.identity);
            bBishops[i] = Instantiate(bBishopPrefab, new Vector2(0f, 6f), Quaternion.identity);
        }

        // Instantiate Queen and King
        wQueen = Instantiate(wQueenPrefab, new Vector2(0f, -6f), Quaternion.identity);
        wKing = Instantiate(wKingPRefab, new Vector2(0f, -6f), Quaternion.identity);
        bQueen = Instantiate(bQueenPrefab, new Vector2(0f, 6f), Quaternion.identity);
        bKing = Instantiate(bKingPRefab, new Vector2(0f, 6f), Quaternion.identity);

        // Move all pawn to the correct position
        for (int i = -4; i < 4; i++)
        {
            wPawns[i + 4].transform.DOMove(new Vector2(i + 0.5f, -2.5f), 0.5f, false);
            bPawns[i + 4].transform.DOMove(new Vector2((i * -1) - 0.5f, 2.5f), 0.5f, false);
            yield return new WaitForSeconds(0.1f);
        }

        // Move all Rooks to the correct position
        for (int i = 0; i < 2; i++)
        {
            float degree = Mathf.Deg2Rad * (90 + i * 180);
            wRooks[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 3.5f, -3.5f), 0.5f, false);
            bRooks[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 3.5f, 3.5f), 0.5f, false);
        }
        
        yield return new WaitForSeconds(0.5f);

        // Move all Kights to the correct position
        for (int i = 0; i < 2; i++)
        {
            float degree = Mathf.Deg2Rad * (90 + i * 180);
            wKnights[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 2.5f, -3.5f), 0.5f, false);
            bKnights[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 2.5f, 3.5f), 0.5f, false);
        }

        yield return new WaitForSeconds(0.5f);

        // Move all Bishops to the correct position
        for (int i = 0; i < 2; i++)
        {
            float degree = Mathf.Deg2Rad * (90 + i * 180);
            wBishops[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 1.5f, -3.5f), 0.5f, false);
            bBishops[i].transform.DOMove(new Vector2(Mathf.Sin(degree) * 1.5f, 3.5f), 0.5f, false);
        }

        yield return new WaitForSeconds(0.5f);
        
        // Move King and Queen to the correct position
        wQueen.transform.DOMove(new Vector2(-0.5f, -3.5f), 0.5f, false);
        wKing.transform.DOMove(new Vector2(0.5f, -3.5f), 0.5f, false);
        bQueen.transform.DOMove(new Vector2(-0.5f, 3.5f), 0.5f, false);
        bKing.transform.DOMove(new Vector2(0.5f, 3.5f), 0.5f, false);
    }
}
