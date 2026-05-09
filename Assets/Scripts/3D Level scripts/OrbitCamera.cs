using UnityEngine;

public class BlenderStyleOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Orbit")]
    [SerializeField] private float orbitSensitivity = 4f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    [Header("Zoom")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float zoomSensitivity = 2f;
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 25f;

    [Header("Input")]
    [SerializeField] private bool useMiddleMouseButton = true;

    private float yaw;
    private float pitch;
    private bool canControl = true;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Orbit Camera: target is not assigned.");
            enabled = false;
            return;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Vector3 euler = transform.rotation.eulerAngles;

        yaw = NormalizeAngle(euler.y);
        pitch = NormalizeAngle(euler.x);

        ApplyCameraPosition();
    }

    private void LateUpdate()
    {
        if (!canControl)
            return;

        HandleOrbitInput();
        HandleZoomInput();

        ApplyCameraPosition();
    }

    private void HandleOrbitInput()
    {
        bool orbitButtonPressed = useMiddleMouseButton
            ? Input.GetMouseButton(2)
            : Input.GetMouseButton(0);

        if (!orbitButtonPressed)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * orbitSensitivity;
        pitch -= mouseY * orbitSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleZoomInput()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        distance -= scroll * zoomSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void ApplyCameraPosition()
    {
        transform.position = target.position;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(0f, 0f, -distance);
            cameraTransform.localRotation = Quaternion.identity;
        }
    }

    public void SetCanControl(bool value)
    {
        canControl = value;
    }

    public void SetOrbitRotation(Vector2 newRotation)
    {
        yaw = newRotation.x;
        pitch = Mathf.Clamp(newRotation.y, minPitch, maxPitch);
        ApplyCameraPosition();
    }

    public Quaternion GetCurrentRotation()
    {
        return Quaternion.Euler(pitch, yaw, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}