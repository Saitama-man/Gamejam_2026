using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneImagePan : MonoBehaviour
{
	[Header("Image")]
	[SerializeField] private RectTransform imageTransform;

	[Header("Pan Settings")]
	[SerializeField] private Vector2 startPosition = new Vector2(400f, 0f);
	[SerializeField] private Vector2 endPosition = new Vector2(-400f, 0f);
	[SerializeField] private float panDuration = 3f;

	[Header("Fade Overlay")]
	[SerializeField] private CanvasGroup fadeOverlayCanvasGroup;
	[SerializeField] private float startFadeDuration = 0.4f;
	[SerializeField] private float endFadeDuration = 0.4f;

	[Header("Next Scene")]
	[SerializeField] private string nextSceneName = "LevelRealNight";
	[SerializeField] private string loadingSceneName = "LoadingScreen";
	[SerializeField] private bool useLoadingScreen = true;

	private void Start()
	{
		if (imageTransform == null)
		{
			Debug.LogError("CutsceneImagePan: Image Transform не назначен.", this);
			return;
		}

		if (fadeOverlayCanvasGroup == null)
		{
			Debug.LogError("CutsceneImagePan: Fade Overlay Canvas Group не назначен.", this);
			return;
		}

		StartCoroutine(PlayCutscene());
	}

	private IEnumerator PlayCutscene()
	{
		imageTransform.anchoredPosition = startPosition;

		// Стартуем с чёрного экрана и плавно открываем катсцену
		fadeOverlayCanvasGroup.alpha = 1f;
		yield return StartCoroutine(FadeOverlay(1f, 0f, startFadeDuration));

		// Двигаем картинку
		yield return StartCoroutine(PanImage());

		// В конце закрываем всё чёрным экраном
		yield return StartCoroutine(FadeOverlay(0f, 1f, endFadeDuration));

		// Только после полного затемнения грузим следующую сцену
		FinishCutscene();
	}

	private IEnumerator PanImage()
	{
		float timer = 0f;

		while (timer < panDuration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / panDuration);

			imageTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);

			yield return null;
		}

		imageTransform.anchoredPosition = endPosition;
	}

	private IEnumerator FadeOverlay(float from, float to, float duration)
	{
		float timer = 0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp01(timer / duration);

			fadeOverlayCanvasGroup.alpha = Mathf.Lerp(from, to, t);

			yield return null;
		}

		fadeOverlayCanvasGroup.alpha = to;
	}

	private void FinishCutscene()
	{
		if (string.IsNullOrEmpty(nextSceneName))
		{
			Debug.LogWarning("CutsceneImagePan: Next Scene Name не задан.", this);
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