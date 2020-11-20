using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

[RequireComponent(typeof(SpriteRenderer))]
public class ChessPieceProperties : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject dust = default;

    [Header("Lock On Highlight")]
    [SerializeField] private float outlineThickness;
    private bool lockOn;
    public bool IsLockedOn { get { return lockOn; } }

    // private variables
    private float selectionJumpRange = 0.05f;   // the range amount this piece will move upward when it has been selected
    private bool selectable;
    private IEnumerator shake;

    // constant variables
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }
    private Vector2 originalGraphicPosition;
    public Vector2 GraphicPosition { get { return originalGraphicPosition; } }

    [SerializeField] private PieceType _type = default;
    public PieceType Type { get { return _type; } }

    [SerializeField] private Team _team = default;
    public Team Team { get { return _team; } }

    //Flags for positioning
    [HideInInspector] public bool isPinned = false;
    [HideInInspector] public TileIndex pinningPieceIndex = TileIndex.Null;
    [HideInInspector] public bool isHasMoved = false;
    [HideInInspector] public bool isHasJustDoubleMoved = false;

    void Awake()
    {
        isPinned = false;
        isHasMoved = false;
        isHasJustDoubleMoved = false;
        pinningPieceIndex = TileIndex.Null;
        shake = Shake(0.01f);
    }


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Graphic reference not found in this chess piece! [" + gameObject.name + "]");
        }

        UpdateRenderOrder();
        selectable = true;

        // store the original position of its graphic at start
        originalGraphicPosition = SpriteRenderer.transform.localPosition;
    }

    /// <summary>
    /// Update it's rendering order base on its Y position. Return sorting order as an Integer
    /// </summary>
    /// <returns></returns>
    public int UpdateRenderOrder()
    {
        SpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10f);
        return SpriteRenderer.sortingOrder;
    }


    public void CasheValidMoves()
    {

    }

    public void Unselect(float speed)
    {
        SpriteRenderer.transform.DOLocalMoveY(originalGraphicPosition.y, speed, false);
    }

    public void Select(float speed)
    {
        SpriteRenderer.transform.DOLocalMoveY(originalGraphicPosition.y + selectionJumpRange, speed, false);
    }

    public void Attacked(float delay)
    {
        // death animation
        StartCoroutine(Death(transform, 15f, 0.75f, delay));

        // TODO: shake the screen
    }

    private IEnumerator Death(Transform target, float speed, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timeElapsed = 0.0f;

        // reset the renderer position to 0 so it can spin with a right pivot point
        SpriteRenderer.transform.localPosition = Vector2.zero;

        // dust particle effect
        var effect = Instantiate(dust, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        effect.GetComponent<Renderer>().sortingLayerName = SpriteRenderer.sortingLayerName;
        Destroy(effect.gameObject, effect.main.duration);

        SpriteRenderer.sortingLayerName = ("FlyingChess");
        transform.DOMove(new Vector3(10f * (Random.Range(0, 2) * 2 - 1), transform.position.y + 5f, 0f), time, false);
        selectable = false;

        do
        {
            // rotate this chess piece
            timeElapsed += Time.deltaTime;
            target.Rotate(new Vector3(0f, 0f, target.rotation.eulerAngles.z + speed * Time.deltaTime));

            // create afterimage
            var afterImage = new GameObject(gameObject.name + "'s afterimage");
            var script = afterImage.AddComponent<AfterImage>();
            script.Initialization(SpriteRenderer.transform, 0.1f, 0.5f);

            yield return null;
        } while (timeElapsed < time);

        //renderer.DOFade(0.0f, 0.2f);

        yield return new WaitForSeconds(0.25f);
        SpriteRenderer.DOFade(0.0f, 0.25f);

        selectable = true;
    }

    // the player is trying to move this piece / cancel the selection
    public void LockOn(bool boolean)
    {
        if (!selectable)
            return;

        if (boolean)
        {
            SpriteRenderer.material.SetFloat("_OutlineThickness", outlineThickness);
            lockOn = true;
        }
        else
        {
            SpriteRenderer.material.SetFloat("_OutlineThickness", 0.0f);
            lockOn = false;
        }
    }

    public void Move(Vector2 newPosition)
    {
        StartCoroutine(AfterImage(SpriteRenderer.transform, 0.25f, 0.4f, 0.2f, Time.fixedDeltaTime));
        transform.DOMove(newPosition, 0.2f);
    }

    private IEnumerator AfterImage(Transform target, float afterImageTime, float initialAlpha, float duration, float interval)
    {
        float timeElapsed = 0.0f;
        do
        {
            // rotate this chess piece
            timeElapsed += Time.deltaTime;

            // create afterimage
            var afterImage = new GameObject(gameObject.name + "'s afterimage");
            var script = afterImage.AddComponent<AfterImage>();
            script.Initialization(target, afterImageTime, initialAlpha);

            // is a very bad idea to put this in here.
            UpdateRenderOrder();
            yield return new WaitForSeconds(interval);
        } while (timeElapsed < duration);

        yield return null;
    }

    public void Threatened(bool boolean)
    {
        if (boolean)
        {
            StartCoroutine(shake);
            SpriteRenderer.material.SetFloat("_OutlineThickness", outlineThickness);
        }
        else
        {
            Debug.Log("stop");
            SpriteRenderer.transform.localPosition = originalGraphicPosition;
            StopCoroutine(shake);
            SpriteRenderer.material.SetFloat("_OutlineThickness", 0.0f);
        }
    }

    private IEnumerator Shake(float magnitude)
    {
        Vector2 originPos = originalGraphicPosition;
        while (true)
        {
            SpriteRenderer.transform.localPosition = originPos;
            // x * 2 - 1 is a equation that make the number always result in either 1 or -1, there will be no 0
            SpriteRenderer.transform.localPosition = new Vector2(((Random.Range(0, 2) * 2) - 1) * magnitude, ((Random.Range(0, 2) * 2) - 1) * magnitude) + originalGraphicPosition;
            yield return null;
        }
    }
}
