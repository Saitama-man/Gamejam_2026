using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove = true;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("На Player нет Rigidbody2D!");
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        FlipCharacter();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
    }
    private void Move()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck не назначен в PlayerMovement!");
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void FlipCharacter()
    {
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled && rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
}