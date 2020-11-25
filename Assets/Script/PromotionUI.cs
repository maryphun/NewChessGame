using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum SelectionType
{
    Rook,
    Knight,
    Bishop,
    Queen
}

[RequireComponent(typeof(CanvasGroup))]
public class PromotionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private Image[] choiceList = default;
    [SerializeField] private float grayscale = 0.415f;
    [SerializeField]
    private GameObject[]
        bReplacePrefab = default,
        wReplacePrefab = default;
    
    [SerializeField] private MovementManager moveManager;

    private CursorInput input = null;
    private int currentChoice;
    private BoardArray board;
    
    private TileIndex targetindex;
    private GameStateManager gamestate;

    private CursorInput KeyInput
    {
        get
        {
            if (input != null) { return input; }
            return input = new CursorInput();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // reference
        board = moveManager.board;
        
        // register movement key
        KeyInput.Default.MoveHorizontal.performed += ctx => MoveSelection((int)ctx.ReadValue<float>());
        KeyInput.Default.Confirm.performed += _ => Select(currentChoice);
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        KeyInput.Enable();
    }

    private void OnDisable()
    {
        KeyInput.Disable();
    }

    private void Select(int choice)
    {
        GameObject targetpiece = board.GetTilePieceAt(targetindex);
        GameObject replacePrefab;
        GameObject[] prefabList = targetpiece.GetComponent<ChessPieceProperties>().Team == Team.White ? wReplacePrefab : bReplacePrefab;

        // 0 = rook , 1 = knight, 2 = bishop, 3 = queen
        replacePrefab = prefabList[choice];

        if (replacePrefab != null)
        {
            GameObject newPiece = Instantiate(replacePrefab, targetpiece.transform.position, Quaternion.identity, targetpiece.transform.parent);

            moveManager.ReplaceChessPiece(newPiece, targetindex);

            Destroy(targetpiece);

            gamestate.Promoted();

            // remove this canvas
            canvasGroup.DOFade(0.0f, 0.2f);
            Invoke("SetInactive", 0.2f);
        }
        else
        {
            Debug.LogWarning("Selection out of bound!" + choice + "th choice is selected but there are only "
                + choiceList.Length + " choices.");
        }

        AudioManager.Instance.PlaySFX("promoted");
    }

    private void SetInactive()
    {
        gameObject.SetActive(false);
    }

    private void MoveSelection(int move)
    {
        int originalChoice = currentChoice;
        currentChoice = Mathf.Clamp(currentChoice + move, 0, choiceList.Length-1);

        // grayscale
        if (originalChoice != currentChoice)
        {
            choiceList[originalChoice].color = new Color(grayscale, grayscale, grayscale, grayscale);
        }
        // highlight
        choiceList[currentChoice].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        AudioManager.Instance.PlaySFX("cursor", 0.05f);
    }

    public void SetPromotionTarget(TileIndex index, GameStateManager reference)
    {
        currentChoice = 0;
        MoveSelection(0);   // default choice

        canvasGroup.DOFade(1.0f, 0.2f);
        AudioManager.Instance.PlaySFX("promotion");

        targetindex = index;
        gamestate = reference;
    }
}
