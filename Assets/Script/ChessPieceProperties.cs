using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChessPieceProperties : MonoBehaviour
{
    [SerializeField] GameObject dust;

    private float selectionJumpRange = 0.05f;   // the range amount this piece will move upward when it has been selected
    private SpriteRenderer renderer;

    private bool selectable;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Graphic reference not found in this chess piece! [" + gameObject.name + "]");
        }

        UpdateRenderOrder();
        selectable = true;
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

    public void Unselect(float speed)
    {
         renderer.transform.DOMoveY(renderer.transform.position.y - selectionJumpRange, speed, false);
    }

    public void Select(float speed)
    {
        renderer.transform.DOMoveY(renderer.transform.position.y + selectionJumpRange, speed, false);
    }

    public void Attacked()
    {
        // death animation
        StartCoroutine(Death(transform, 15f, 0.75f, false));

        // TODO: remove this chess piece from the array
        //BoardArray.Instance().SetTilePieceAt();
    }

    private IEnumerator Death(Transform target, float speed, float time, bool returnToZeroRotation)
    {
        float timeElapsed = 0.0f;

        // reset the renderer position to 0 so it can spin with a right pivot point
        renderer.transform.localPosition = Vector2.zero;

        // dust particle effect
        var effect = Instantiate(dust, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        effect.GetComponent<Renderer>().sortingLayerName = renderer.sortingLayerName;
        Destroy(effect.gameObject, effect.main.duration);

        renderer.sortingLayerName = ("FlyingChess");
        transform.DOMove(new Vector3(10f, transform.position.y + 5f, 0f), time, false);
        selectable = false;

        do
        {
            timeElapsed += Time.deltaTime;
            target.Rotate(new Vector3(0f, 0f, target.rotation.eulerAngles.z + speed * Time.deltaTime));
            yield return null;
        } while (timeElapsed < time);

        if (returnToZeroRotation)
        {
            target.localRotation = Quaternion.identity;
        }
        //renderer.DOFade(0.0f, 0.2f);

        transform.DOShakePosition(0.25f);
        yield return new WaitForSeconds(0.25f);
        renderer.DOFade(0.0f, 0.25f);

        selectable = true;
    }
}
