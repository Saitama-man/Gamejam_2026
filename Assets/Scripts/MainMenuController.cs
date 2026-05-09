using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI ѕанели")]
    [SerializeField] private GameObject settingsPanel;

    [Header("«вук")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (musicSlider != null)
        {
            musicSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
    }

    // --- ”Ќ»¬≈–—јЋ№Ќјя Ћќ√» ј —÷≈Ќ ---

    // Ётот метод теперь можно вызывать из ЋёЅќ… кнопки, просто вписав им€ сцены в Unity
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("»м€ сцены не указано в кнопке!");
        }
    }

    // ћетод дл€ простой перезагрузки текущей сцены (полезно дл€ кнопки Restart)
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- ќ—“јЋ№Ќјя Ћќ√» ј ---

    public void ToggleSettings(bool isOpen) => settingsPanel.SetActive(isOpen);

    public void ExitGame()
    {
        Debug.Log("¬ыход из игры...");
        Application.Quit();
    }

    public void SetVolume(float sliderValue)
    {
        float volume = Mathf.Log10(sliderValue) * 20;
        if (sliderValue == 0) volume = -80;
        mainMixer.SetFloat("MusicVol", volume);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
}