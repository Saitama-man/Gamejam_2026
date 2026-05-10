using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwoImageCutscene : MonoBehaviour
{
	[Header("Images")]
	[SerializeField] private CanvasGroup leftImageGroup;
	[SerializeField] private CanvasGroup rightImageGroup;

	[Header("Timing")]
	[SerializeField] private float startDelay = 0.3f;
	[SerializeField] private float fadeInDuration = 0.5f;
	[SerializeField] private float holdDuration = 1.2f;
	[SerializeField] private float fadeOutDuration = 0.5f;
	[SerializeField] private float pauseBetweenImages = 0.25f;

	[Header("Story Flag On Complete")]
	[SerializeField] private bool setStoryFlagOnComplete = true;
	[SerializeField] private string storyFlagName = "ObservatoryCutscenePlayed";

	[Header("Next Scene")]
	[SerializeField] private string nextSceneName = "DarkObservatory";
	[SerializeField] private string loadingSceneName = "LoadingScreen";
	[SerializeField] private bool useLoadingScreen = true;

	private void Start()
	{
		if (leftImageGroup != null)
		{
			leftImageGroup.alpha = 0f;
		}

		if (rightImageGroup != null)
		{
			rightImageGroup.alpha = 0f;
		}

		StartCoroutine(PlayCutscene());
	}

	private IEnumerator PlayCutscene()
	{
		yield return new WaitForSeconds(startDelay);

		if (leftImageGroup != null)
		{
			yield return StartCoroutine(FadeCanvasGroup(leftImageGroup, 0f, 1f, fadeInDuration));
			yield return new WaitForSeconds(holdDuration);
			yield return StartCoroutine(FadeCanvasGroup(leftImageGroup, 1f, 0f, fadeOutDuration));
		}

		yield return new WaitForSeconds(pauseBetweenImages);

		if (rightImageGroup != null)
		{
			yield return StartCoroutine(FadeCanvasGroup(rightImageGroup, 0f, 1f, fadeInDuration));
			yield return new WaitForSeconds(holdDuration);
			yield return StartCoroutine(FadeCanvasGroup(rightImageGroup, 1f, 0f, fadeOutDuration));
		}

		FinishCutscene();
	}

	private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
	{
		float timer = 0f;
		group.alpha = from;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / duration);

			group.alpha = Mathf.Lerp(from, to, t);

			yield return null;
		}

		group.alpha = to;
	}

	private void FinishCutscene()
	{
		if (setStoryFlagOnComplete)
		{
			StoryFlags.SetFlag(storyFlagName);
		}

		if (string.IsNullOrEmpty(nextSceneName))
		{
			Debug.LogWarning("TwoImageCutscene: Next Scene Name не задан.", this);
			return;
		}

		if (useLoadingScreen)
		{
			SceneLoadData.TargetSceneName = nextSceneName;
			SceneManager.LoadScene(loadingSceneName);
		}
		else
		{
			SceneManager.LoadScene(nextSceneName);
		}
	}
}