using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

/// <summary>
/// Quản lý việc chuyển đổi scene trong game True Sight.
/// Hỗ trợ fade transition và các tiện ích như restart, load theo index/name.
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Transition Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    #region Public Methods

    /// <summary>
    /// Load scene theo tên với fade transition.
    /// </summary>
    public async void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            await LoadSceneCoroutine(sceneName);
        }
    }

    /// <summary>
    /// Load scene theo build index với fade transition.
    /// </summary>
    public async void LoadScene(int sceneIndex)
    {
        if (!isTransitioning && sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            await LoadSceneCoroutine(sceneIndex);
        }
    }

    /// <summary>
    /// Reload scene hiện tại (khi player chết).
    /// </summary>
    public void RestartCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Load scene tiếp theo trong build order.
    /// </summary>
    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("SceneLoader: Đã ở scene cuối cùng!");
        }
    }

    /// <summary>
    /// Load scene trước đó trong build order.
    /// </summary>
    public void LoadPreviousScene()
    {
        int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (prevIndex >= 0)
        {
            LoadScene(prevIndex);
        }
        else
        {
            Debug.LogWarning("SceneLoader: Đã ở scene đầu tiên!");
        }
    }

    /// <summary>
    /// Load Main Menu (scene index 0).
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene(0);
    }
    
    /// <summary>
    /// Lấy tên scene hiện tại.
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Lấy index của scene hiện tại.
    /// </summary>
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    #endregion

    #region Private Methods

    async Task LoadSceneCoroutine(string sceneName)
    {
        isTransitioning = true;

        // Fade out
        await Fade(1f);

        // Load scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }

        // Fade in
        await Fade(0f);

        isTransitioning = false;
    }

    async Task LoadSceneCoroutine(int sceneIndex)
    {
        isTransitioning = true;

        // Fade out
        await Fade(1f);

        // Load scene
        var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }

        // Fade in
        await Fade(0f);

        isTransitioning = false;
    }

    async Task Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            return;
        }

        var startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            await Task.Yield();
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    #endregion
}
