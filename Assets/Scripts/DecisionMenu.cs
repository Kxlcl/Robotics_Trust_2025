using UnityEngine;
using UnityEngine.SceneManagement;

public class DecisionMenu : MonoBehaviour
{
    // Static variable to track which branch was chosen
    public static int chosenBranch = 0;

    // Call these from UI buttons for each choice
    public void ChooseBranchAndGoToScene(int branchIndex, string sceneName)
    {
        chosenBranch = branchIndex;
        GoToScene(sceneName);
    }

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
