using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public TMP_Text turnText;
    public GameManager gameManager;
    public SeedCardView[] redTeamUnitButtons;
    public SeedCardView[] blueTeamUnitButtons;

    public Button redStartButton;
    public Button blueStartButton;
    public RectTransform redPanel;
    public RectTransform bluePanel;

    public void SetTurnText(int turn)
    {
        turnText.text = $"TURN\n<size=60>{turn}</size>";
    }

    public void SetRedTeamUnitCount(int index, int count, int maxCount)
    {
        redTeamUnitButtons[index].SetCountText(count, maxCount);
    }

    public void SetBlueTeamUnitCount(int index, int count, int maxCount)
    {
        blueTeamUnitButtons[index].SetCountText(count, maxCount);
    }

    public void SetTurnChanged()
    {
        redStartButton.interactable = gameManager.TeamTurn == Team.Red;
        blueStartButton.interactable = gameManager.TeamTurn == Team.Blue;
        if (gameManager.TeamTurn == Team.Red)
        {
            redPanel.DOAnchorPosX(0, 1.0f);
            bluePanel.DOAnchorPosX(128, 1.0f);
        }
        else
        {
            redPanel.DOAnchorPosX(-128, 1.0f);
            bluePanel.DOAnchorPosX(0, 1.0f);
        }
    }

    public void SetComplateTurn()
    {
        gameManager.ComplateTurn();
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