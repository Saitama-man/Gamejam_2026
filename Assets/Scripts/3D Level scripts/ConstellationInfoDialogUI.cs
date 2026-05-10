using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstellationInfoDialogUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button closeButton;

    [SerializeField] private Image artImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        Close();
    }

    public void Open(ConstellationLevelData data)
    {
        if (data == null)
            return;

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);

        if (artImage != null)
            artImage.sprite = data.artImage;

        if (nameText != null)
            nameText.text = data.constellationName;

        if (descriptionText != null)
            descriptionText.text = data.description;
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}