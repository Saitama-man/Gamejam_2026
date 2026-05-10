using UnityEngine;

public class SimpleWalkAnimation : MonoBehaviour
{
	[Header("Sprite Renderer")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	[Header("Movement Source")]
	[SerializeField] private PlayerMovement playerMovement;

	[Header("Walk Frames")]
	[SerializeField] private Sprite[] walkFrames;

	[Header("Settings")]
	[SerializeField] private float framesPerSecond = 8f;

	[Header("Flip")]
	[SerializeField] private bool artFacesRight = true;

	private int currentFrame;
	private float timer;
	private Vector3 startScale;

	private void Awake()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		if (playerMovement == null)
		{
			playerMovement = GetComponent<PlayerMovement>();

			if (playerMovement == null)
			{
				playerMovement = GetComponentInParent<PlayerMovement>();
			}
		}

		startScale = transform.localScale;
	}

	private void Update()
	{
		if (spriteRenderer == null)
			return;

		if (playerMovement != null && !playerMovement.canMove)
		{
			SetIdleFrame();
			return;
		}

		float moveX = Input.GetAxisRaw("Horizontal");

		HandleFlip(moveX);
		HandleWalkAnimation(moveX);
	}

	private void HandleWalkAnimation(float moveX)
	{
		if (walkFrames == null || walkFrames.Length == 0)
			return;

		bool isMoving = Mathf.Abs(moveX) > 0.01f;

		if (!isMoving)
		{
			SetIdleFrame();
			return;
		}

		timer += Time.deltaTime;

		if (timer >= 1f / framesPerSecond)
		{
			timer = 0f;

			currentFrame++;

			if (currentFrame >= walkFrames.Length)
			{
				currentFrame = 0;
			}

			spriteRenderer.sprite = walkFrames[currentFrame];
		}
	}

	private void SetIdleFrame()
	{
		if (walkFrames == null || walkFrames.Length == 0)
			return;

		currentFrame = 0;
		timer = 0f;
		spriteRenderer.sprite = walkFrames[0];
	}

	private void HandleFlip(float moveX)
	{
		if (Mathf.Abs(moveX) < 0.01f)
			return;

		float direction = moveX > 0 ? 1f : -1f;

		if (!artFacesRight)
		{
			direction *= -1f;
		}

		transform.localScale = new Vector3(
			Mathf.Abs(startScale.x) * direction,
			startScale.y,
			startScale.z
		);
	}
}