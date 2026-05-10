using UnityEngine;
using UnityEngine.UI;

public class PuzzleMapSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite emptySprite;

    private void Awake()
    {
        if (slotImage == null)
            slotImage = GetComponent<Image>();
    }

    public void SetEmpty()
    {
        if (slotImage == null)
            return;

        slotImage.sprite = emptySprite;
        slotImage.color = Color.white;
    }

    public void SetCompleted(Sprite constellationSprite)
    {
        if (slotImage == null || constellationSprite == null)
            return;

        slotImage.sprite = constellationSprite;
        slotImage.color = Color.white;
    }
}