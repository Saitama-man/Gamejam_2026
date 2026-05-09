using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;
    public KeyCode nextKey = KeyCode.Space;

    private string[] currentLines;
    private string currentSpeaker;
    private int currentLineIndex;

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        if (Input.GetKeyDown(nextKey) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                FinishTypingInstantly();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }

        currentSpeaker = speakerName;
        currentLines = lines;
        currentLineIndex = 0;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        speakerNameText.text = currentSpeaker;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void FinishTypingInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentLines[currentLineIndex];
        isTyping = false;
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;

        dialoguePanel.SetActive(false);

        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }
    }
}