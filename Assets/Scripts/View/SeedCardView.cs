using TMPro;
using UnityEngine;

public class SeedCardView : MonoBehaviour
{
    public TMP_Text countText;

    public void SetCountText(int count, int maxCount)
    {
        countText.text = $"{count}/{maxCount}";
    }
}