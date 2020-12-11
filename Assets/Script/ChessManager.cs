using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(MovementManager))]
public class ChessManager : MonoBehaviour
{
    [SerializeField] private Transform chessPieceHolder = default;
    [SerializeField]
    private GameObject
        wPawnPrefab = default,
        wKnightPrefab = default,
        wRookPrefab = default,
        wBishopPrefab = default,
        wQueenPrefab = default,
        wKingPRefab = default;
    [SerializeField]
    private GameObject
        bPawnPrefab = default,
        bKnightPrefab = default,
        bRookPrefab = default,
        bBishopPrefab = default,
        bQueenPrefab = default,
        bKingPRefab = default;

    private GameObject[] pPawns, pKnights, pRooks, pBishops;
    private GameObject[] oPawns, oKnights, oRooks, oBishops;
    private GameObject pKing, pQueen, oKing, oQueen;

    private MovementManager moveManager;
    private List<GameObject> playerPieces, opponentPieces;

    private void Awake()
    {
        moveManager = GetComponent<MovementManager>();

        pPawns = new GameObject[8];
        pKnights = new GameObject[2];
        pRooks = new GameObject[2];
        pBishops = new GameObject[2];

        oPawns = new GameObject[8];
        oKnights = new GameObject[2];
        oRooks = new GameObject[2];
        oBishops = new GameObject[2];

        playerPieces = new List<GameObject>();
        opponentPieces = new List<GameObject>();

    }

    public IEnumerator InitiateChess(Team playerChess, float speedMultiplier = 1.0f)
    {
        // function that handle all the object instantiation
        SpawnChessForPlayer(playerChess);
        SpawnChessForOpponent(playerChess == Team.White ? Team.Black : Team.White);

        // Move all pawn to the correct position
        for (int i = 0; i < 8; i++)
        {
            Debug.Log("white pawn: " + pPawns[i].name + " board:" + (moveManager.board != null) + " i:" + i);
            pPawns[i].transform.DOMove(moveManager.board.GetTileCenter(1, i), 0.5f * speedMultiplier, false);
            oPawns[i].transform.DOMove(moveManager.board.GetTileCenter(6, i), 0.5f * speedMultiplier, false);
            moveManager.board.SetTilePieceAt(1, i, pPawns[i], (PieceID)i, true);
            moveManager.board.SetTilePieceAt(6, i, oPawns[i], (PieceID)i, true);
            AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
            yield return new WaitForSeconds(0.1f * speedMultiplier);
        }

        // Move all Rooks to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 0 : 7;
            pRooks[i].transform.DOMove(moveManager.board.GetTileCenter(0, index), 0.5f * speedMultiplier, false);
            oRooks[i].transform.DOMove(moveManager.board.GetTileCenter(7, index), 0.5f * speedMultiplier, false);
            moveManager.board.SetTilePieceAt(0, index, pRooks[i], (PieceID)i + 8, true);
            moveManager.board.SetTilePieceAt(7, index, oRooks[i], (PieceID)i + 8, true);
        }

        yield return new WaitForSeconds(0.5f * speedMultiplier);

