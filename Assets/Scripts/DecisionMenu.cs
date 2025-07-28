using UnityEngine;
using UnityEngine.SceneManagement;

public class DecisionMenu : MonoBehaviour
{
    // Call this from a UI button's OnClick event, passing the scene name as a string
    public void GoToScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in build settings.");
        }
    }
}
