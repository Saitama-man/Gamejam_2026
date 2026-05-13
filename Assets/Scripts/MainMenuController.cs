using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "LevelRealNight";
    [SerializeField] private string loadingSceneName = "LoadingScreen";

    [Header("UI & Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip paperClickClip;
    [SerializeField] private AudioClip hoverClip;

    private const string MIXER_MUSIC = "MusicVol";
    private const string PREF_MUSIC = "MusicVolume";

    private const string MIXER_SFX = "SFXVol";
    private const string PREF_SFX = "SfxVolume";

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        float savedMusicVol = PlayerPrefs.GetFloat(PREF_MUSIC, 0.75f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedMusicVol;
        }

        SetVolume(savedMusicVol);

        float savedSfxVol = PlayerPrefs.GetFloat(PREF_SFX, 0.75f);

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfxVol;
        }

        SetSfxVolume(savedSfxVol);
    }

    public void StartGame()
    {
        PlayClickSound();

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
        PlayClickSound();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isOpen);
        }
        else
        {
            Debug.LogWarning("MainMenuController: Settings Panel не назначена в инспекторе!");
        }
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;

        if (mainMixer != null)
        {
            mainMixer.SetFloat(MIXER_MUSIC, dB);
        }

        PlayerPrefs.SetFloat(PREF_MUSIC, sliderValue);
    }

    public void SetSfxVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;

        if (mainMixer != null)
        {
            mainMixer.SetFloat(MIXER_SFX, dB);
        }

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

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}