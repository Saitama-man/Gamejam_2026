using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

	[Header("Quit Game On Complete")]
	[SerializeField] private bool quitGameOnComplete = false;

	[Header("Quit Fade")]
	[SerializeField] private bool useFadeBeforeQuit = true;
	[SerializeField] private CanvasGroup quitFadeCanvasGroup;
	[SerializeField] private float quitFadeDuration = 0.6f;

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

		if (quitGameOnComplete)
		{
			StartCoroutine(QuitGameRoutine());
			return;
		}

		if (loadSceneOnComplete)
		{
			LoadNextScene();
		}
	}

	private IEnumerator QuitGameRoutine()
	{
		if (useFadeBeforeQuit)
		{
			if (quitFadeCanvasGroup == null)
			{
				GameObject fadePanelObject = GameObject.Find("FadePanel");

				if (fadePanelObject != null)
				{
					quitFadeCanvasGroup = fadePanelObject.GetComponent<CanvasGroup>();
				}
			}

			if (quitFadeCanvasGroup != null)
			{
				quitFadeCanvasGroup.gameObject.SetActive(true);
				quitFadeCanvasGroup.alpha = 0f;

				float timer = 0f;

				while (timer < quitFadeDuration)
				{
					timer += Time.deltaTime;
					quitFadeCanvasGroup.alpha = Mathf.Clamp01(timer / quitFadeDuration);
					yield return null;
				}

				quitFadeCanvasGroup.alpha = 1f;
			}
			else
			{
				Debug.LogWarning("DialogueSequence: FadePanel CanvasGroup не найден. Игра завершится без затемнения.", this);
			}
		}

		QuitGame();
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

	private void QuitGame()
	{
		Debug.Log("Игра завершена после диалога.");

#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	public void ResetDialogue()
	{
		alreadyPlayed = false;
	}
}