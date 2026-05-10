using System;
using TMPro;
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

    [Header("Info Window")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button infoCloseButton;
    [SerializeField] private TMP_Text infoNameText;
    [SerializeField] private TMP_Text infoDescriptionText;

    [Header("Hide While Map Is Open")]
    [SerializeField] private GameObject[] hideWhileMapOpen;

    public bool IsOpen => isOpen;

    private bool isOpen;

    private void Awake()
    {
        if (mapButton != null)
            mapButton.onClick.AddListener(OpenMap);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMap);

        if (infoCloseButton != null)
            infoCloseButton.onClick.AddListener(CloseConstellationInfo);

        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                    slots[i].Initialize(this);
            }
        }

        if (mapPanel != null)
            mapPanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        SetHiddenObjectsVisible(true);
    }

    private void OnDestroy()
    {
        if (mapButton != null)
            mapButton.onClick.RemoveListener(OpenMap);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseMap);

        if (infoCloseButton != null)
            infoCloseButton.onClick.RemoveListener(CloseConstellationInfo);
    }

    public void ResetMap()
    {
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                    slots[i].SetEmpty();
            }
        }

        CloseConstellationInfo();

        if (mapPanel != null)
            mapPanel.SetActive(false);

        isOpen = false;
        SetHiddenObjectsVisible(true);
    }

    public void SetConstellationCompleted(
        int index,
        string constellationName,
        string constellationDescription
    )
    {
        if (slots == null)
            return;

        if (index < 0 || index >= slots.Length)
            return;

        if (slots[index] != null)
        {
            slots[index].SetCompleted(
                constellationName,
                constellationDescription
            );
        }
    }

    public void OpenMap()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (mapPanel != null)
            mapPanel.SetActive(true);

        SetHiddenObjectsVisible(false);

        OnMapOpened?.Invoke();
    }

    public void CloseMap()
    {
        if (!isOpen)
            return;

        CloseConstellationInfo();

        isOpen = false;

        if (mapPanel != null)
            mapPanel.SetActive(false);

        SetHiddenObjectsVisible(true);

        OnMapClosed?.Invoke();
    }

    public void OpenConstellationInfo(
        string constellationName,
        string constellationDescription
    )
    {
        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (infoNameText != null)
            infoNameText.text = constellationName;

        if (infoDescriptionText != null)
            infoDescriptionText.text = constellationDescription;
    }

    public void CloseConstellationInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void SetHiddenObjectsVisible(bool value)
    {
        if (hideWhileMapOpen == null)
            return;

        for (int i = 0; i < hideWhileMapOpen.Length; i++)
        {
            if (hideWhileMapOpen[i] != null)
                hideWhileMapOpen[i].SetActive(value);
        }
    }
}