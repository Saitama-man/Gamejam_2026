using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    [Header("Target Dialogue")]
    public DialogueSequence dialogueSequence;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

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

            dialogueSequence.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        Debug.Log("Игрок рядом. Нажми E для диалога.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}