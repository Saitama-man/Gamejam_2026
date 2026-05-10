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

	[Header("X Bounds")]
	[SerializeField] private bool useXBounds = true;
	[SerializeField] private float minX = -5f;
	[SerializeField] private float maxX = 5f;

	[Header("Y Bounds")]
	[SerializeField] private bool useYBounds = false;
	[SerializeField] private float minY = -2f;
	[SerializeField] private float maxY = 2f;

	[Header("Zoom")]
	[SerializeField] private Camera targetCamera;
	[SerializeField] private bool useCurrentCameraSizeAsDefault = true;
	[SerializeField] private float defaultOrthographicSize = 1.6f;
	[SerializeField] private float zoomSmoothTime = 1.2f;

	[Header("Vertical Framing")]
	[SerializeField] private float verticalOffsetSmoothTime = 1.2f;

	private float targetOrthographicSize;
	private float zoomVelocity;

	private float baseCameraY;
	private float defaultYOffset;
	private float targetYOffset;
	private float currentYOffset;
	private float yOffsetVelocity;

	private void Awake()
	{
		if (targetCamera == null)
		{
			targetCamera = GetComponent<Camera>();
		}

		if (targetCamera != null)
		{
			if (useCurrentCameraSizeAsDefault)
			{
				defaultOrthographicSize = targetCamera.orthographicSize;
			}

			targetOrthographicSize = defaultOrthographicSize;
		}

		baseCameraY = transform.position.y;

		defaultYOffset = offset.y;
		targetYOffset = defaultYOffset;
		currentYOffset = defaultYOffset;
	}

	private void LateUpdate()
	{
		UpdateZoom();
		UpdateVerticalOffset();
		FollowTarget();
	}

	private void FollowTarget()
	{
		if (target == null)
			return;

		Vector3 currentPosition = transform.position;
		Vector3 targetPosition = currentPosition;

		if (followX)
		{
			targetPosition.x = target.position.x + offset.x;
		}

		if (followY)
		{
			targetPosition.y = target.position.y + currentYOffset;
		}
		else
		{
			targetPosition.y = baseCameraY + currentYOffset;
		}

		targetPosition.z = offset.z;

		if (useXBounds)
		{
			targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
		}

		if (useYBounds)
		{
			targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
		}

		transform.position = Vector3.Lerp(
			currentPosition,
			targetPosition,
			smoothSpeed * Time.deltaTime
		);
	}

	private void UpdateZoom()
	{
		if (targetCamera == null)
			return;

		targetCamera.orthographicSize = Mathf.SmoothDamp(
			targetCamera.orthographicSize,
			targetOrthographicSize,
			ref zoomVelocity,
			zoomSmoothTime
		);
	}

	private void UpdateVerticalOffset()
	{
		currentYOffset = Mathf.SmoothDamp(
			currentYOffset,
			targetYOffset,
			ref yOffsetVelocity,
			verticalOffsetSmoothTime
		);
	}

	public float GetDefaultOrthographicSize()
	{
		return defaultOrthographicSize;
	}

	public void SetTargetZoom(float newOrthographicSize)
	{
		targetOrthographicSize = newOrthographicSize;
	}

	public void ResetZoom()
	{
		targetOrthographicSize = defaultOrthographicSize;
	}

	public void SetTargetYOffset(float newYOffset)
	{
		targetYOffset = newYOffset;
	}

	public void ResetYOffset()
	{
		targetYOffset = defaultYOffset;
	}
}