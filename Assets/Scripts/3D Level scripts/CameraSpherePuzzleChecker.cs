using UnityEngine;

public class CameraSpherePuzzleChecker : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private Transform constellationCenter;
    [SerializeField] private Transform mainCamera;

    [Header("UI")]
    [SerializeField] private StarProgressUI starProgressUI;

    [Header("Manager")]
    [SerializeField] private Level3ConstellationManager levelManager;

    private ConstellationLevelData currentLevel;

    private float holdTimer;
    private bool canCheck;

    private void Update()
    {
        if (!canCheck || currentLevel == null)
            return;

        float angle = GetCurrentAngle();
        float progress01 = GetProgress01(angle);

        if (starProgressUI != null)
            starProgressUI.SetProgress01(progress01);

        if (angle <= currentLevel.correctZoneAngle)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= currentLevel.holdTimeToSolve)
            {
                SolveCurrentLevel();
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    public void StartChecking(ConstellationLevelData levelData)
    {
        currentLevel = levelData;
        holdTimer = 0f;
        canCheck = true;

        if (starProgressUI != null)
            starProgressUI.ResetProgress();
    }

    public void StopChecking()
    {
        canCheck = false;
        holdTimer = 0f;
    }

    private float GetCurrentAngle()
    {
        if (currentLevel == null || currentLevel.correctViewPoint == null)
            return 180f;

        Vector3 currentDirection =
            (mainCamera.position - constellationCenter.position).normalized;

        Vector3 correctDirection =
            (currentLevel.correctViewPoint.position - constellationCenter.position).normalized;

        return Vector3.Angle(currentDirection, correctDirection);
    }

    private float GetProgress01(float angle)
    {
        float emptyAngle = Mathf.Max(1f, currentLevel.angleForEmptyStar);

        float progress = 1f - angle / emptyAngle;
        return Mathf.Clamp01(progress);
    }

    private void SolveCurrentLevel()
    {
        canCheck = false;
        holdTimer = 0f;

        if (starProgressUI != null)
            starProgressUI.SetProgress01(1f);

        Debug.Log($"Созвездие решено: {currentLevel.constellationName}");

        if (levelManager != null)
            levelManager.OnCurrentLevelSolved();
    }
}