using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 5)]
    public string text;
}

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Settings")]
    public bool playOnlyOnce = true;

    private bool alreadyPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnlyOnce && alreadyPlayed)
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("В сцене нет DialogueManager. Добавь DialogueCanvas prefab.");
            return;
        }

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning("В DialogueTrigger нет реплик.", this);
            return;
        }

        alreadyPlayed = true;

        DialogueManager.Instance.StartDialogue(dialogueLines);
    }
}