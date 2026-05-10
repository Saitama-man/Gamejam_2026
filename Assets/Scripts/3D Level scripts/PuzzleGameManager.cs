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

    [Header("Constellation Spawn")]
    [SerializeField] private bool centerConstellationByStars = true;
    [SerializeField] private bool resetPrefabRotationOnSpawn = true;
    [SerializeField] private bool resetPrefabScaleOnSpawn = true;
    [SerializeField] private bool clearHolderBeforeSpawn = true;

    [Header("Camera Random Start")]
    [SerializeField] private float minStartAngleFromCorrectPoint = 50f;

    [Header("Cutscenes")]
    [SerializeField] private string winSceneName;
    [SerializeField] private string loseSceneName;

    [Header("Flow")]
    [SerializeField] private float delayBeforeNextConstellation = 1.5f;

    private const string ConstellationCenterName = "Constellation Center";
    private const string CorrectViewPointName = "CorrectViewPoint";
    private const string HolderName = "CurrentConstellationHolder";

    private int currentLevelIndex;
    private GameObject currentConstellation;
    private bool gameFinished;
    private Coroutine nextLevelCoroutine;

    private void Start()
    {
        ResolveSceneReferences();

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

        if (clearHolderBeforeSpawn)
            ClearConstellationHolder();

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

        ResolveSceneReferences();

        if (currentLevelIndex >= levels.Length)
        {
            WinGame();
            return;
        }

        if (!ValidateReferences())
            return;

        ConstellationLevel level = levels[currentLevelIndex];

        if (level.constellationPrefab == null)
        {
            Debug.LogError($"У уровня {currentLevelIndex} не назначен Constellation Prefab.");
            return;
        }

        if (clearHolderBeforeSpawn)
            ClearConstellationHolder();
        else
            DestroyCurrentConstellation();

        currentConstellation = Instantiate(level.constellationPrefab, constellationHolder);

        if (!string.IsNullOrEmpty(level.constellationName))
            currentConstellation.name = level.constellationName;

        currentConstellation.transform.localPosition = Vector3.zero;

        if (resetPrefabRotationOnSpawn)
            currentConstellation.transform.localRotation = Quaternion.identity;

        if (resetPrefabScaleOnSpawn)
            currentConstellation.transform.localScale = Vector3.one;

        Transform[] stars = CollectStars(currentConstellation.transform);

        if (centerConstellationByStars)
            CenterConstellationByStars(stars);

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

        if (orbitCamera != null)
        {
            orbitCamera.SetCanControl(true);

            orbitCamera.RandomizeViewAwayFromCorrectPoint(
                constellationCenter,
                sharedCorrectViewPoint,
                minStartAngleFromCorrectPoint
            );
        }
    }

    private void ResolveSceneReferences()
    {
        if (constellationCenter == null)
        {
            GameObject centerObject = GameObject.Find(ConstellationCenterName);

            if (centerObject != null)
                constellationCenter = centerObject.transform;
        }

        if (sharedCorrectViewPoint == null)
        {
            GameObject correctObject = GameObject.Find(CorrectViewPointName);

            if (correctObject != null)
                sharedCorrectViewPoint = correctObject.transform;
        }

        bool holderIsWrong =
            constellationHolder == null ||
            constellationHolder == sharedCorrectViewPoint ||
            constellationHolder == constellationCenter ||
            constellationHolder.name == CorrectViewPointName ||
            constellationHolder.name.Contains("CorrectViewPoint");

        if (holderIsWrong)
        {
            Transform foundHolder = null;

            if (constellationCenter != null)
                foundHolder = constellationCenter.Find(HolderName);

            if (foundHolder == null)
            {
                GameObject holderObject = GameObject.Find(HolderName);

                if (holderObject != null)
                    foundHolder = holderObject.transform;
            }

            if (foundHolder != null)
            {
                constellationHolder = foundHolder;
                Debug.Log($"Constellation Holder автоматически назначен: {constellationHolder.name}");
            }
        }

        if (constellationHolder == null && constellationCenter != null)
        {
            GameObject newHolder = new GameObject(HolderName);
            newHolder.transform.SetParent(constellationCenter);
            newHolder.transform.localPosition = Vector3.zero;
            newHolder.transform.localRotation = Quaternion.identity;
            newHolder.transform.localScale = Vector3.one;

            constellationHolder = newHolder.transform;

            Debug.LogWarning("CurrentConstellationHolder не найден, поэтому был создан автоматически.");
        }
    }

    private bool ValidateReferences()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("Levels пустой. Добавь созвездия в PuzzleGameManager.");
            return false;
        }

        if (constellationCenter == null)
        {
            Debug.LogError("Constellation Center не назначен.");
            return false;
        }

        if (sharedCorrectViewPoint == null)
        {
            Debug.LogError("Shared Correct View Point не назначен.");
            return false;
        }

        if (constellationHolder == null)
        {
            Debug.LogError("Constellation Holder не назначен.");
            return false;
        }

        if (constellationHolder == sharedCorrectViewPoint)
        {
            Debug.LogError("Constellation Holder всё ещё равен CorrectViewPoint. Назначь CurrentConstellationHolder.");
            return false;
        }

        if (constellationHolder == constellationCenter)
        {
            Debug.LogError("Constellation Holder не должен быть Constellation Center. Назначь CurrentConstellationHolder.");
            return false;
        }

        if (puzzle == null)
        {
            Debug.LogError("Puzzle не назначен.");
            return false;
        }

        return true;
    }

    private void DestroyCurrentConstellation()
    {
        if (currentConstellation != null)
        {
            Destroy(currentConstellation);
            currentConstellation = null;
        }
    }

    private void ClearConstellationHolder()
    {
        if (constellationHolder == null)
            return;

        for (int i = constellationHolder.childCount - 1; i >= 0; i--)
        {
            Transform child = constellationHolder.GetChild(i);

            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        currentConstellation = null;
    }

    private Transform[] CollectStars(Transform root)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child == root)
                continue;

            if (IsStarTransform(child))
                result.Add(child);
        }

        result.Sort((a, b) =>
        {
            int aNumber;
            int bNumber;

            bool aHasNumber = TryExtractNumber(a.name, out aNumber);
            bool bHasNumber = TryExtractNumber(b.name, out bNumber);

            if (aHasNumber && bHasNumber)
                return aNumber.CompareTo(bNumber);

            return a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
        });

        if (result.Count == 0)
        {
            Debug.LogWarning(
                $"В префабе {root.name} не найдены звёзды. " +
                "Назови дочерние объекты Star, Star0, Star1... или 0, 1, 2..."
            );
        }

        return result.ToArray();
    }

    private bool IsStarTransform(Transform target)
    {
        string objectName = target.name.Trim();

        if (objectName == "Star")
            return true;

        if (objectName.StartsWith("Star"))
            return true;

        if (IsNumericName(objectName))
            return true;

        return false;
    }

    private void CenterConstellationByStars(Transform[] stars)
    {
        if (stars == null || stars.Length == 0 || currentConstellation == null)
            return;

        Bounds bounds = new Bounds(stars[0].position, Vector3.zero);

        for (int i = 1; i < stars.Length; i++)
        {
            if (stars[i] != null)
                bounds.Encapsulate(stars[i].position);
        }

        Vector3 starsCenter = bounds.center;
        Vector3 targetCenter = constellationCenter.position;
        Vector3 offset = targetCenter - starsCenter;

        currentConstellation.transform.position += offset;
    }

    private bool IsNumericName(string objectName)
    {
        int number;
        return int.TryParse(objectName, out number);
    }

    private bool TryExtractNumber(string objectName, out int number)
    {
        if (int.TryParse(objectName, out number))
            return true;

        string digits = "";

        for (int i = 0; i < objectName.Length; i++)
        {
            if (char.IsDigit(objectName[i]))
                digits += objectName[i];
        }

        if (!string.IsNullOrEmpty(digits))
            return int.TryParse(digits, out number);

        number = 0;
        return false;
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
            if (nextLevelCoroutine != null)
                StopCoroutine(nextLevelCoroutine);

            nextLevelCoroutine = StartCoroutine(LoadNextAfterDelay());
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

        if (nextLevelCoroutine != null)
            StopCoroutine(nextLevelCoroutine);

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

        if (nextLevelCoroutine != null)
            StopCoroutine(nextLevelCoroutine);

        if (timer != null)
            timer.StopTimer();

        if (!string.IsNullOrEmpty(winSceneName))
            SceneManager.LoadScene(winSceneName);
        else
            Debug.Log("Победа. Win Scene Name не задан.");
    }
}