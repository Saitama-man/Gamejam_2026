using TMPro;
using UnityEngine;

public class InteractionHintManager : MonoBehaviour
{
	public static InteractionHintManager Instance { get; private set; }

	[Header("UI")]
	[SerializeField] private GameObject hintPanel;
	[SerializeField] private TMP_Text hintText;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Start()
	{
		Hide();
	}

	public void Show(string text)
	{
		if (hintPanel == null || hintText == null)
		{
			Debug.LogWarning("InteractionHintManager: Hint Panel или Hint Text не назначены.", this);
			return;
		}

		hintText.text = text;
		hintPanel.SetActive(true);
	}

	public void Hide()
	{
		if (hintPanel != null)
		{
			hintPanel.SetActive(false);
		}
	}
}