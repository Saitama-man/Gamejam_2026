using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level3ConstellationManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private GameObject[] constellationLevelPrefabs;
    [SerializeField] private int requiredSolvedLevels = 4;

    [Header("Scene")]
    [SerializeField] private Transform currentConstellationHolder;
    [SerializeField] private CameraSpherePuzzleChecker puzzleChecker;

    [Tooltip("Сюда закинь свой скрипт орбитальной камеры на CameraRig")]
    [SerializeField] private MonoBehaviour orbitCameraScript;

    [Header("Timer")]
    [SerializeField] private float timeLimit = 300f;
    [SerializeField] private TMP_Text timerText;

    [Header("UI")]
    [SerializeField] private StarMapUI starMapUI;
    [SerializeField] private TMP_Text solvedCounterText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Final Panels")]
    [SerializeField] private GameObject globalSuccessPanel;
    [SerializeField] private GameObject globalFailPanel;

    private readonly List<ConstellationLevelData> levelDataList = new List<ConstellationLevelData>();

    private GameObject currentLevelObject;
    private ConstellationLevelData currentLevelData;

    private int currentLevelIndex;
    private int solvedLevels;
    private int totalScore;

    private float currentTime;
    private bool timerPaused;
    private bool levelEnded;
    private bool waitingAfterSolvedLevel;

    private void Start()
    {
        currentTime = timeLimit;

        if (globalSuccessPanel != null)
            globalSuccessPanel.SetActive(false);

        if (globalFailPanel != null)
            globalFailPanel.SetActive(false);

        CacheLevelData();

        if (starMapUI != null)
            starMapUI.Init(levelDataList, this);

        LoadLevel(0);
        UpdateUI();
    }

    private void Update()
    {
        if (levelEnded)
            return;

        if (!timerPaused)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                FinishLevel(solvedLevels >= requiredSolvedLevels);
            }
        }

        UpdateTimerUI();
    }

    private void CacheLevelData()
    {
        levelDataList.Clear();

        foreach (GameObject levelPrefab in constellationLevelPrefabs)
        {
            if (levelPrefab == null)
            {
                Debug.LogError("В списке Constellation Level Prefabs есть пустой элемент.");
                continue;
            }

            ConstellationLevelData data = levelPrefab.GetComponent<ConstellationLevelData>();

            if (data == null)
            {
                Debug.LogError($"На prefab {levelPrefab.name} нет ConstellationLevelData.");
                continue;
            }

            levelDataList.Add(data);
        }
    }

    private void LoadLevel(int index)
    {
        if (index >= constellationLevelPrefabs.Length)
        {
            FinishLevel(solvedLevels >= requiredSolvedLevels);
            return;
        }

        currentLevelIndex = index;
        waitingAfterSolvedLevel = false;
        timerPaused = false;

        ClearCurrentLevel();

        GameObject prefab = constellationLevelPrefabs[currentLevelIndex];

        currentLevelObject = Instantiate(prefab, currentConstellationHolder);
        currentLevelObject.transform.localPosition = Vector3.zero;
        currentLevelObject.transform.localRotation = Quaternion.identity;
        currentLevelObject.transform.localScale = Vector3.one;

        currentLevelData = currentLevelObject.GetComponent<ConstellationLevelData>();

        if (currentLevelData == null)
        {
            Debug.LogError("На созданном уровне нет ConstellationLevelData.");
            return;
        }

        RandomizeCameraStart();

        SetCameraControl(true);

        if (puzzleChecker != null)
            puzzleChecker.StartChecking(currentLevelData);

        UpdateUI();
    }

    private void ClearCurrentLevel()
    {
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
            currentLevelObject = null;
        }
    }

    public void OnCurrentLevelSolved()
    {
        if (levelEnded || waitingAfterSolvedLevel)
            return;

        waitingAfterSolvedLevel = true;
        timerPaused = true;

        solvedLevels++;

        if (currentLevelData != null)
            totalScore += currentLevelData.starPoints;

        if (puzzleChecker != null)
            puzzleChecker.StopChecking();

        SetCameraControl(false);

        if (starMapUI != null)
        {
            starMapUI.UnlockConstellation(currentLevelIndex);
            starMapUI.OpenMapAfterSolved();
        }
        else
        {
            ContinueAfterMap();
        }

        UpdateUI();
    }

    public void ContinueAfterMap()
    {
        if (!waitingAfterSolvedLevel)
            return;

        if (starMapUI != null)
            starMapUI.ForceCloseMap();

        if (solvedLevels >= requiredSolvedLevels)
        {
            FinishLevel(true);
            return;
        }

        LoadLevel(currentLevelIndex + 1);
    }

    public void OpenMapFromButton()
    {
        if (levelEnded)
            return;

        timerPaused = true;

        if (puzzleChecker != null)
            puzzleChecker.StopChecking();

        SetCameraControl(false);

        if (starMapUI != null)
            starMapUI.OpenMapManually();
    }

    public void RequestCloseMap()
    {
        if (levelEnded)
            return;

        if (waitingAfterSolvedLevel)
            return;

        if (starMapUI != null)
            starMapUI.ForceCloseMap();

        timerPaused = false;

        SetCameraControl(true);

        if (puzzleChecker != null && currentLevelData != null)
            puzzleChecker.StartChecking(currentLevelData);
    }

    private void FinishLevel(bool success)
    {
        levelEnded = true;
        timerPaused = true;

        if (puzzleChecker != null)
            puzzleChecker.StopChecking();

        SetCameraControl(false);

        if (starMapUI != null)
            starMapUI.ForceCloseMap();

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

    private void RandomizeCameraStart()
    {
        if (orbitCameraScript == null)
            return;

        Vector2 randomOrbitRotation = new Vector2(
            Random.Range(60f, 250f),
            Random.Range(-50f, 50f)
        );

        orbitCameraScript.SendMessage(
            "SetOrbitRotation",
            randomOrbitRotation,
            SendMessageOptions.DontRequireReceiver
        );
    }

    private void SetCameraControl(bool value)
    {
        if (orbitCameraScript == null)
            return;

        orbitCameraScript.SendMessage(
            "SetCanControl",
            value,
            SendMessageOptions.DontRequireReceiver
        );
    }

    private void UpdateUI()
    {
        UpdateTimerUI();

        if (solvedCounterText != null)
            solvedCounterText.text = $"{solvedLevels}/{constellationLevelPrefabs.Length}";

        if (scoreText != null)
            scoreText.text = $"Очки: {totalScore}";
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}