        // Move all Kights to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 1 : 6;
            pKnights[i].transform.DOMove(moveManager.board.GetTileCenter(0, index), 0.5f * speedMultiplier, false);
            oKnights[i].transform.DOMove(moveManager.board.GetTileCenter(7, index), 0.5f * speedMultiplier, false);
            moveManager.board.SetTilePieceAt(0, index, pKnights[i], (PieceID)i + 10, true);
            moveManager.board.SetTilePieceAt(7, index, oKnights[i], (PieceID)i + 10, true);
        }

        yield return new WaitForSeconds(0.5f * speedMultiplier);

        // Move all Bishops to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        for (int i = 0; i < 2; i++)
        {
            int index = i == 0 ? 2 : 5;
            pBishops[i].transform.DOMove(moveManager.board.GetTileCenter(0, index), 0.5f * speedMultiplier, false);
            oBishops[i].transform.DOMove(moveManager.board.GetTileCenter(7, index), 0.5f * speedMultiplier, false);
            moveManager.board.SetTilePieceAt(0, index, pBishops[i], (PieceID)i + 12, true);
            moveManager.board.SetTilePieceAt(7, index, oBishops[i], (PieceID)i + 12, true);
        }

        yield return new WaitForSeconds(0.5f * speedMultiplier);

        // Move King and Queen to the correct position
        AudioManager.Instance.PlaySFX("chessSpawn", 0.05f);
        pQueen.transform.DOMove(moveManager.board.GetTileCenter(0, 3), 0.5f * speedMultiplier, false);
        pKing.transform.DOMove(moveManager.board.GetTileCenter(0, 4), 0.5f * speedMultiplier, false);
        oQueen.transform.DOMove(moveManager.board.GetTileCenter(7, 3), 0.5f * speedMultiplier, false);
        oKing.transform.DOMove(moveManager.board.GetTileCenter(7, 4), 0.5f * speedMultiplier, false);
        moveManager.board.SetTilePieceAt(0, 3, pQueen, PieceID.Queen, true);
        moveManager.board.SetTilePieceAt(0, 4, pKing, PieceID.King, true);
        moveManager.board.SetTilePieceAt(7, 3, oQueen, PieceID.Queen, true);
        moveManager.board.SetTilePieceAt(7, 4, oKing, PieceID.King, true);
        yield return new WaitForSeconds(0.51f * speedMultiplier);

        // Update it's rendering order. This function should always get called when you move the piece
        foreach (GameObject pieces in playerPieces)
        {
            pieces.GetComponent<ChessPieceProperties>().UpdateRenderOrder();
            pieces.transform.SetParent(chessPieceHolder);
            pieces.gameObject.tag = "Player Piece";
        }
        foreach (GameObject pieces in opponentPieces)
        {
            pieces.GetComponent<ChessPieceProperties>().UpdateRenderOrder();
            pieces.transform.SetParent(chessPieceHolder);
        }
        moveManager.logic.UpdateValidMoves();
    }

    private void SpawnChessForPlayer(Team team)
    {
        // Instantiate pawn
        for (int i = 0; i < 8; i++)
        {
            GameObject pawn = team == Team.White ? wPawnPrefab : bPawnPrefab;
            pPawns[i] = Instantiate(pawn, new Vector2(0f, -6f), Quaternion.identity);

            // Listing
            playerPieces.Add(pPawns[i]);
        }

        // Instantiate Rook, Knight and Bishop
        for (int i = 0; i < 2; i++)
        {
            GameObject rook = team == Team.White ? wRookPrefab : bRookPrefab;
            GameObject knight = team == Team.White ? wKnightPrefab : bKnightPrefab;
            GameObject bishop = team == Team.White ? wBishopPrefab : bBishopPrefab;
            pRooks[i] = Instantiate(rook, new Vector2(0f, -6f), Quaternion.identity);
            pKnights[i] = Instantiate(knight, new Vector2(0f, -6f), Quaternion.identity);
            pBishops[i] = Instantiate(bishop, new Vector2(0f, -6f), Quaternion.identity);

            // Listing
            playerPieces.Add(pRooks[i]);
            playerPieces.Add(pKnights[i]);
            playerPieces.Add(pBishops[i]);
        }

        // Instantiate Queen and King
        GameObject queen = team == Team.White ? wQueenPrefab : bQueenPrefab;
        GameObject king = team == Team.White ? wKingPRefab : bKingPRefab;
        pQueen = Instantiate(queen, new Vector2(0f, -6f), Quaternion.identity);
        pKing = Instantiate(king, new Vector2(0f, -6f), Quaternion.identity);

        // Listing
        playerPieces.Add(pQueen);
        playerPieces.Add(pKing);
    }

    private void SpawnChessForOpponent(Team team)
    {
        // Instantiate pawn
        for (int i = 0; i < 8; i++)
        {
            GameObject pawn = team == Team.White ? wPawnPrefab : bPawnPrefab;
            oPawns[i] = Instantiate(pawn, new Vector2(0f, 6f), Quaternion.identity);

            // Listing
            opponentPieces.Add(oPawns[i]);
        }

        // Instantiate Rook, Knight and Bishop
        for (int i = 0; i < 2; i++)
        {
            GameObject rook = team == Team.White ? wRookPrefab : bRookPrefab;
            GameObject knight = team == Team.White ? wKnightPrefab : bKnightPrefab;
            GameObject bishop = team == Team.White ? wBishopPrefab : bBishopPrefab;
            oRooks[i] = Instantiate(rook, new Vector2(0f, 6f), Quaternion.identity);
            oKnights[i] = Instantiate(knight, new Vector2(0f, 6f), Quaternion.identity);
            oBishops[i] = Instantiate(bishop, new Vector2(0f, 6f), Quaternion.identity);

            // Listing
            opponentPieces.Add(oRooks[i]);
            opponentPieces.Add(oKnights[i]);
            opponentPieces.Add(oBishops[i]);
        }

        // Instantiate Queen and King
        GameObject queen = team == Team.White ? wQueenPrefab : bQueenPrefab;
        GameObject king = team == Team.White ? wKingPRefab : bKingPRefab;
        oQueen = Instantiate(queen, new Vector2(0f, 6f), Quaternion.identity);
        oKing = Instantiate(king, new Vector2(0f, 6f), Quaternion.identity);

        // Listing
        opponentPieces.Add(oQueen);
        opponentPieces.Add(oKing);
    }
    
    public void RemoveAndReset()
    {
        // destroy all objects
        foreach (GameObject pieces in playerPieces)
        {
            Destroy(pieces);
        }
        foreach (GameObject pieces in opponentPieces)
        {
            Destroy(pieces);
        }

        // clear array
        for (int row  = 0; row <= 8; row++)
        {
            for (int col = 0; col <= 8; col++)
            {
                moveManager.board.ClearTrackerAt(row, col);
            }
        }
    }
}