using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Настройки сцен")]
    [Tooltip("Название геймплейной сцены в Build Settings")]
    [SerializeField] private string gameplaySceneName = "Level1_2D"; // Обновил под твою текущую цель

    [Header("UI Панели")]
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        // При старте убеждаемся, что настройки скрыты
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // Метод для кнопки Start
    public void StartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Метод для кнопки Settings (открыть/закрыть)
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

    // Метод для кнопки Exit
    public void ExitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}