using System;
using TMPro;
using UnityEngine;

public class PuzzleTimer : MonoBehaviour
{
    public event Action OnTimeExpired;

    [Header("Timer")]
    [SerializeField] private float startSeconds = 300f;
    [SerializeField] private TMP_Text timerText;

    private float currentSeconds;
    private bool isRunning;
    private bool isExpired;

    private void Awake()
    {
        currentSeconds = startSeconds;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!isRunning || isExpired)
            return;

        currentSeconds -= Time.deltaTime;

        if (currentSeconds <= 0f)
        {
            currentSeconds = 0f;
            isRunning = false;
            isExpired = true;

            UpdateTimerText();
            OnTimeExpired?.Invoke();
            return;
        }

        UpdateTimerText();
    }

    public void StartTimer()
    {
        currentSeconds = startSeconds;
        isExpired = false;
        isRunning = true;
        UpdateTimerText();
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (!isExpired)
            isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentSeconds / 60f);
        int seconds = Mathf.FloorToInt(currentSeconds % 60f);

        timerText.text = $"{minutes:0}:{seconds:00}";
    }
}