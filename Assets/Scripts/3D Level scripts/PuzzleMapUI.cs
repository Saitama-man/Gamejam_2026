using System;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMapUI : MonoBehaviour
{
    public event Action OnMapOpened;
    public event Action OnMapClosed;

    [Header("Buttons")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Button closeButton;

    [Header("Panel")]
    [SerializeField] private GameObject mapPanel;

    [Header("Slots")]
    [SerializeField] private PuzzleMapSlot[] slots;

    private bool isOpen;

    private void Awake()
    {
        if (mapButton != null)
            mapButton.onClick.AddListener(OpenMap);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMap);

        if (mapPanel != null)
            mapPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (mapButton != null)
            mapButton.onClick.RemoveListener(OpenMap);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseMap);
    }

    public void ResetMap()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                slots[i].SetEmpty();
        }
    }

    public void SetConstellationCompleted(int index, Sprite constellationSprite)
    {
        if (index < 0 || index >= slots.Length)
            return;

        if (slots[index] != null)
            slots[index].SetCompleted(constellationSprite);
    }

    public void OpenMap()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (mapPanel != null)
            mapPanel.SetActive(true);

        OnMapOpened?.Invoke();
    }

    public void CloseMap()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (mapPanel != null)
            mapPanel.SetActive(false);

        OnMapClosed?.Invoke();
    }
}