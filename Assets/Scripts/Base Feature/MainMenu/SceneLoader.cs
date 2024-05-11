using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string name)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGame();
    }
}
