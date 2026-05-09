using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Target Dialogue")]
    public DialogueSequence dialogueSequence;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (dialogueSequence == null)
        {
            Debug.LogError("DialogueTrigger: не назначен DialogueSequence.", this);
            return;
        }

        dialogueSequence.Play();
    }
}