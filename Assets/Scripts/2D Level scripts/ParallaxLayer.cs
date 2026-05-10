using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parallax")]
    [Tooltip("0 = слой стоит на месте, 1 = движется как камера")]
    [SerializeField] private float parallaxFactorX = 0.2f;

    [Tooltip("Обычно для сайдскроллера оставляем 0")]
    [SerializeField] private float parallaxFactorY = 0f;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        transform.position = new Vector3(
            startPosition.x + cameraDelta.x * parallaxFactorX,
            startPosition.y + cameraDelta.y * parallaxFactorY,
            startPosition.z
        );
    }
}