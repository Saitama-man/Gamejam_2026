using System;
using System.Collections;
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

    [Header("Constellation Lines")]
    [SerializeField] private Transform[] starPoints;
    [SerializeField] private StarLine[] starLines;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.04f;

    [Header("Zodiac Image Fade")]
    [SerializeField] private Image zodiacImage;
    [SerializeField] private CanvasGroup zodiacCanvasGroup;
    [SerializeField] private float zodiacFadeDuration = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool drawDebugGizmos = true;

    private float holdTimer;
    private bool solved;
    private Transform linesRoot;

    private string currentConstellationName;
    private Sprite currentConstellationSprite;

    private void Start()
    {
        PrepareZodiacImage();
        RemoveOldLines();
    }

    private void Update()
    {
        if (solved)
            return;

        float angle = GetCameraAngleToCorrectPoint();

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
        currentConstellationName = constellationName;
        currentConstellationSprite = newZodiacSprite;

        starPoints = newStarPoints;
        starLines = newStarLines;
        correctViewPoint = newCorrectViewPoint;

        solved = false;
        holdTimer = 0f;

        RemoveOldLines();
        PrepareZodiacImage();

        if (zodiacImage != null)
            zodiacImage.sprite = newZodiacSprite;

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

        DisableCameraControl();
        DrawConstellationLines();
        StartCoroutine(FadeInZodiacImage());

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

    private void PrepareZodiacImage()
    {
        if (zodiacImage == null)
            return;

        zodiacImage.gameObject.SetActive(true);

        if (zodiacCanvasGroup == null)
            zodiacCanvasGroup = zodiacImage.GetComponent<CanvasGroup>();

        if (zodiacCanvasGroup == null)
            zodiacCanvasGroup = zodiacImage.gameObject.AddComponent<CanvasGroup>();

        zodiacCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeInZodiacImage()
    {
        if (zodiacImage == null)
            yield break;

        zodiacImage.gameObject.SetActive(true);

        if (zodiacCanvasGroup == null)
            zodiacCanvasGroup = zodiacImage.GetComponent<CanvasGroup>();

        if (zodiacCanvasGroup == null)
            zodiacCanvasGroup = zodiacImage.gameObject.AddComponent<CanvasGroup>();

        zodiacCanvasGroup.alpha = 0f;

        if (zodiacFadeDuration <= 0f)
        {
            zodiacCanvasGroup.alpha = 1f;
            yield break;
        }

        float timer = 0f;

        while (timer < zodiacFadeDuration)
        {
            timer += Time.deltaTime;
            zodiacCanvasGroup.alpha = Mathf.Clamp01(timer / zodiacFadeDuration);
            yield return null;
        }

        zodiacCanvasGroup.alpha = 1f;
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