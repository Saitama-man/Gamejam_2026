using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI & Panels")]
    public GameObject settingsPanel;
    public Slider volumeSlider; // Ползунок музыки
    public Slider sfxSlider;    // НОВОЕ: Ползунок звуков (SFX)

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public AudioSource sfxSource;
    public AudioClip paperClickClip;
    public AudioClip hoverClip;

    private const string MIXER_MUSIC = "MusicVol";
    private const string PREF_MUSIC = "MusicVolume";

    // НОВЫЕ КОНСТАНТЫ ДЛЯ SFX
    private const string MIXER_SFX = "SFXVol";
    private const string PREF_SFX = "SfxVolume";

    void Start()
    {
        // Настройка ползунка музыки
        float savedMusicVol = PlayerPrefs.GetFloat(PREF_MUSIC, 0.75f);
        if (volumeSlider != null) volumeSlider.value = savedMusicVol;
        SetVolume(savedMusicVol);

        // Настройка ползунка SFX
        float savedSfxVol = PlayerPrefs.GetFloat(PREF_SFX, 0.75f);
        if (sfxSlider != null) sfxSlider.value = savedSfxVol;
        SetSfxVolume(savedSfxVol);
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        if (mainMixer != null) mainMixer.SetFloat(MIXER_MUSIC, dB);
        PlayerPrefs.SetFloat(PREF_MUSIC, sliderValue);
    }

    // НОВЫЙ МЕТОД ДЛЯ SFX
    public void SetSfxVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        if (mainMixer != null) mainMixer.SetFloat(MIXER_SFX, dB);
        PlayerPrefs.SetFloat(PREF_SFX, sliderValue);
    }

    public void PlayClickSound()
    {
        if (sfxSource != null && paperClickClip != null)
        {
            sfxSource.PlayOneShot(paperClickClip);
        }
    }

    public void PlayHoverSound()
    {
        if (sfxSource != null && hoverClip != null)
        {
            sfxSource.PlayOneShot(hoverClip);
        }
    }

    public void ToggleSettings(bool isOpen)
    {
        PlayClickSound();
        if (settingsPanel != null) settingsPanel.SetActive(isOpen);
    }

    public void LoadScene(string sceneName)
    {
        PlayClickSound();
        SceneFadeTransition fadeTransition = FindFirstObjectByType<SceneFadeTransition>();

        if (fadeTransition != null) fadeTransition.FadeToScene(sceneName);
        else SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}