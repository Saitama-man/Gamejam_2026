using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;
    public KeyCode nextKey = KeyCode.Space;

    private DialogueLine[] currentLines;
    private int currentLineIndex;

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private PlayerMovement currentPlayerMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("В сцене найден второй DialogueManager. Лишний будет удалён.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
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

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (isDialogueActive)
            return;

        if (lines == null || lines.Length == 0)
            return;

        currentPlayerMovement = FindFirstObjectByType<PlayerMovement>();

        if (currentPlayerMovement != null)
        {
            currentPlayerMovement.SetMovementEnabled(false);
        }

        currentLines = lines;
        currentLineIndex = 0;

        isDialogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLines == null || currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentLines[currentLineIndex];

        if (speakerNameText != null)
        {
            speakerNameText.text = line.speakerName;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string lineText)
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        foreach (char letter in lineText)
        {
            if (dialogueText != null)
            {
                dialogueText.text += letter;
            }

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

        DialogueLine line = currentLines[currentLineIndex];

        if (dialogueText != null)
        {
            dialogueText.text = line.text;
        }

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

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (currentPlayerMovement != null)
        {
            currentPlayerMovement.SetMovementEnabled(true);
        }

        currentPlayerMovement = null;
        currentLines = null;
        currentLineIndex = 0;
    }
}