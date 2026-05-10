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
	[SerializeField] private float firstFadeInDuration = 0.5f;
	[SerializeField] private float delayBeforeSecondImage = 0.7f;
	[SerializeField] private float secondFadeInDuration = 0.5f;
	[SerializeField] private float holdAfterBothImages = 1.2f;

	[Header("End Fade")]
	[SerializeField] private bool fadeOutBothImagesAtEnd = true;
	[SerializeField] private float endFadeOutDuration = 0.5f;

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

		// Первая картинка появляется и остаётся
		if (leftImageGroup != null)
		{
			yield return StartCoroutine(FadeCanvasGroup(leftImageGroup, 0f, 1f, firstFadeInDuration));
		}

		yield return new WaitForSeconds(delayBeforeSecondImage);

		// Вторая картинка появляется, первая всё ещё видна
		if (rightImageGroup != null)
		{
			yield return StartCoroutine(FadeCanvasGroup(rightImageGroup, 0f, 1f, secondFadeInDuration));
		}

		yield return new WaitForSeconds(holdAfterBothImages);

		// По желанию обе картинки вместе затухают перед переходом
		if (fadeOutBothImagesAtEnd)
		{
			yield return StartCoroutine(FadeBothImagesOut());
		}

		FinishCutscene();
	}

	private IEnumerator FadeBothImagesOut()
	{
		float timer = 0f;

		float leftStartAlpha = leftImageGroup != null ? leftImageGroup.alpha : 0f;
		float rightStartAlpha = rightImageGroup != null ? rightImageGroup.alpha : 0f;

		while (timer < endFadeOutDuration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / endFadeOutDuration);

			if (leftImageGroup != null)
			{
				leftImageGroup.alpha = Mathf.Lerp(leftStartAlpha, 0f, t);
			}

			if (rightImageGroup != null)
			{
				rightImageGroup.alpha = Mathf.Lerp(rightStartAlpha, 0f, t);
			}

			yield return null;
		}

		if (leftImageGroup != null)
		{
			leftImageGroup.alpha = 0f;
		}

		if (rightImageGroup != null)
		{
			rightImageGroup.alpha = 0f;
		}
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