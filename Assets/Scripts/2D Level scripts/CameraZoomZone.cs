using UnityEngine;

public class CameraZoomZone2D : MonoBehaviour
{
	[Header("Camera")]
	[SerializeField] private CameraFollow2D cameraFollow;

	[Header("Zoom")]
	[SerializeField] private float zoomedOutSize = 1.55f;

	[Header("Bottom Edge Lock")]
	[SerializeField] private bool keepBottomEdge = true;

	[Header("Manual Vertical Offset")]
	[SerializeField] private bool useManualYOffset = false;
	[SerializeField] private float manualYOffset = 0.1f;

	private void Awake()
	{
		if (cameraFollow == null)
		{
			cameraFollow = FindFirstObjectByType<CameraFollow2D>();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		if (cameraFollow == null)
			return;

		cameraFollow.SetTargetZoom(zoomedOutSize);

		if (keepBottomEdge)
		{
			float defaultSize = cameraFollow.GetDefaultOrthographicSize();
			float yCompensation = zoomedOutSize - defaultSize;

			cameraFollow.SetTargetYOffset(yCompensation);
		}
		else if (useManualYOffset)
		{
			cameraFollow.SetTargetYOffset(manualYOffset);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		if (cameraFollow == null)
			return;

		cameraFollow.ResetZoom();
		cameraFollow.ResetYOffset();
	}
}