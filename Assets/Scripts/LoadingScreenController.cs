using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadingTime = 1f;

    [Header("Fallback")]
    [SerializeField] private string fallbackSceneName = "MainMenu";

    [Header("Visual")]
    [SerializeField] private CanvasGroup loadingLeafCanvasGroup;
    [SerializeField] private float leafFadeInTime = 0.25f;
    [SerializeField] private float leafFadeOutTime = 0.25f;

    private void Start()
    {
        if (loadingLeafCanvasGroup != null)
        {
            loadingLeafCanvasGroup.alpha = 0f;
        }

        string targetScene = SceneLoadData.TargetSceneName;

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("LoadingScreen: TargetSceneName не задан.");
            SceneManager.LoadScene(fallbackSceneName);
            return;
        }

        StartCoroutine(LoadTargetScene(targetScene));
    }

    private IEnumerator LoadTargetScene(string sceneName)
    {
        yield return StartCoroutine(FadeLeaf(0f, 1f, leafFadeInTime));

        float timer = 0f;

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress < 0.9f || timer < minimumLoadingTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(FadeLeaf(1f, 0f, leafFadeOutTime));

        loadingOperation.allowSceneActivation = true;
    }

    private IEnumerator FadeLeaf(float from, float to, float duration)
    {
        if (loadingLeafCanvasGroup == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            loadingLeafCanvasGroup.alpha = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }

        loadingLeafCanvasGroup.alpha = to;
    }
}