using UnityEngine;

public class LoadingRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 60f;

    private void Update()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}