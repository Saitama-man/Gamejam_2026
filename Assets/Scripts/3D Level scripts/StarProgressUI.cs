using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarProgressUI : MonoBehaviour
{
    [SerializeField] private Image fillStarImage;
    [SerializeField] private TMP_Text percentText;

    public void SetProgress01(float value)
    {
        value = Mathf.Clamp01(value);

        if (fillStarImage != null)
            fillStarImage.fillAmount = value;

        if (percentText != null)
            percentText.text = $"{value * 100f:0}%";
    }

    public void ResetProgress()
    {
        SetProgress01(0f);
    }
}