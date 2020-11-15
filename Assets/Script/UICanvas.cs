using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private Image whiteCharacter, blackCharacter;

    public void CharacterMoveIn(float time)
    {
        // initiate
       // blackCharacter.transform.localScale.x = -1f;
        // move in
        whiteCharacter.rectTransform.DOMoveX(-6f, time, false);
        blackCharacter.rectTransform.DOMoveX(6f, time, false);
    }
}
