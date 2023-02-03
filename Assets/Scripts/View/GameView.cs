using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    public TMP_Text turnText;
    public GameManager gameManager;
    public GameObject[] redTeamUnitButtons;
    public GameObject[] blueTeamUnitButtons;

    public void SetTurnText(int turn)
    {
        turnText.text = $"TURN\n<size=60>{turn}</size>";
    }

    public void SetRedTeamUnitCount(int index, int count)
    {
        redTeamUnitButtons[index].GetComponentInChildren<TMP_Text>().text = count.ToString();
    }
    
    public void SetBlueTeamUnitCount(int index, int count)
    {
        blueTeamUnitButtons[index].GetComponentInChildren<TMP_Text>().text = count.ToString();
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