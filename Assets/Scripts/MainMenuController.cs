using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Настройки сцен")]
    [Tooltip("Название геймплейной сцены в Build Settings")]
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    // Метод для кнопки Start
    public void StartGame()
    {
        // Загружаем сцену по имени
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Метод для кнопки Exit
    public void ExitGame()
    {
        Debug.Log("Выход из игры...");
        // Работает в билде, в редакторе просто выведет лог выше
        Application.Quit();
    }
}