using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	[Header("Настройки сцен")]
	[Tooltip("Название первой игровой сцены")]
	[SerializeField] private string gameplaySceneName = "LevelRealNight";

	[Tooltip("Название сцены загрузки")]
	[SerializeField] private string loadingSceneName = "LoadingScreen";

	[Header("UI Панели")]
	[SerializeField] private GameObject settingsPanel;

	private void Start()
	{
		if (settingsPanel != null)
		{
			settingsPanel.SetActive(false);
		}
	}

	public void StartGame()
	{
		if (string.IsNullOrEmpty(gameplaySceneName))
		{
			Debug.LogError("MainMenuController: Gameplay Scene Name не заполнен!");
			return;
		}

		if (string.IsNullOrEmpty(loadingSceneName))
		{
			Debug.LogError("MainMenuController: Loading Scene Name не заполнен!");
			return;
		}

		SceneLoadData.TargetSceneName = gameplaySceneName;
		SceneManager.LoadScene(loadingSceneName);
	}

	public void ToggleSettings(bool isOpen)
	{
		if (settingsPanel != null)
		{
			settingsPanel.SetActive(isOpen);
		}
		else
		{
			Debug.LogWarning("Settings Panel не назначена в инспекторе!");
		}
	}

	public void ExitGame()
	{
		Debug.Log("Выход из игры...");
		Application.Quit();
	}
}