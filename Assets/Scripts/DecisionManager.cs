using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject decisionPanel;
    public List<Button> decisionButtons = new List<Button>();
    
    [Header("Decision Options")]
    public List<DecisionOption> decisions = new List<DecisionOption>();
    
    private bool decisionMade = false;
    
    [System.Serializable]
    public class DecisionOption
    {
        public string choiceId;  // Unique identifier for this choice
        public string choiceText;
        public string targetScene;
    }
    
    void Start()
    {
        // Hide decision panel initially
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(false);
        }
    }
    
    public void ShowDecisions()
    {
        Debug.Log("ShowDecisions() called - Stack trace:");
        Debug.Log(System.Environment.StackTrace);
        
        // Don't show decisions if already made
        if (decisionMade)
        {
            Debug.Log("Decision already made, not showing buttons again");
            return;
        }
        
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
            SetupDecisionButtons();
            
            // Enable UI interaction in PlayerController
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableUIInteraction();
            }
            
            // Change cursor to free mode for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Debug UI setup
            UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
            Debug.Log($"EventSystem found: {eventSystem != null}");
            
            Canvas canvas = decisionPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                Debug.Log($"Canvas found: {canvas.name}, GraphicRaycaster: {raycaster != null}");
            }
            
            Debug.Log("Decision panel shown with choices - cursor confined for UI interaction");
        }
    }
    
    void SetupDecisionButtons()
    {
        Debug.Log($"SetupDecisionButtons called - Decisions count: {decisions.Count}, DecisionButtons count: {decisionButtons.Count}");
        
        // Make sure we have enough buttons for the decisions
        int decisionsCount = Mathf.Min(decisions.Count, decisionButtons.Count);
        
        Debug.Log($"Will setup {decisionsCount} buttons");
        
        // Set up each button with corresponding decision
        for (int i = 0; i < decisionsCount; i++)
        {
            DecisionOption option = decisions[i];
            Button button = decisionButtons[i];
            
            if (button != null)
            {
                // Show the button
                button.gameObject.SetActive(true);
                
                // Set button text
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText == null)
                {
                    // Try TextMeshPro if regular Text not found
                    TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = option.choiceText;
                    }
                }
                else
                {
                    buttonText.text = option.choiceText;
                }
                
                // Clear existing listeners and add new one
                button.onClick.RemoveAllListeners();
                
                // Capture variables for closure
                string choiceId = option.choiceId;
                string choiceText = option.choiceText;
                string sceneName = option.targetScene;
                
                // Add debug logging
                button.onClick.AddListener(() => {
                    Debug.Log($"Button clicked: {choiceText}");
                    OnDecisionMade(choiceId, choiceText, sceneName);
                });
                
                Debug.Log($"Setup button {i}: '{choiceText}' - Button active: {button.gameObject.activeInHierarchy}");
            }
        }
        
        // Hide unused buttons
        for (int i = decisionsCount; i < decisionButtons.Count; i++)
        {
            if (decisionButtons[i] != null)
            {
                decisionButtons[i].gameObject.SetActive(false);
                Debug.Log($"Hiding unused button {i}: {decisionButtons[i].name}");
            }
        }
        
        Debug.Log($"Button setup complete. {decisionsCount} buttons should now be visible.");
    }
    
    public void OnDecisionMade(string choiceId, string choiceText, string targetScene)
    {
        Debug.Log($"OnDecisionMade called - Player chose: {choiceText} -> {targetScene}");
        
        // Mark decision as made to prevent showing buttons again
        decisionMade = true;
        
        // Record the choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice(choiceId, choiceText, targetScene);
        }
        
        // Disable UI interaction in PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableUIInteraction();
        }
        
        // Return cursor to locked state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Hide decision buttons but keep panel active
        foreach (Button button in decisionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
                Debug.Log($"Hiding button: {button.name}");
            }
        }
        Debug.Log("All decision buttons hidden after choice made");
        
        // Show next dialogue line
        ShowNextDialogueLine();
    }
    
    void ShowNextDialogueLine()
    {
        // Find DialogueManager and show the ID verification text
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.ShowIDVerificationDialogue();
        }
        else
        {
            Debug.LogWarning("DialogueManager not found for next dialogue line");
        }
    }
    
    public void HideDecisions()
    {
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(false);
        }
        
        // Disable UI interaction in PlayerController
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableUIInteraction();
        }
        
        // Return cursor to locked state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Hide all decision buttons
        foreach (Button button in decisionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
    
    // Call this method to trigger the decision system
    public void TriggerDecision()
    {
        Debug.Log("TriggerDecision() called");
        ShowDecisions();
    }
}