using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý UI trong gameplay: Pause, Settings, Lose/Game Over.
/// </summary>
public class GameplayUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Pause Panel Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings Panel")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button settingsOverlay;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Lose Panel Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button loseMainMenuButton;

    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;
    private bool isGameOver = false;

    private void Start()
    {
        SetupButtonListeners();
        LoadSettings();
        HideAllPanels();
    }

    private void Update()
    {
        // Chỉ cho phép pause khi chưa game over
        if (Input.GetKeyDown(pauseKey) && !isGameOver)
        {
            TogglePause();
        }
    }

    private void SetupButtonListeners()
    {
        // Pause Panel
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Settings Panel
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(CloseSettings);

        if (settingsOverlay != null)
            settingsOverlay.onClick.AddListener(CloseSettings);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Lose Panel
        if (retryButton != null)
            retryButton.onClick.AddListener(Restart);

        if (loseMainMenuButton != null)
            loseMainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void LoadSettings()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    #region Pause Methods

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        ShowPanel(pausePanel);
        HidePanel(settingsPanel);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        HideAllPanels();
    }

    #endregion

    #region Settings Methods

    private void OpenSettings()
    {
        HidePanel(pausePanel);
        ShowPanel(settingsPanel);
    }

    private void CloseSettings()
    {
        HidePanel(settingsPanel);
        ShowPanel(pausePanel);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    #endregion

    #region Game Over Methods

    /// <summary>
    /// Gọi method này khi player chết.
    /// </summary>
    public void ShowLoseScreen()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        HidePanel(pausePanel);
        HidePanel(settingsPanel);
        ShowPanel(losePanel);
    }

    #endregion

    #region Navigation Methods

    private void Restart()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
        SceneLoader.Instance?.RestartCurrentScene();
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
        SceneLoader.Instance?.LoadMainMenu();
    }

    #endregion

    #region Utility Methods

    private void HideAllPanels()
    {
        HidePanel(pausePanel);
        HidePanel(settingsPanel);
        HidePanel(losePanel);
    }

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
        // Cleanup
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (settingsBackButton != null) settingsBackButton.onClick.RemoveAllListeners();
        if (settingsOverlay != null) settingsOverlay.onClick.RemoveAllListeners();
        if (retryButton != null) retryButton.onClick.RemoveAllListeners();
        if (loseMainMenuButton != null) loseMainMenuButton.onClick.RemoveAllListeners();
    }
}
