using UnityEngine;
using UnityEngine.UI;

public class ConstellationMapSlotUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject questionMarkObject;
    [SerializeField] private Image constellationIconImage;

    private int stageIndex;
    private bool unlocked;
    private StarMapUI mapUI;

    public void Init(int index, StarMapUI owner)
    {
        stageIndex = index;
        mapUI = owner;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        SetUnlocked(false, null);
    }

    public void SetUnlocked(bool value, Sprite icon)
    {
        unlocked = value;

        if (questionMarkObject != null)
            questionMarkObject.SetActive(!unlocked);

        if (constellationIconImage != null)
        {
            constellationIconImage.gameObject.SetActive(unlocked);
            constellationIconImage.sprite = icon;
        }

        if (button != null)
            button.interactable = unlocked;
    }

    private void OnClick()
    {
        if (!unlocked)
            return;

        if (mapUI != null)
            mapUI.OpenConstellationDialog(stageIndex);
    }
}