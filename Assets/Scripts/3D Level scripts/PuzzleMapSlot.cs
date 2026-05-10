using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMapSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image constellationImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Slot View")]
    [SerializeField] private bool hideImageUntilCompleted = true;
    [SerializeField] private bool hideNameUntilCompleted = true;

    private bool isCompleted;
    private string constellationName;
    private string constellationDescription;

    private PuzzleMapUI mapUI;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (constellationImage == null)
            constellationImage = GetComponentInChildren<Image>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    public void Initialize(PuzzleMapUI owner)
    {
        mapUI = owner;
        SetEmpty();
    }

    public void SetEmpty()
    {
        isCompleted = false;
        constellationName = "";
        constellationDescription = "";

        if (constellationImage != null)
            constellationImage.gameObject.SetActive(!hideImageUntilCompleted);

        if (nameText != null)
        {
            nameText.text = "";
            nameText.gameObject.SetActive(!hideNameUntilCompleted);
        }

        if (button != null)
            button.interactable = false;
    }

    public void SetCompleted(string newName, string newDescription)
    {
        isCompleted = true;
        constellationName = newName;
        constellationDescription = newDescription;

        if (constellationImage != null)
            constellationImage.gameObject.SetActive(true);

        if (nameText != null)
        {
            nameText.text = constellationName;
            nameText.gameObject.SetActive(true);
        }

        if (button != null)
            button.interactable = true;
    }

    private void OnClick()
    {
        if (!isCompleted || mapUI == null)
            return;

        mapUI.OpenConstellationInfo(
            constellationName,
            constellationDescription
        );
    }
}