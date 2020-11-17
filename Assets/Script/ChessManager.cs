using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChessManager : MonoBehaviour
{
    [SerializeField] private Transform chessPieceHolder = default;
    [SerializeField] private GameObject 
        wPawnPrefab = default, 
        wKnightPrefab = default, 
        wRookPrefab = default , 
        wBishopPrefab = default, 
        wQueenPrefab = default, 
        wKingPRefab = default;
    [SerializeField] private GameObject 
        bPawnPrefab = default, 
        bKnightPrefab = default, 
        bRookPrefab = default, 
        bBishopPrefab = default, 
        bQueenPrefab = default, 
        bKingPRefab = default;

    private GameObject[] wPawns, wKnights, wRooks, wBishops;
    private GameObject[] bPawns, bKnights, bRooks, bBishops;
    private GameObject wKing, wQueen, bKing, bQueen;
    
    private BoardArray board;
    private List<GameObject> whitePieces, blackPieces;

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
        
        board = BoardArray.Instance();
        whitePieces = new List<GameObject>();
        blackPieces = new List<GameObject>();
    }

    public IEnumerator InitiateChess()
    {
        // Instantiate pawn
        for (int i = 0; i < 8; i++)
        {
            wPawns[i] = Instantiate(wPawnPrefab, new Vector2(0f, -6f), Quaternion.identity);
            bPawns[i] = Instantiate(bPawnPrefab, new Vector2(0f, 6f), Quaternion.identity);

            // Listing
            whitePieces.Add(wPawns[i]);
            blackPieces.Add(bPawns[i]);
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

            // Listing
            whitePieces.Add(wRooks[i]);
            whitePieces.Add(wKnights[i]);
            whitePieces.Add(wBishops[i]);
            blackPieces.Add(bRooks[i]);
            blackPieces.Add(bKnights[i]);
            blackPieces.Add(bBishops[i]);
        }

        // Instantiate Queen and King
        wQueen = Instantiate(wQueenPrefab, new Vector2(0f, -6f), Quaternion.identity);
        wKing = Instantiate(wKingPRefab, new Vector2(0f, -6f), Quaternion.identity);
        bQueen = Instantiate(bQueenPrefab, new Vector2(0f, 6f), Quaternion.identity);
        bKing = Instantiate(bKingPRefab, new Vector2(0f, 6f), Quaternion.identity);

        // Listing
        whitePieces.Add(wQueen);
        whitePieces.Add(wKing);
        blackPieces.Add(bQueen);
        blackPieces.Add(bKing);

        // Move all pawn to the correct position
        for (int i = 0; i < 8; i++)
        {
            wPawns[i].transform.DOMove(board.GetTileCenter(1, i), 0.5f, false);
            bPawns[i].transform.DOMove(board.GetTileCenter(6, i), 0.5f, false);
            board.SetTilePieceAt(1, i, wPawns[i]);
            board.SetTilePieceAt(6, i, bPawns[i]);
            AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
            yield return new WaitForSeconds(0.1f);
        }

        // Move all Rooks to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 0 : 7;
            wRooks[i].transform.DOMove(board.GetTileCenter(0, index), 0.5f, false);
            bRooks[i].transform.DOMove(board.GetTileCenter(7, index), 0.5f, false);
            board.SetTilePieceAt(0, index, wRooks[i]);
            board.SetTilePieceAt(7, index, bRooks[i]);
        }

        yield return new WaitForSeconds(0.5f);

        // Move all Kights to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 1 : 6;
            wKnights[i].transform.DOMove(board.GetTileCenter(0, index), 0.5f, false);
            bKnights[i].transform.DOMove(board.GetTileCenter(7, index), 0.5f, false);
            board.SetTilePieceAt(0, index, wKnights[i]);
            board.SetTilePieceAt(7, index, bKnights[i]);
        }

        yield return new WaitForSeconds(0.5f);

        // Move all Bishops to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 2 : 5;
            wBishops[i].transform.DOMove(board.GetTileCenter(0, index), 0.5f, false);
            bBishops[i].transform.DOMove(board.GetTileCenter(7, index), 0.5f, false);
            board.SetTilePieceAt(0, index, wBishops[i]);
            board.SetTilePieceAt(7, index, bBishops[i]);
        }

        yield return new WaitForSeconds(0.5f);

        // Move King and Queen to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        wQueen.transform.DOMove(board.GetTileCenter(0, 3), 0.5f, false);
        wKing.transform.DOMove(board.GetTileCenter(0, 4), 0.5f, false);
        bQueen.transform.DOMove(board.GetTileCenter(7, 4), 0.5f, false);
        bKing.transform.DOMove(board.GetTileCenter(7, 3), 0.5f, false);
        board.SetTilePieceAt(0, 3, wQueen);
        board.SetTilePieceAt(0, 4, wKing);
        board.SetTilePieceAt(7, 4, bQueen);
        board.SetTilePieceAt(7, 3, bKing);
        yield return new WaitForSeconds(0.51f);

        // Update it's rendering order. This function should always get called when you move the piece
        foreach (GameObject pieces in whitePieces)
        {
            pieces.GetComponent<ChessPieceProperties>().UpdateRenderOrder();
            pieces.transform.SetParent(chessPieceHolder);
        }
        foreach (GameObject pieces in blackPieces)
        {
            pieces.GetComponent<ChessPieceProperties>().UpdateRenderOrder();
            pieces.transform.SetParent(chessPieceHolder);
        }
    }
}