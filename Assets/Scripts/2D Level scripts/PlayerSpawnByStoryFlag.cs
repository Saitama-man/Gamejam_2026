using UnityEngine;

public class PlayerSpawnByStoryFlag : MonoBehaviour
{
	[Header("Story Flag")]
	[SerializeField] private string storyFlagName = "ObservatoryCutscenePlayed";

	[Header("Spawn Point")]
	[SerializeField] private Transform spawnPointIfFlagExists;

	private void Start()
	{
		if (!StoryFlags.HasFlag(storyFlagName))
			return;

		if (spawnPointIfFlagExists == null)
		{
			Debug.LogWarning("PlayerSpawnByStoryFlag: Spawn Point не назначен.", this);
			return;
		}

		transform.position = spawnPointIfFlagExists.position;

		Rigidbody2D rb = GetComponent<Rigidbody2D>();

		if (rb != null)
		{
			rb.linearVelocity = Vector2.zero;
		}
	}
}