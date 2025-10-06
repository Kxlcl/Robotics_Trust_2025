using UnityEngine;

public class SimpleWaitingManager : MonoBehaviour
{
    void Start()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"SimpleWaitingManager Start() - Current scene: '{currentScene}'");
        Debug.Log("Waiting_Scene loaded - clean state");
        
        // Don't hide decision buttons - they'll be needed for work/wait choices
        Debug.Log("SimpleWaitingManager ready - DecisionManager will be triggered by PlayerController");
    }
    
    private System.Collections.IEnumerator DelayedHideDecisionButtons()
    {
        // Wait a few frames for all components to be loaded
        yield return null;
        yield return null;
        yield return null;
        
        HideDecisionButtons();
    }
    
    private void HideDecisionButtons()
    {
        // Find and hide decision manager buttons
        DecisionManager decisionManager = FindObjectOfType<DecisionManager>();
        if (decisionManager != null)
        {
            // Hide the decision panel completely
            if (decisionManager.decisionPanel != null)
            {
                decisionManager.decisionPanel.SetActive(false);
                Debug.Log("Decision panel hidden in waiting scene");
            }
            
            // Hide all decision buttons
            if (decisionManager.decisionButtons != null)
            {
                foreach (var button in decisionManager.decisionButtons)
                {
                    if (button != null)
                    {
                        button.gameObject.SetActive(false);
                    }
                }
                Debug.Log("All decision buttons hidden in waiting scene");
            }
            
            // Disable the DecisionManager component
            decisionManager.enabled = false;
            Debug.Log("DecisionManager disabled in waiting scene");
        }
        else
        {
            Debug.Log("No DecisionManager found in waiting scene");
        }
    }
}