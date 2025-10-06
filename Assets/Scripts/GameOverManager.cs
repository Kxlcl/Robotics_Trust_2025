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


#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void openSurveySameTab();
#endif

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        
        // Wait a few seconds before redirecting to survey to let player read the game over screen
        StartCoroutine(DelayedSurveyRedirect());
    }
    
    private System.Collections.IEnumerator DelayedSurveyRedirect()
    {
        // Submit game data to database before redirecting
        if (DatabaseSubmitter.Instance != null)
        {
            DatabaseSubmitter.Instance.OnGameOver();
            Debug.Log("Game over data submitted to database");
        }

        // Wait 3 seconds to let player read the game over message
        yield return new UnityEngine.WaitForSeconds(3f);

        Debug.Log("Redirecting to survey after game over");

#if UNITY_WEBGL && !UNITY_EDITOR
        openSurveySameTab();
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
    
    // Method to manually proceed to survey (can be called by a button)
    public void ProceedToSurvey()
    {
        Debug.Log("Player manually proceeding to survey");
        
#if UNITY_WEBGL && !UNITY_EDITOR
        openSurveySameTab();
#endif
    }
}
