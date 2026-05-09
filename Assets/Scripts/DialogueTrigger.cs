using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueManager dialogueManager;
    public string speakerName = "Ребёнок";

    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Settings")]
    public bool playOnlyOnce = true;

    private bool alreadyPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnlyOnce && alreadyPlayed)
            return;

        alreadyPlayed = true;

        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(speakerName, dialogueLines);
        }
        else
        {
            Debug.LogError("DialogueManager не назначен в DialogueTrigger!");
        }
    }
}