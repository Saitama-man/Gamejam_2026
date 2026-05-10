using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstellationPuzzleMVP : MonoBehaviour
{
    public event Action<string, Sprite> OnPuzzleSolved;

    [System.Serializable]
    public struct StarLine
    {
        public int from;
        public int to;
    }

    [Header("Camera Check")]
    [SerializeField] private Transform constellationCenter;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform correctViewPoint;

    [Header("Solve Settings")]
    [SerializeField] private float correctZoneAngle = 7f;
    [SerializeField] private float holdTimeToSolve = 1f;

    [Header("Camera Control")]
    [SerializeField] private MonoBehaviour orbitCameraScript;

    [Header("Success Camera Pullback")]
    [SerializeField] private bool useSuccessCameraPullback = true;
    [SerializeField] private float successCameraDistance = 16f;
    [SerializeField] private float successCameraPullbackDuration = 1.2f;

    [Header("Constellation Lines")]
    [SerializeField] private Transform[] starPoints;
    [SerializeField] private StarLine[] starLines;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.04f;

    [Header("Zodiac Image Fade")]
    [SerializeField] private Image zodiacImage;
    [SerializeField] private CanvasGroup zodiacCanvasGroup;
    [SerializeField] private TMP_Text zodiacNameText;
    [SerializeField] private CanvasGroup zodiacNameCanvasGroup;
    [SerializeField] private float zodiacFadeDuration = 1f;

    [Header("Hot Cold Star")]
    [SerializeField] private Image hotColdFillImage;
    [SerializeField] private Image hotColdGlowImage;
    [SerializeField] private float angleForEmptyStar = 75f;
    [SerializeField] private bool fillStarOnSolved = true;

    [Header("Hot Cold Glow")]
    [SerializeField] private bool useGlow = true;
    [SerializeField] private float glowMaxAlpha = 0.65f;
    [SerializeField] private float glowPulseSpeed = 4f;
    [SerializeField] private float glowPulseStrength = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool drawDebugGizmos = true;

    private float holdTimer;
    private bool solved;
    private bool levelCompletionAlreadySent;

    private Transform linesRoot;
    private Coroutine zodiacFadeCoroutine;

    private string currentConstellationName;
    private Sprite currentConstellationSprite;

    private void Start()
    {
        PrepareZodiacVisuals();
        HideZodiacVisualsImmediate();
        PrepareHotColdStar();
        RemoveOldLines();
    }

    private void Update()
    {
        if (solved)
            return;

        float angle = GetCameraAngleToCorrectPoint();

        UpdateHotColdStar(angle);

        if (angle <= correctZoneAngle)
        {
            holdTimer += Time.deltaTime;

            if (showDebugLogs)
            {
                Debug.Log(
                    $"В правильной зоне: {angle:0.00}°, " +
                    $"удержание {holdTimer:0.00}/{holdTimeToSolve:0.00}"
                );
            }

            if (holdTimer >= holdTimeToSolve)
                SolvePuzzle();
        }
        else
        {
            holdTimer = 0f;
        }
    }

    public void SetupLevel(
        string constellationName,
        Transform[] newStarPoints,
        StarLine[] newStarLines,
        Transform newCorrectViewPoint,
        Sprite newZodiacSprite
    )
    {
        StopZodiacFade();

        currentConstellationName = constellationName;
        currentConstellationSprite = newZodiacSprite;

        starPoints = newStarPoints;
        starLines = newStarLines;
        correctViewPoint = newCorrectViewPoint;

        solved = false;
        levelCompletionAlreadySent = false;
        holdTimer = 0f;

        RemoveOldLines();

        PrepareZodiacVisuals();

        if (zodiacImage != null)
            zodiacImage.sprite = newZodiacSprite;

        if (zodiacNameText != null)
            zodiacNameText.text = constellationName;

        HideZodiacVisualsImmediate();
        ResetHotColdStar();

        if (orbitCameraScript != null)
        {
            orbitCameraScript.SendMessage(
                "SetCanControl",
                true,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    private float GetCameraAngleToCorrectPoint()
    {
        if (constellationCenter == null)
        {
            Debug.LogError("Constellation Center не назначен.");
            return 180f;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera не назначена.");
            return 180f;
        }

        if (correctViewPoint == null)
        {
            Debug.LogError("Correct View Point не назначен.");
            return 180f;
        }

        Vector3 currentDirection =
            (mainCamera.position - constellationCenter.position).normalized;

        Vector3 correctDirection =
            (correctViewPoint.position - constellationCenter.position).normalized;

        return Vector3.Angle(currentDirection, correctDirection);
    }

    private void SolvePuzzle()
    {
        if (solved)
            return;

        solved = true;
        holdTimer = 0f;

        if (showDebugLogs)
            Debug.Log("Созвездие решено!");

        if (fillStarOnSolved)
        {
            if (hotColdFillImage != null)
                hotColdFillImage.fillAmount = 1f;

            if (hotColdGlowImage != null)
            {
                hotColdGlowImage.fillAmount = 1f;
                SetImageAlpha(hotColdGlowImage, glowMaxAlpha);
            }
        }

        DisableCameraControl();
        StartSuccessCameraPullback();
        DrawConstellationLines();

        StopZodiacFade();
        zodiacFadeCoroutine = StartCoroutine(FadeInZodiacVisualsThenCompleteLevel());
    }

    private void StartSuccessCameraPullback()
    {
        if (!useSuccessCameraPullback)
            return;

        if (orbitCameraScript is BlenderStyleOrbitCamera orbitCamera)
        {
            orbitCamera.SmoothZoomToDistance(
                successCameraDistance,
                successCameraPullbackDuration
            );
        }
    }

    private void CompleteLevelAfterVisuals()
    {
        if (levelCompletionAlreadySent)
            return;

        levelCompletionAlreadySent = true;
        OnPuzzleSolved?.Invoke(currentConstellationName, currentConstellationSprite);
    }

    private void DisableCameraControl()
    {
        if (orbitCameraScript == null)
            return;

        orbitCameraScript.SendMessage(
            "SetCanControl",
            false,
            SendMessageOptions.DontRequireReceiver
        );
    }

    private void DrawConstellationLines()
    {
        if (starPoints == null || starPoints.Length < 2)
        {
            Debug.LogWarning("Недостаточно точек для линий созвездия.");
            return;
        }

        if (starLines == null || starLines.Length == 0)
        {
            Debug.LogWarning("Не заданы связи Star Lines.");
            return;
        }

        if (linesRoot != null)
            Destroy(linesRoot.gameObject);

        GameObject rootObject = new GameObject("SolvedConstellationLines");
        rootObject.transform.SetParent(constellationCenter, true);
        linesRoot = rootObject.transform;

        for (int i = 0; i < starLines.Length; i++)
        {
            int from = starLines[i].from;
            int to = starLines[i].to;

            if (!IsValidStarIndex(from) || !IsValidStarIndex(to))
            {
                Debug.LogWarning($"Неверная связь линии {i}: {from} → {to}");
                continue;
            }

            CreateLineSegment(
                starPoints[from].position,
                starPoints[to].position,
                $"Line_{from}_{to}"
            );
        }
    }

    private void CreateLineSegment(Vector3 start, Vector3 end, string lineName)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(linesRoot, true);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.numCapVertices = 4;
        lineRenderer.numCornerVertices = 4;

        if (lineMaterial != null)
            lineRenderer.material = lineMaterial;
        else
            Debug.LogWarning("Line Material не назначен.");
    }

    private bool IsValidStarIndex(int index)
    {
        return index >= 0 &&
               index < starPoints.Length &&
               starPoints[index] != null;
    }

    private void PrepareZodiacVisuals()
    {
        if (zodiacImage != null)
        {
            if (zodiacCanvasGroup == null)
                zodiacCanvasGroup = zodiacImage.GetComponent<CanvasGroup>();

            if (zodiacCanvasGroup == null)
                zodiacCanvasGroup = zodiacImage.gameObject.AddComponent<CanvasGroup>();
        }

        if (zodiacNameText != null)
        {
            if (zodiacNameCanvasGroup == null)
                zodiacNameCanvasGroup = zodiacNameText.GetComponent<CanvasGroup>();

            if (zodiacNameCanvasGroup == null)
                zodiacNameCanvasGroup = zodiacNameText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void HideZodiacVisualsImmediate()
    {
        if (zodiacCanvasGroup != null)
            zodiacCanvasGroup.alpha = 0f;

        if (zodiacImage != null)
            zodiacImage.gameObject.SetActive(false);

        if (zodiacNameCanvasGroup != null)
            zodiacNameCanvasGroup.alpha = 0f;

        if (zodiacNameText != null)
            zodiacNameText.gameObject.SetActive(false);
    }

    private void ShowZodiacVisualsImmediate()
    {
        if (zodiacImage != null)
            zodiacImage.gameObject.SetActive(true);

        if (zodiacCanvasGroup != null)
            zodiacCanvasGroup.alpha = 0f;

        if (zodiacNameText != null)
            zodiacNameText.gameObject.SetActive(true);

        if (zodiacNameCanvasGroup != null)
            zodiacNameCanvasGroup.alpha = 0f;
    }

    private void StopZodiacFade()
    {
        if (zodiacFadeCoroutine != null)
        {
            StopCoroutine(zodiacFadeCoroutine);
            zodiacFadeCoroutine = null;
        }
    }

    private IEnumerator FadeInZodiacVisualsThenCompleteLevel()
    {
        bool hasImage = zodiacImage != null;
        bool hasName = zodiacNameText != null;

        if (!hasImage && !hasName)
        {
            CompleteLevelAfterVisuals();
            yield break;
        }

        ShowZodiacVisualsImmediate();

        if (zodiacFadeDuration <= 0f)
        {
            if (zodiacCanvasGroup != null)
                zodiacCanvasGroup.alpha = 1f;

            if (zodiacNameCanvasGroup != null)
                zodiacNameCanvasGroup.alpha = 1f;

            CompleteLevelAfterVisuals();
            yield break;
        }

        float timer = 0f;

        while (timer < zodiacFadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / zodiacFadeDuration);

            if (zodiacCanvasGroup != null)
                zodiacCanvasGroup.alpha = alpha;

            if (zodiacNameCanvasGroup != null)
                zodiacNameCanvasGroup.alpha = alpha;

            yield return null;
        }

        if (zodiacCanvasGroup != null)
            zodiacCanvasGroup.alpha = 1f;

        if (zodiacNameCanvasGroup != null)
            zodiacNameCanvasGroup.alpha = 1f;

        zodiacFadeCoroutine = null;
        CompleteLevelAfterVisuals();
    }

    private void PrepareHotColdStar()
    {
        PrepareFilledImage(hotColdFillImage, true);
        PrepareFilledImage(hotColdGlowImage, false);
    }

    private void PrepareFilledImage(Image image, bool fullAlpha)
    {
        if (image == null)
            return;

        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = (int)Image.OriginVertical.Bottom;
        image.fillAmount = 0f;

        if (fullAlpha)
            SetImageAlpha(image, 1f);
        else
            SetImageAlpha(image, 0f);
    }

    private void ResetHotColdStar()
    {
        if (hotColdFillImage != null)
            hotColdFillImage.fillAmount = 0f;

        if (hotColdGlowImage != null)
        {
            hotColdGlowImage.fillAmount = 0f;
            SetImageAlpha(hotColdGlowImage, 0f);
        }
    }

    private void UpdateHotColdStar(float angle)
    {
        float safeEmptyAngle = Mathf.Max(correctZoneAngle + 1f, angleForEmptyStar);

        float progress = 1f - angle / safeEmptyAngle;
        progress = Mathf.Clamp01(progress);

        if (hotColdFillImage != null)
            hotColdFillImage.fillAmount = progress;

        if (hotColdGlowImage != null)
        {
            hotColdGlowImage.fillAmount = progress;

            if (useGlow)
            {
                float pulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed) * glowPulseStrength;
                float alpha = Mathf.Clamp01(progress * glowMaxAlpha * pulse);
                SetImageAlpha(hotColdGlowImage, alpha);
            }
            else
            {
                SetImageAlpha(hotColdGlowImage, progress * glowMaxAlpha);
            }
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }

    private void RemoveOldLines()
    {
        if (constellationCenter == null)
            return;

        Transform oldLines = constellationCenter.Find("SolvedConstellationLines");

        if (oldLines != null)
            Destroy(oldLines.gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawDebugGizmos)
            return;

        if (constellationCenter != null && correctViewPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(constellationCenter.position, correctViewPoint.position);
            Gizmos.DrawWireSphere(correctViewPoint.position, 0.4f);
        }

        if (starPoints == null || starLines == null)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < starLines.Length; i++)
        {
            int from = starLines[i].from;
            int to = starLines[i].to;

            if (from < 0 || to < 0)
                continue;

            if (from >= starPoints.Length || to >= starPoints.Length)
                continue;

            if (starPoints[from] == null || starPoints[to] == null)
                continue;

            Gizmos.DrawLine(starPoints[from].position, starPoints[to].position);
        }
    }
#endif
}