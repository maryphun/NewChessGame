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

public class ChessPieceProperties : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject dust;

    [Header("Lock On Highlight")]
    [SerializeField] private float outlineThickness;
    private bool lockOn;

    private float selectionJumpRange = 0.05f;   // the range amount this piece will move upward when it has been selected
    private SpriteRenderer renderer;

    private bool selectable;

    Vector2 originalGraphicPosition;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private PieceType _type = default;
    public PieceType Type { get { return _type; } }

    [SerializeField] private Team _team = default;
    public Team Team { get { return _team; } }
    
    //Flags for positioning
    [HideInInspector] public bool isPinned = false;
    [HideInInspector] public TileIndex pinningPieceIndex = TileIndex.Null;
    [HideInInspector] public bool isHasMoved = false;
    [HideInInspector] public bool isHasJustDoubleMoved = false;


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
        originalGraphicPosition = renderer.transform.localPosition;
    }

    /// <summary>
    /// Update it's rendering order base on its Y position. Return sorting order as an Integer
    /// </summary>
    /// <returns></returns>
    public int UpdateRenderOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -10f);
        return spriteRenderer.sortingOrder;
    }


    public void CasheValidMoves()
    {

    }

    public void Unselect(float speed)
    {
        renderer.transform.DOLocalMoveY(originalGraphicPosition.y, speed, false);
    }

    public void Select(float speed)
    {
        renderer.transform.DOLocalMoveY(originalGraphicPosition.y + selectionJumpRange, speed, false);
    }

    public void Attacked()
    {
        // death animation
        StartCoroutine(Death(transform, 15f, 0.75f));

        // TODO: remove this chess piece from the array
        //BoardArray.Instance().SetTilePieceAt();

        // TODO: shake the screen
    }

    private IEnumerator Death(Transform target, float speed, float time)
    {
        float timeElapsed = 0.0f;

        // reset the renderer position to 0 so it can spin with a right pivot point
        renderer.transform.localPosition = Vector2.zero;

        // dust particle effect
        var effect = Instantiate(dust, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        effect.GetComponent<Renderer>().sortingLayerName = renderer.sortingLayerName;
        Destroy(effect.gameObject, effect.main.duration);

        renderer.sortingLayerName = ("FlyingChess");
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
            script.Initialization(renderer.transform, 0.1f, 0.5f);

            yield return null;
        } while (timeElapsed < time);

        //renderer.DOFade(0.0f, 0.2f);

        yield return new WaitForSeconds(0.25f);
        renderer.DOFade(0.0f, 0.25f);

        selectable = true;
    }

    // the player is trying to move this piece / cancel the selection
    public void LockOn(bool boolean)
    {
        if (!selectable)
            return;

        if (boolean)
        {
            renderer.material.SetFloat("_OutlineThickness", outlineThickness);
            lockOn = true;
        }
        else
        {
            renderer.material.SetFloat("_OutlineThickness", 0.0f);
            lockOn = false;
        }
    }

    public bool IsLockedOn()
    {
        return lockOn;
    }
}
