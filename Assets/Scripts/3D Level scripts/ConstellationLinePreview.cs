using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConstellationLinePreview : MonoBehaviour
{
    [Header("Preview Lines")]
    [SerializeField] private ConstellationPuzzleMVP.StarLine[] starLines;

    [Header("Star Detection")]
    [SerializeField] private bool autoCollectStars = true;
    [SerializeField] private Transform[] starPoints;

    [Header("Scene View Display")]
    [SerializeField] private bool drawPreview = true;
    [SerializeField] private bool drawLabels = true;
    [SerializeField] private bool drawStarSpheres = true;
    [SerializeField] private float labelSize = 18f;
    [SerializeField] private float starSphereSize = 0.12f;

    [Header("Colors")]
    [SerializeField] private Color lineColor = Color.cyan;
    [SerializeField] private Color invalidLineColor = Color.red;
    [SerializeField] private Color starColor = Color.yellow;
    [SerializeField] private Color labelColor = Color.white;

    public ConstellationPuzzleMVP.StarLine[] StarLines => starLines;
    public Transform[] StarPoints => starPoints;

    private void OnValidate()
    {
        if (autoCollectStars)
            CollectStars();
    }

    [ContextMenu("Collect Stars")]
    public void CollectStars()
    {
        starPoints = CollectStarsFromChildren(transform);
    }

    [ContextMenu("Log Star Order")]
    public void LogStarOrder()
    {
        if (autoCollectStars || starPoints == null || starPoints.Length == 0)
            CollectStars();

        if (starPoints == null || starPoints.Length == 0)
        {
            Debug.LogWarning($"[{name}] «вЄзды не найдены.");
            return;
        }

        string log = $"[{name}] Star order:\n";

        for (int i = 0; i < starPoints.Length; i++)
        {
            string path = GetTransformPath(starPoints[i]);
            log += $"{i}: {starPoints[i].name} | {path}\n";
        }

        Debug.Log(log);
    }

    private Transform[] CollectStarsFromChildren(Transform root)
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

    private string GetTransformPath(Transform target)
    {
        string path = target.name;
        Transform current = target.parent;

        while (current != null && current != transform)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawPreview)
            return;

        if (autoCollectStars || starPoints == null || starPoints.Length == 0)
            starPoints = CollectStarsFromChildren(transform);

        DrawStars();
        DrawLines();
        DrawLabels();
    }

    private void DrawStars()
    {
        if (!drawStarSpheres || starPoints == null)
            return;

        Gizmos.color = starColor;

        for (int i = 0; i < starPoints.Length; i++)
        {
            if (starPoints[i] == null)
                continue;

            Gizmos.DrawWireSphere(starPoints[i].position, starSphereSize);
        }
    }

    private void DrawLines()
    {
        if (starLines == null || starPoints == null)
            return;

        for (int i = 0; i < starLines.Length; i++)
        {
            int from = starLines[i].from;
            int to = starLines[i].to;

            bool valid =
                from >= 0 &&
                to >= 0 &&
                from < starPoints.Length &&
                to < starPoints.Length &&
                starPoints[from] != null &&
                starPoints[to] != null;

            Gizmos.color = valid ? lineColor : invalidLineColor;

            if (valid)
            {
                Gizmos.DrawLine(starPoints[from].position, starPoints[to].position);
            }
            else
            {
                Vector3 warningPosition = transform.position + Vector3.up * (0.5f + i * 0.15f);
                Gizmos.DrawWireSphere(warningPosition, 0.08f);
            }
        }
    }

    private void DrawLabels()
    {
        if (!drawLabels || starPoints == null)
            return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = labelColor;
        style.fontSize = Mathf.RoundToInt(labelSize);
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < starPoints.Length; i++)
        {
            if (starPoints[i] == null)
                continue;

            Vector3 labelPosition = starPoints[i].position + Vector3.up * 0.18f;
            Handles.Label(labelPosition, i.ToString(), style);
        }
    }
#endif
}