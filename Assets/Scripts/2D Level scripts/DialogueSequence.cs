using UnityEngine;

public class DialogueSequence : MonoBehaviour
{
    [Header("Dialogue Lines")]
    public DialogueLine[] dialogueLines;

    [Header("Settings")]
    public bool playOnlyOnce = true;

    private bool alreadyPlayed = false;

    public void Play()
    {
        if (playOnlyOnce && alreadyPlayed)
            return;

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning("DialogueSequence: нет реплик.", this);
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueSequence: в сцене нет DialogueManager.", this);
            return;
        }

        alreadyPlayed = true;

        DialogueManager.Instance.StartDialogue(dialogueLines);
    }

    public void ResetDialogue()
    {
        alreadyPlayed = false;
    }
}