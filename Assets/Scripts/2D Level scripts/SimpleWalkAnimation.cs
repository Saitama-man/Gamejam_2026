using UnityEngine;

[DefaultExecutionOrder(30000)]
[RequireComponent(typeof(SpriteRenderer))]
public class SimpleWalkAnimation : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private Sprite[] walkFrames;

    [Header("Animation")]
    [SerializeField] private float framesPerSecond = 8f;

    [Header("Flip")]
    [SerializeField] private bool invertFlip = false;
    [SerializeField] private float moveThreshold = 0.05f;

    [Header("References")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Debug")]
    [SerializeField] private float debugHorizontal;
    [SerializeField] private bool debugFacingLeft;

    private SpriteRenderer spriteRenderer;

    private float timer;
    private int currentFrame;

    private bool facingLeft;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerRigidbody == null)
            playerRigidbody = GetComponentInParent<Rigidbody2D>();

        if (walkFrames != null && walkFrames.Length > 0)
            spriteRenderer.sprite = walkFrames[0];
    }

    private void Update()
    {
        float horizontal = GetHorizontalDirection();

        debugHorizontal = horizontal;

        if (horizontal < -moveThreshold)
            facingLeft = true;
        else if (horizontal > moveThreshold)
            facingLeft = false;

        debugFacingLeft = facingLeft;

        ApplyFlip();
        Animate(Mathf.Abs(horizontal) > moveThreshold);
    }

    private float GetHorizontalDirection()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        if (Mathf.Abs(horizontal) > 0.01f)
            return horizontal;

        if (playerRigidbody != null)
            return playerRigidbody.linearVelocity.x;

        return 0f;
    }

    private void ApplyFlip()
    {
        bool finalFlip = facingLeft;

        if (invertFlip)
            finalFlip = !finalFlip;

        spriteRenderer.flipX = finalFlip;
    }

    private void Animate(bool isMoving)
    {
        if (walkFrames == null || walkFrames.Length == 0)
            return;

        if (!isMoving)
        {
            timer = 0f;
            currentFrame = 0;
            spriteRenderer.sprite = walkFrames[0];
            return;
        }

        timer += Time.deltaTime;

        if (timer >= 1f / framesPerSecond)
        {
            timer = 0f;

            currentFrame++;

            if (currentFrame >= walkFrames.Length)
                currentFrame = 0;

            spriteRenderer.sprite = walkFrames[currentFrame];
        }
    }
}