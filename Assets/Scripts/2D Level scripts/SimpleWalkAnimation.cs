using UnityEngine;

public class SimpleWalkAnimation : MonoBehaviour
{
	[Header("Sprite Renderer")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	[Header("Movement Source")]
	[SerializeField] private PlayerMovement playerMovement;

	[Header("Walk Frames - With Candle")]
	[SerializeField] private Sprite[] walkFramesWithCandle;

	[Header("Walk Frames - Without Candle")]
	[SerializeField] private Sprite[] walkFramesWithoutCandle;

	[Header("Settings")]
	[SerializeField] private float framesPerSecond = 8f;

	[Header("Starting Form")]
	[SerializeField] private bool startsWithCandle = true;

	[Header("Flip")]
	[SerializeField] private bool artFacesRight = true;

	private Sprite[] currentWalkFrames;

	private int currentFrame;
	private float timer;
	private Vector3 startScale;
	private bool hasCandle;

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

		SetHasCandle(startsWithCandle);
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

	public void SetHasCandle(bool value)
	{
		hasCandle = value;

		currentWalkFrames = hasCandle
			? walkFramesWithCandle
			: walkFramesWithoutCandle;

		currentFrame = 0;
		timer = 0f;

		SetIdleFrame();
	}

	public void GiveCandle()
	{
		SetHasCandle(true);
	}

	public void RemoveCandle()
	{
		SetHasCandle(false);
	}

	private void HandleWalkAnimation(float moveX)
	{
		if (currentWalkFrames == null || currentWalkFrames.Length == 0)
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

			if (currentFrame >= currentWalkFrames.Length)
			{
				currentFrame = 0;
			}

			spriteRenderer.sprite = currentWalkFrames[currentFrame];
		}
	}

	private void SetIdleFrame()
	{
		if (currentWalkFrames == null || currentWalkFrames.Length == 0)
			return;

		currentFrame = 0;
		timer = 0f;
		spriteRenderer.sprite = currentWalkFrames[0];
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