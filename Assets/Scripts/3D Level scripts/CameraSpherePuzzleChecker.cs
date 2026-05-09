using UnityEngine;
using TMPro;

public class CameraSpherePuzzleChecker : MonoBehaviour
{
    [Header("Sphere Check")]
    [SerializeField] private Transform constellationCenter;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform correctViewPoint;

    [Header("Control")]
    [SerializeField] private BlenderStyleOrbitCamera orbitCamera;
    [SerializeField] private ConstellationLevelManager levelManager;

    [Header("Progress")]
    [SerializeField] private float requiredPercent = 80f;
    [SerializeField] private float angleForZeroProgress = 90f;
    [SerializeField] private float correctZoneAngle = 15f;

    [Header("Hold")]
    [SerializeField] private float holdTimeToSolve = 1.2f;

    [Header("UI")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text hintText;

    private float holdTimer;
    private bool canCheck;

    private void Update()
    {
        if (!canCheck)
            return;

        float progress = GetProgressPercent();
        float angle = GetCurrentAngle();

        if (progress >= requiredPercent && angle <= correctZoneAngle)
        {
            holdTimer += Time.deltaTime;

            if (hintText != null)
                hintText.text = $"Удерживай положение: {holdTimer:0.0}/{holdTimeToSolve:0.0}";

            if (holdTimer >= holdTimeToSolve)
            {
                SolveCurrentConstellation();
            }
        }
        else
        {
            holdTimer = 0f;

            if (hintText != null)
                hintText.text = "Вращай телескоп, чтобы совместить созвездие";
        }

        if (progressText != null)
            progressText.text = $"Совпадение: {progress:0}%";
    }

    public void StartChecking(Transform newCorrectViewPoint, float newCorrectZoneAngle)
    {
        correctViewPoint = newCorrectViewPoint;
        correctZoneAngle = newCorrectZoneAngle;

        holdTimer = 0f;
        canCheck = true;

        if (orbitCamera != null)
            orbitCamera.SetCanControl(true);
    }

    public void StopChecking()
    {
        canCheck = false;

        if (orbitCamera != null)
            orbitCamera.SetCanControl(false);
    }

    private void SolveCurrentConstellation()
    {
        StopChecking();

        if (levelManager != null)
            levelManager.OnCurrentConstellationSolved();
    }

    private float GetCurrentAngle()
    {
        Vector3 currentDirection = (mainCamera.position - constellationCenter.position).normalized;
        Vector3 correctDirection = (correctViewPoint.position - constellationCenter.position).normalized;

        return Vector3.Angle(currentDirection, correctDirection);
    }

    public float GetProgressPercent()
    {
        float angle = GetCurrentAngle();

        float percent = 100f - angle / angleForZeroProgress * 100f;
        return Mathf.Clamp(percent, 0f, 100f);
    }
}