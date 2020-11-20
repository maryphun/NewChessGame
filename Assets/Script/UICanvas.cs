using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private Image playerCharacter, opponentCharacter;

    [Header("Setting")]
    [SerializeField] private float highlightScale = 1.1f;

    public void CharacterMoveIn(float time)
    {
        // move in
        playerCharacter.rectTransform.DOMoveX(-6f, time, false);
        opponentCharacter.rectTransform.DOMoveX(6f, time, false);
    }

    public void PlayerTurn(bool boolean)
    {
        Image turnStart, turnEnd;
        if (boolean)
        {
            turnStart = playerCharacter;
            turnEnd = opponentCharacter;
        }
        else
        {
            turnStart = opponentCharacter;
            turnEnd = playerCharacter;
        }

        turnStart.rectTransform.DOScale(highlightScale, 0.5f);
        turnEnd.rectTransform.DOScale(1.0f , 0.5f);
    }
}
