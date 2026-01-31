using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public string nextSceneName;

    public void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
