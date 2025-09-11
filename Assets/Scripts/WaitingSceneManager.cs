using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingSceneManager : MonoBehaviour
{
    void Start()
    {
        // Only active in Waiting_Scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "Waiting_Scene")
        {
            this.enabled = false;
            return;
        }
        
        Debug.Log("Waiting_Scene initialized");
        
        // Ensure proper state without destroying dialogue system
        EnsureProperState();
    }
    
    void EnsureProperState()
    {
        // Only hide decision buttons, not the entire dialogue system
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button button in allButtons)
        {
            // Only hide buttons that are decision-related (robot choices, yes/no)
            if (button.name.Contains("Robot") || button.name.Contains("Yes") || button.name.Contains("No") || 
                button.name.Contains("Decision") || button.name.Contains("Choice"))
            {
                button.gameObject.SetActive(false);
                Debug.Log($"Hidden decision button: {button.name}");
            }
        }
        
        // Ensure DialogueManager is properly disabled for this scene
        DialogueManager[] dialogueManagers = FindObjectsOfType<DialogueManager>();
        foreach (DialogueManager dlg in dialogueManagers)
        {
            // Don't destroy, just disable for Waiting_Scene
            dlg.enabled = false;
            if (dlg.dialoguePanel != null)
            {
                dlg.dialoguePanel.SetActive(false);
            }
            Debug.Log("DialogueManager disabled for Waiting_Scene");
        }
        
        // Ensure DecisionManager is disabled
        DecisionManager[] decisionManagers = FindObjectsOfType<DecisionManager>();
        foreach (DecisionManager dm in decisionManagers)
        {
            dm.enabled = false;
            Debug.Log("DecisionManager disabled for Waiting_Scene");
        }
        
        // Ensure timer is working and visible
        if (GlobalTimer.Instance != null && GlobalTimer.Instance.timerText != null)
        {
            GlobalTimer.Instance.timerText.gameObject.SetActive(true);
            Debug.Log("Timer confirmed active in Waiting_Scene");
        }
        
        Debug.Log("Waiting_Scene state cleaned - dialogue system preserved, only decision buttons hidden");
    }
}