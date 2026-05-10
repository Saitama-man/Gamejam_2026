using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleGameManager : MonoBehaviour
{
    [System.Serializable]
    public class ConstellationLevel
    {
        public string constellationName;
        public GameObject constellationPrefab;
        public Sprite zodiacSprite;
        public ConstellationPuzzleMVP.StarLine[] starLines;
    }

    [Header("Levels")]
    [SerializeField] private ConstellationLevel[] levels;
    [SerializeField] private Transform constellationHolder;

    [Header("Controllers")]
    [SerializeField] private ConstellationPuzzleMVP puzzle;
    [SerializeField] private PuzzleTimer timer;
    [SerializeField] private PuzzleMapUI mapUI;
    [SerializeField] private BlenderStyleOrbitCamera orbitCamera;

    [Header("Camera Check")]
    [SerializeField] private Transform constellationCenter;
    [SerializeField] private Transform sharedCorrectViewPoint;

    [Header("Camera Random Start")]
    [SerializeField] private float minStartAngleFromCorrectPoint = 50f;

    [Header("Cutscenes")]
    [SerializeField] private string winSceneName;
    [SerializeField] private string loseSceneName;

    [Header("Flow")]
    [SerializeField] private float delayBeforeNextConstellation = 1.5f;

    private int currentLevelIndex;
    private GameObject currentConstellation;
    private bool gameFinished;

    private void Start()
    {
        if (puzzle != null)
            puzzle.OnPuzzleSolved += OnPuzzleSolved;

        if (timer != null)
            timer.OnTimeExpired += LoseGame;

        if (mapUI != null)
        {
            mapUI.OnMapOpened += PauseTimer;
            mapUI.OnMapClosed += ResumeTimer;
            mapUI.ResetMap();
        }

        currentLevelIndex = 0;

        if (timer != null)
            timer.StartTimer();

        LoadCurrentLevel();
    }

    private void OnDestroy()
    {
        if (puzzle != null)
            puzzle.OnPuzzleSolved -= OnPuzzleSolved;

        if (timer != null)
            timer.OnTimeExpired -= LoseGame;

        if (mapUI != null)
        {
            mapUI.OnMapOpened -= PauseTimer;
            mapUI.OnMapClosed -= ResumeTimer;
        }
    }

    private void LoadCurrentLevel()
    {
        if (gameFinished)
            return;

        if (currentLevelIndex >= levels.Length)
        {
            WinGame();
            return;
        }

        if (sharedCorrectViewPoint == null)
        {
            Debug.LogError("Shared Correct View Point не назначен в PuzzleGameManager.");
            return;
        }

        if (constellationHolder == null)
        {
            Debug.LogError("Constellation Holder не назначен в PuzzleGameManager.");
            return;
        }

        ConstellationLevel level = levels[currentLevelIndex];

        if (level.constellationPrefab == null)
        {
            Debug.LogError($"У уровня {currentLevelIndex} не назначен Constellation Prefab.");
            return;
        }

        if (currentConstellation != null)
            Destroy(currentConstellation);

        currentConstellation = Instantiate(
            level.constellationPrefab,
            constellationHolder.position,
            constellationHolder.rotation,
            constellationHolder
        );

        Transform[] stars = CollectStars(currentConstellation.transform);

        if (puzzle != null)
        {
            puzzle.SetupLevel(
                level.constellationName,
                stars,
                level.starLines,
                sharedCorrectViewPoint,
                level.zodiacSprite
            );
        }
        else
        {
            Debug.LogError("Puzzle не назначен в PuzzleGameManager.");
        }

        if (orbitCamera != null)
        {
            orbitCamera.SetCanControl(true);

            orbitCamera.RandomizeViewAwayFromCorrectPoint(
                constellationCenter,
                sharedCorrectViewPoint,
                minStartAngleFromCorrectPoint
            );
        }
        else
        {
            Debug.LogWarning("Orbit Camera не назначена в PuzzleGameManager.");
        }
    }

    private Transform[] CollectStars(Transform root)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child == root)
                continue;

            if (child.name.StartsWith("Star") ||
                child.CompareTag("Star") ||
                IsNumericName(child.name))
            {
                result.Add(child);
            }
        }

        result.Sort((a, b) =>
        {
            int aNumber;
            int bNumber;

            bool aIsNumber = int.TryParse(a.name, out aNumber);
            bool bIsNumber = int.TryParse(b.name, out bNumber);

            if (aIsNumber && bIsNumber)
                return aNumber.CompareTo(bNumber);

            return a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
        });

        if (result.Count == 0)
            Debug.LogWarning($"В префабе {root.name} не найдены звёзды. Назови их Star0, Star1... или 0, 1, 2...");

        return result.ToArray();
    }

    private bool IsNumericName(string objectName)
    {
        int number;
        return int.TryParse(objectName, out number);
    }

    private void OnPuzzleSolved(string constellationName, Sprite constellationSprite)
    {
        if (gameFinished)
            return;

        if (mapUI != null)
            mapUI.SetConstellationCompleted(currentLevelIndex, constellationSprite);

        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            WinGame();
        }
        else
        {
            StartCoroutine(LoadNextAfterDelay());
        }
    }

    private IEnumerator LoadNextAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextConstellation);
        LoadCurrentLevel();
    }

    private void PauseTimer()
    {
        if (timer != null && !gameFinished)
            timer.PauseTimer();
    }

    private void ResumeTimer()
    {
        if (timer != null && !gameFinished)
            timer.ResumeTimer();
    }

    private void LoseGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        if (timer != null)
            timer.StopTimer();

        if (!string.IsNullOrEmpty(loseSceneName))
            SceneManager.LoadScene(loseSceneName);
        else
            Debug.Log("Проигрыш. Lose Scene Name не задан.");
    }

    private void WinGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        if (timer != null)
            timer.StopTimer();

        if (!string.IsNullOrEmpty(winSceneName))
            SceneManager.LoadScene(winSceneName);
        else
            Debug.Log("Победа. Win Scene Name не задан.");
    }
}