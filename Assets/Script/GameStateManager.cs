using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] bool alwaysPlayerTurn = false;

    // Reference
    [SerializeField] GameObject chessBoard = default;
    [SerializeField] ParticleSystem dust = default;
    [SerializeField] CursorController playerCursor = default, enemyCursor = default;
    [SerializeField] UICanvas UICanvas = default;
    [SerializeField] GameObject promotionUI = default;

    private bool isPlayerTurn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChessBoardInitiate(chessBoard, 15f, 1f, 0.5f, 1f));
    }

    private IEnumerator ChessBoardInitiate(GameObject board, float initialSize, float finalSize, float time, float speedUp)
    {
        var animator = chessBoard.GetComponent<Animator>();
        animator.Play("Drop");

        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        yield return new WaitForSeconds(clipInfo[0].clip.length * 0.4f);
        AudioManager.Instance.PlaySFX("whoosh", 0.45f);
        yield return new WaitForSeconds(clipInfo[0].clip.length * 0.6f);

        StartCoroutine(Shake(chessBoard.transform, 0.25f, 0.25f));
        AudioManager.Instance.PlaySFX("impact");

        for (int i = -8; i <= 8; i++)
        {
            for (int j = -8; j <= 8; j++)
            {
                var particle = Instantiate(dust, new Vector3(i * 0.5f, j * 0.5f, 0), Quaternion.identity);
                Destroy(particle.gameObject, particle.GetComponent<ParticleSystem>().main.duration);
            }
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(GetComponent<ChessManager>().InitiateChess());
        chessBoard.GetComponentInChildren<CanvasGroup>().DOFade(1.0f, 3f);

        //AudioManager.Instance.SetMusicVolume(0.5f);
        //AudioManager.Instance.PlayMusicWithFade("theme", 3f);

        yield return new WaitForSeconds(3f);

        UICanvas.CharacterMoveIn(0.5f);

        // Enable cursor and assign a team for it
        InitiateCursor(playerCursor, Team.White);
        playerCursor.GetComponent<Input>().SetControlActive(true);
        InitiateCursor(enemyCursor, Team.Black);
        enemyCursor.GetComponent<CursorAIInput>().active = true;

        // player start first
        isPlayerTurn = false;
        Turn();
    }

    public void Turn()
    {
        isPlayerTurn = !isPlayerTurn;   // switch player\

        if (alwaysPlayerTurn)
        {
            isPlayerTurn = true;
        }

        if (isPlayerTurn)
        {
            playerCursor.isInTurn = true;
            enemyCursor.isInTurn = false;
        }
        else
        {
            playerCursor.isInTurn = false;
            enemyCursor.isInTurn = true;
            Debug.Log(enemyCursor.GetComponent<CursorAIInput>().MoveTo(new TileIndex(Random.Range(0, 7), Random.Range(0, 7))));
        }

        // UI
        UICanvas.PlayerTurn(isPlayerTurn);
    }

    // display selection UI when a pawn got promoted
    public void Promotion(TileIndex targetIndex)
    {
        // disable both cursor first
        playerCursor.isInTurn = false;
        playerCursor.GetComponent<Input>().SetControlActive(false);
        enemyCursor.isInTurn = false;

        GameObject tmp = Instantiate(promotionUI, UICanvas.transform);
        tmp.GetComponent<PromotionUI>().SetPromotionTarget(targetIndex, this);
    }

    // when the promotion is finished
    public void Promoted()
    {
        playerCursor.GetComponent<Input>().SetControlActive(true);
        Turn();
    }

    private IEnumerator Shake(Transform target, float magnitude, float time)
    {
        float timeElapsed = 0.0f;
        Vector3 originalPosition = target.position;

        do
        {
            timeElapsed += Time.deltaTime;
            target.position = originalPosition + new Vector3(Random.Range(-magnitude, +magnitude), 
                Random.Range(-magnitude, +magnitude), Random.Range(-magnitude, +magnitude));
            yield return null;
        } while (timeElapsed < time);

        target.position = originalPosition;
    }

    private void InitiateCursor(CursorController target, Team team)
    {
        target.gameObject.SetActive(true);
        target.cursorTeam = team;
        target.SpriteRenderer.DOFade(1.0f, 0.1f);
        target.ResetPosition();
        target.gamestate = this;
    }
}
