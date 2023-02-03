using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleView : MonoBehaviour
{
    public Image fadeimage;
    public GameObject option;
    
    public void StartGame()
    {
        fadeimage.DOFade(1, 0.5f).From(0)
            .OnStart(() => { fadeimage.gameObject.SetActive(true); })
            .OnComplete(() => { SceneManager.LoadScene("1_Game"); });
    }

    public void OptionOn()
    {
        option.SetActive(true);
    }

    public void OptionOff()
    {
        option.SetActive(false);
    }
}
