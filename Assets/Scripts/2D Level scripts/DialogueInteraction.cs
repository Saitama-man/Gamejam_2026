using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
	[Header("Target Dialogue")]
	[SerializeField] private DialogueSequence dialogueSequence;

	[Header("Interaction")]
	[SerializeField] private KeyCode interactKey = KeyCode.E;
	[SerializeField] private string interactionHint = "E Ч поговорить";

	private bool playerInside = false;

	private void Update()
	{
		if (!playerInside)
			return;

		if (Input.GetKeyDown(interactKey))
		{
			if (dialogueSequence == null)
			{
				Debug.LogError("DialogueInteraction: не назначен DialogueSequence.", this);
				return;
			}

			if (!dialogueSequence.CanPlay())
			{
				InteractionHintManager.Instance?.Hide();
				return;
			}

			InteractionHintManager.Instance?.Hide();
			dialogueSequence.Play();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		playerInside = true;

		if (dialogueSequence != null && dialogueSequence.CanPlay())
		{
			InteractionHintManager.Instance?.Show(interactionHint);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		playerInside = false;

		InteractionHintManager.Instance?.Hide();
	}
}