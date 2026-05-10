using UnityEngine;
using UnityEngine.UI;

public class ConstellationPuzzleMVP : MonoBehaviour
{
    [Header("Camera Check")]
    [SerializeField] private Transform constellationCenter;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform correctViewPoint;

    [Header("Solve Settings")]
    [SerializeField] private float correctZoneAngle = 12f;
    [SerializeField] private float holdTimeToSolve = 1f;

    [Header("Camera Control")]
    [SerializeField] private MonoBehaviour orbitCameraScript;

    [Header("Constellation Lines")]
    [SerializeField] private Transform[] starPoints;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.04f;

    [Header("Zodiac Image")]
    [SerializeField] private Image zodiacImage;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private float holdTimer;
    private bool solved;

    private void Start()
    {
        if (zodiacImage != null)
            zodiacImage.gameObject.SetActive(false);
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
                Debug.Log($"В правильной зоне: {angle:0.0}°, удержание {holdTimer:0.0}/{holdTimeToSolve}");

            if (holdTimer >= holdTimeToSolve)
            {
                SolvePuzzle();
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private float GetCameraAngleToCorrectPoint()
    {
        if (constellationCenter == null || mainCamera == null || correctViewPoint == null)
        {
            Debug.LogError("Не назначены Constellation Center, Main Camera или Correct View Point.");
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
        solved = true;

        if (showDebugLogs)
            Debug.Log("Созвездие решено!");

        DisableCameraControl();
        DrawConstellationLines();
        ShowZodiacImage();
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

        orbitCameraScript.enabled = false;
    }

    private void DrawConstellationLines()
    {
        if (starPoints == null || starPoints.Length < 2)
        {
            Debug.LogWarning("Недостаточно точек для линий созвездия.");
            return;
        }

        GameObject lineObject = new GameObject("SolvedConstellationLines");
        lineObject.transform.SetParent(constellationCenter, worldPositionStays: true);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = starPoints.Length;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = lineWidth;

        if (lineMaterial != null)
            lineRenderer.material = lineMaterial;

        for (int i = 0; i < starPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, starPoints[i].position);
        }
    }

    private void ShowZodiacImage()
    {
        if (zodiacImage != null)
            zodiacImage.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (constellationCenter == null || correctViewPoint == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(constellationCenter.position, correctViewPoint.position);
        Gizmos.DrawWireSphere(correctViewPoint.position, 0.4f);
    }
#endif
}