using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    [DllImport("__Internal")]
    private static extern void showSurveyForm();

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
#if UNITY_WEBGL && !UNITY_EDITOR
        showSurveyForm();
#endif
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
