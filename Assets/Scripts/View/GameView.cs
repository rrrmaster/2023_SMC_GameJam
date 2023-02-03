using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    public TMP_Text turnText;
    public GameManager gameManager;

    public void SetTurnText(int turn)
    {
        turnText.text = $"TURN\n<size=60>{turn}</size>";
    }

    public void SetRedUnitIndex(int index)
    {
        gameManager.redUnitIndex = index;
    }

    public void SetBlueUnitIndex(int index)
    {
        gameManager.blueUnitIndex = index;
    }
}