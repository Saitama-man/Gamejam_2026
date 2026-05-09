using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class ConstellationStage
{
    [Header("Main")]
    public string constellationName;
    public GameObject constellationPrefab;

    [TextArea(3, 7)]
    public string description;

    [Header("Visual")]
    public Sprite constellationArt;

    [Header("Placement")]
    public Vector3 prefabLocalPosition;
    public Vector3 prefabLocalRotation;
    public Vector3 prefabLocalScale = Vector3.one;

    [Header("Correct Camera Zone")]
    public Vector3 correctViewPointLocalPosition = new Vector3(0f, 0f, -10f);
    public float correctZoneAngle = 15f;

    [Header("Reward")]
    public int starPoints = 100;
}

public class ConstellationLevelManager : MonoBehaviour
{
    [Header("Stages")]
    [SerializeField] private List<ConstellationStage> stages = new List<ConstellationStage>();

    [Header("Scene Links")]
    [SerializeField] private Transform constellationHolder;
    [SerializeField] private Transform correctViewPoint;
    [SerializeField] private CameraSpherePuzzleChecker puzzleChecker;
    [SerializeField] private BlenderStyleOrbitCamera orbitCamera;

    [Header("Timer")]
    [SerializeField] private float timeLimit = 300f;
    [SerializeField] private TMP_Text timerText;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text mapProgressText;

    [Header("Result UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultArtImage;
    [SerializeField] private TMP_Text resultTitleText;
    [SerializeField] private TMP_Text resultDescriptionText;
    [SerializeField] private TMP_Text resultRewardText;

    [Header("Map UI")]
    [SerializeField] private GameObject[] solvedMapMarks;

    [Header("Final UI")]
    [SerializeField] private GameObject globalSuccessPanel;
    [SerializeField] private GameObject globalFailPanel;

    private GameObject currentConstellationObject;

    private int currentStageIndex;
    private int completedCount;
    private int totalScore;

    private float currentTime;
    private bool levelEnded;
    private bool waitingForContinue;

    private int RequiredCompletedCount => Mathf.CeilToInt(stages.Count * 0.8f);

    private void Start()
    {
        currentTime = timeLimit;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (globalSuccessPanel != null)
            globalSuccessPanel.SetActive(false);

        if (globalFailPanel != null)
            globalFailPanel.SetActive(false);

        HideAllMapMarks();

        LoadStage(0);
        UpdateUI();
    }

    private void Update()
    {
        if (levelEnded)
            return;

        if (!waitingForContinue)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                FinishLevelByTime();
            }
        }

        UpdateTimerUI();
    }

    private void LoadStage(int index)
    {
        if (index >= stages.Count)
        {
            FinishLevel(true);
            return;
        }

        currentStageIndex = index;
        ConstellationStage stage = stages[currentStageIndex];

        ClearCurrentConstellation();

        currentConstellationObject = Instantiate(stage.constellationPrefab, constellationHolder);

        currentConstellationObject.transform.localPosition = stage.prefabLocalPosition;
        currentConstellationObject.transform.localEulerAngles = stage.prefabLocalRotation;
        currentConstellationObject.transform.localScale = stage.prefabLocalScale;

        correctViewPoint.localPosition = stage.correctViewPointLocalPosition;

        RandomizeCameraStart();

        waitingForContinue = false;

        puzzleChecker.StartChecking(correctViewPoint, stage.correctZoneAngle);

        UpdateUI();
    }

    private void ClearCurrentConstellation()
    {
        if (currentConstellationObject != null)
        {
            Destroy(currentConstellationObject);
            currentConstellationObject = null;
        }
    }

    private void RandomizeCameraStart()
    {
        if (orbitCamera == null)
            return;

        Vector2 randomRotation = new Vector2(
            UnityEngine.Random.Range(60f, 250f),
            UnityEngine.Random.Range(-50f, 50f)
        );

        orbitCamera.SetOrbitRotation(randomRotation);
    }

    public void OnCurrentConstellationSolved()
    {
        if (levelEnded || waitingForContinue)
            return;

        waitingForContinue = true;

        ConstellationStage stage = stages[currentStageIndex];

        completedCount++;
        totalScore += stage.starPoints;

        OpenMapMark(currentStageIndex);
        ShowConstellationResult(stage);
        UpdateUI();
    }

    private void ShowConstellationResult(ConstellationStage stage)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultArtImage != null)
            resultArtImage.sprite = stage.constellationArt;

        if (resultTitleText != null)
            resultTitleText.text = stage.constellationName;

        if (resultDescriptionText != null)
            resultDescriptionText.text = stage.description;

        if (resultRewardText != null)
            resultRewardText.text = $"+{stage.starPoints} звездных очков";
    }

    public void ContinueToNextConstellation()
    {
        if (!waitingForContinue)
            return;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        int nextIndex = currentStageIndex + 1;

        if (nextIndex >= stages.Count)
        {
            FinishLevel(true);
        }
        else
        {
            LoadStage(nextIndex);
        }
    }

    private void FinishLevelByTime()
    {
        bool enoughMapCompleted = completedCount >= RequiredCompletedCount;
        FinishLevel(enoughMapCompleted);
    }

    private void FinishLevel(bool success)
    {
        levelEnded = true;
        waitingForContinue = false;

        if (puzzleChecker != null)
            puzzleChecker.StopChecking();

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (success)
        {
            if (globalSuccessPanel != null)
                globalSuccessPanel.SetActive(true);
        }
        else
        {
            if (globalFailPanel != null)
                globalFailPanel.SetActive(true);
        }
    }

    private void HideAllMapMarks()
    {
        foreach (GameObject mark in solvedMapMarks)
        {
            if (mark != null)
                mark.SetActive(false);
        }
    }

    private void OpenMapMark(int index)
    {
        if (index < 0 || index >= solvedMapMarks.Length)
            return;

        if (solvedMapMarks[index] != null)
            solvedMapMarks[index].SetActive(true);
    }

    private void UpdateUI()
    {
        UpdateTimerUI();

        if (scoreText != null)
            scoreText.text = $"Звездные очки: {totalScore}";

        if (mapProgressText != null)
        {
            float mapPercent = (float)completedCount / stages.Count * 100f;
            mapProgressText.text = $"Карта восстановлена: {mapPercent:0}%";
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}