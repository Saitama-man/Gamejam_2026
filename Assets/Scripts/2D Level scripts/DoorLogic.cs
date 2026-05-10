using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorLogic : MonoBehaviour
{
	[Header("Scene")]
	[SerializeField] private string sceneToLoad;
	[SerializeField] private string loadingSceneName = "LoadingScreen";

	[Header("Interaction")]
	[SerializeField] private bool requireButtonPress = false;
	[SerializeField] private KeyCode interactKey = KeyCode.E;

	[Header("Story Requirement")]
	[SerializeField] private bool requireStoryFlag = false;
	[SerializeField] private string requiredStoryFlagName;
	[SerializeField] private DialogueSequence blockedDialogue;

	private bool playerInside = false;
	private bool alreadyTriggered = false;

	private void Update()
	{
		if (requireButtonPress && playerInside && Input.GetKeyDown(interactKey))
		{
			TryLoadScene();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		playerInside = true;

		if (!requireButtonPress)
		{
			TryLoadScene();
		}
		else
		{
			Debug.Log("Нажми E, чтобы перейти.");
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (!other.CompareTag("Player"))
			return;

		playerInside = false;
	}

	private void TryLoadScene()
	{
		if (alreadyTriggered)
			return;

		if (requireStoryFlag && !StoryFlags.HasFlag(requiredStoryFlagName))
		{
			if (blockedDialogue != null)
			{
				blockedDialogue.Play();
			}
			else
			{
				Debug.Log("Дверь закрыта: не выполнено условие " + requiredStoryFlagName);
			}

			return;
		}

		alreadyTriggered = true;

		if (string.IsNullOrEmpty(sceneToLoad))
		{
			Debug.LogError("DoorLogic: Scene To Load не заполнена!", this);
			alreadyTriggered = false;
			return;
		}

		if (string.IsNullOrEmpty(loadingSceneName))
		{
			Debug.LogError("DoorLogic: Loading Scene Name не заполнена!", this);
			alreadyTriggered = false;
			return;
		}

		PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();

		if (playerMovement != null)
		{
			playerMovement.SetMovementEnabled(false);
		}

		SceneLoadData.TargetSceneName = sceneToLoad;

		SceneFadeTransition fadeTransition = FindFirstObjectByType<SceneFadeTransition>();

		if (fadeTransition != null)
		{
			fadeTransition.FadeToScene(loadingSceneName);
		}
		else
		{
			SceneManager.LoadScene(loadingSceneName);
		}
	}
}