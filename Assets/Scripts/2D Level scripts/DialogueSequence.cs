using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSequence : MonoBehaviour
{
	[Header("Dialogue Lines")]
	[SerializeField] private DialogueLine[] dialogueLines;

	[Header("Settings")]
	[SerializeField] private bool playOnlyOnce = true;

	[Header("Story Flag On Complete")]
	[SerializeField] private bool setStoryFlagOnComplete = false;
	[SerializeField] private string storyFlagName;

	[Header("Scene Load On Complete")]
	[SerializeField] private bool loadSceneOnComplete = false;
	[SerializeField] private string sceneToLoadOnComplete;
	[SerializeField] private string loadingSceneName = "LoadingScreen";
	[SerializeField] private bool useLoadingScreen = true;

	private bool alreadyPlayed = false;

	public bool CanPlay()
	{
		if (dialogueLines == null || dialogueLines.Length == 0)
			return false;

		if (playOnlyOnce && alreadyPlayed)
			return false;

		return true;
	}

	public void Play()
	{
		if (!CanPlay())
			return;

		if (DialogueManager.Instance == null)
		{
			Debug.LogError("DialogueSequence: в сцене нет DialogueManager.", this);
			return;
		}

		alreadyPlayed = true;

		DialogueManager.Instance.StartDialogue(dialogueLines, OnDialogueFinished);
	}

	private void OnDialogueFinished()
	{
		if (setStoryFlagOnComplete)
		{
			StoryFlags.SetFlag(storyFlagName);
		}

		if (loadSceneOnComplete)
		{
			LoadNextScene();
		}
	}

	private void LoadNextScene()
	{
		if (string.IsNullOrEmpty(sceneToLoadOnComplete))
		{
			Debug.LogWarning("DialogueSequence: Scene To Load On Complete не задан.", this);
			return;
		}

		if (useLoadingScreen)
		{
			SceneLoadData.TargetSceneName = sceneToLoadOnComplete;
			SceneManager.LoadScene(loadingSceneName);
		}
		else
		{
			SceneManager.LoadScene(sceneToLoadOnComplete);
		}
	}

	public void ResetDialogue()
	{
		alreadyPlayed = false;
	}
}