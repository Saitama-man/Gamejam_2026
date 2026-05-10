using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Follow Axis")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition;

        if (followX)
            targetPosition.x = target.position.x + offset.x;

        if (followY)
            targetPosition.y = target.position.y + offset.y;

        targetPosition.z = offset.z;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(
            currentPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}