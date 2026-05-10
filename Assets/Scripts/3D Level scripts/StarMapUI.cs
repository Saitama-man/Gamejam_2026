using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarMapUI : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GameObject fullMapPanel;
    [SerializeField] private ConstellationInfoDialogUI infoDialog;

    [Header("Slots")]
    [SerializeField] private ConstellationMapSlotUI[] slots;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button closeMapButton;

    private List<ConstellationLevelData> levels;
    private Level3ConstellationManager levelManager;
    private bool[] unlockedLevels;

    public void Init(List<ConstellationLevelData> levelDataList, Level3ConstellationManager manager)
    {
        levels = levelDataList;
        levelManager = manager;
        unlockedLevels = new bool[levels.Count];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
                slots[i].Init(i, this);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (closeMapButton != null)
        {
            closeMapButton.onClick.RemoveAllListeners();
            closeMapButton.onClick.AddListener(OnCloseMapClicked);
        }

        ForceCloseMap();
    }

    public void UnlockConstellation(int index)
    {
        if (index < 0 || index >= levels.Count)
            return;

        unlockedLevels[index] = true;

        if (index < slots.Length && slots[index] != null)
        {
            slots[index].SetUnlocked(true, levels[index].mapIcon);
        }
    }

    public void OpenMapAfterSolved()
    {
        OpenMap(showContinueButton: true);
    }

    public void OpenMapManually()
    {
        OpenMap(showContinueButton: false);
    }

    private void OpenMap(bool showContinueButton)
    {
        if (fullMapPanel != null)
            fullMapPanel.SetActive(true);

        if (continueButton != null)
            continueButton.gameObject.SetActive(showContinueButton);

        if (closeMapButton != null)
            closeMapButton.gameObject.SetActive(!showContinueButton);

        if (infoDialog != null)
            infoDialog.Close();
    }

    public void ForceCloseMap()
    {
        if (infoDialog != null)
            infoDialog.Close();

        if (fullMapPanel != null)
            fullMapPanel.SetActive(false);
    }

    public void OpenConstellationDialog(int index)
    {
        if (index < 0 || index >= levels.Count)
            return;

        if (!unlockedLevels[index])
            return;

        if (infoDialog != null)
            infoDialog.Open(levels[index]);
    }

    private void OnContinueClicked()
    {
        if (levelManager != null)
            levelManager.ContinueAfterMap();
    }

    private void OnCloseMapClicked()
    {
        if (levelManager != null)
            levelManager.RequestCloseMap();
    }
}