using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Xử lý UI cho Main Scene (Main Menu) của game True Sight.
/// Quản lý các nút bấm và panel trong menu chính.
/// </summary>
public class MainScene : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Back Buttons")]
    [SerializeField] private Button settingsBackButton;

    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Scene Settings")]
    [SerializeField] private int firstLevel = 1;

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        LoadSettings();
    }

    private void InitializeUI()
    {
        ShowPanel(mainMenuPanel);
        HidePanel(settingsPanel);
    }

    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);

        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(OnBackToMainMenu);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    #region Button Handlers

    private void OnPlayButtonClicked()
    {
        SceneLoader.Instance?.LoadScene(firstLevel);
    }

    private void OnSettingsButtonClicked()
    {
        HidePanel(mainMenuPanel);
        ShowPanel(settingsPanel);
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("MainScene: Thoát game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnBackToMainMenu()
    {
        HidePanel(settingsPanel);
        ShowPanel(mainMenuPanel);
    }

    #endregion

    #region Settings Handlers

    private void LoadSettings()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Screen.fullScreen = isFullscreen;
    }

    #endregion

    #region Utility Methods

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    #endregion

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
        if (settingsBackButton != null) settingsBackButton.onClick.RemoveAllListeners();
    }
}
