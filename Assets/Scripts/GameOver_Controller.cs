using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    // Этот метод повесим на кнопку "Restart"
    public void RestartLevel()
    {
        // Читаем из памяти название уровня, на котором проиграл игрок. 
        // Если ничего не записано, по умолчанию грузим Level1_2D
        string lastLevel = PlayerPrefs.GetString("LastLevel", "Level1_2D");
        LoadSceneSmoothly(lastLevel);
    }

    // Этот метод повесим на кнопку "Main Menu"
    public void GoToMainMenu()
    {
        LoadSceneSmoothly("MainMenu"); // Убедись, что твоя сцена меню называется именно так
    }

    // Вспомогательный метод для красивого перехода
    private void LoadSceneSmoothly(string sceneName)
    {
        SceneFadeTransition fadeTransition = FindFirstObjectByType<SceneFadeTransition>();

        if (fadeTransition != null)
        {
            fadeTransition.FadeToScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}