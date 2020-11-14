using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameStateManager : MonoBehaviour
{
    // Reference
    [SerializeField] GameObject chessBoard;
    [SerializeField] GameObject cursor;
    [SerializeField] ParticleSystem dust;

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
                Destroy(particle.gameObject, particle.GetComponent<ParticleSystem>().main.duration - 0.1f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(GetComponent<ChessManager>().InitiateChess());
        chessBoard.GetComponentInChildren<CanvasGroup>().DOFade(1.0f, 3f);
        cursor.SetActive(true);
        cursor.GetComponent<SpriteRenderer>().DOFade(1.0f, 2f);

        //AudioManager.Instance.SetMusicVolume(0.5f);
        //AudioManager.Instance.PlayMusicWithFade("theme", 3f);
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
}
