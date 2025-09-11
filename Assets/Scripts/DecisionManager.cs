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
    public List<DecisionOption> yesNoDecisions = new List<DecisionOption>();
    
    private bool robotDecisionMade = false;
    private bool yesNoDecisionMade = false;
    private bool isSecondChance = false;
    
    [System.Serializable]
    public class DecisionOption
    {
        public string choiceId;  // Unique identifier for this choice
        public string choiceText;
        public string targetScene;
    }
    
    void Start()
    {
        // Check current scene - only active in ID_Scene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (currentScene == "ID_Scene")
        {
            // Hide decision panel initially
            if (decisionPanel != null)
            {
                decisionPanel.SetActive(false);
            }
            Debug.Log("DecisionManager active in ID_Scene");
        }
        else
        {
            // In other scenes, deactivate this component
            this.enabled = false;
            Debug.Log($"DecisionManager disabled in {currentScene}");
        }
    }
    
    public void ShowDecisions()
    {
        Debug.Log("ShowDecisions() called - Stack trace:");
        Debug.Log(System.Environment.StackTrace);
        
        // Don't show robot decisions if already made
        if (robotDecisionMade)
        {
            Debug.Log("Robot decision already made, not showing buttons again");
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
        
        // Mark robot decision as made to prevent showing robot buttons again
        robotDecisionMade = true;
        
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
    
    public void ShowYesNoDecisions()
    {
        Debug.Log("ShowYesNoDecisions() called");
        
        // Don't show yes/no decisions if already made
        if (yesNoDecisionMade)
        {
            Debug.Log("Yes/No decision already made, not showing buttons again");
            return;
        }
        
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
            SetupYesNoButtons();
            
            // Enable UI interaction in PlayerController
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableUIInteraction();
            }
            
            // Change cursor to free mode for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("Yes/No decision panel shown");
        }
    }
    
    void SetupYesNoButtons()
    {
        Debug.Log($"SetupYesNoButtons called - YesNoDecisions count: {yesNoDecisions.Count}, DecisionButtons count: {decisionButtons.Count}");
        
        // Make sure we have enough buttons for the yes/no decisions
        int decisionsCount = Mathf.Min(yesNoDecisions.Count, decisionButtons.Count);
        
        Debug.Log($"Will setup {decisionsCount} yes/no buttons");
        
        // Set up each button with corresponding yes/no decision
        for (int i = 0; i < decisionsCount; i++)
        {
            DecisionOption option = yesNoDecisions[i];
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
                    Debug.Log($"Yes/No Button clicked: {choiceText}");
                    OnYesNoDecisionMade(choiceId, choiceText, sceneName);
                });
                
                Debug.Log($"Setup yes/no button {i}: '{choiceText}' - Button active: {button.gameObject.activeInHierarchy}");
            }
        }
        
        // Hide unused buttons
        for (int i = decisionsCount; i < decisionButtons.Count; i++)
        {
            if (decisionButtons[i] != null)
            {
                decisionButtons[i].gameObject.SetActive(false);
                Debug.Log($"Hiding unused yes/no button {i}: {decisionButtons[i].name}");
            }
        }
        
        Debug.Log($"Yes/No button setup complete. {decisionsCount} buttons should now be visible.");
    }
    
    public void OnYesNoDecisionMade(string choiceId, string choiceText, string targetScene)
    {
        Debug.Log($"OnYesNoDecisionMade called - Player chose: {choiceText} -> {targetScene}");
        
        // Mark yes/no decision as made
        yesNoDecisionMade = true;
        
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
        
        // Hide decision buttons
        foreach (Button button in decisionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
                Debug.Log($"Hiding yes/no button: {button.name}");
            }
        }
        Debug.Log("All yes/no decision buttons hidden after choice made");
        
        // Show different response based on choice
        if (choiceText.ToLower().Contains("yes"))
        {
            ShowFollowDialogue();
        }
        else
        {
            // For "No" choice
            if (isSecondChance)
            {
                // Second "No" - game over
                ShowGameOver();
            }
            else
            {
                // First "No" - show cooperation message and give second chance
                ShowCooperationDialogue();
            }
        }
    }
    
    void ShowFollowDialogue()
    {
        // Find DialogueManager and show the follow message
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.ShowFollowDialogue();
        }
        else
        {
            Debug.LogWarning("DialogueManager not found for follow dialogue");
        }
    }
    
    void ShowCooperationDialogue()
    {
        // Find DialogueManager and show the cooperation message
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.ShowCooperationDialogue();
        }
        else
        {
            Debug.LogWarning("DialogueManager not found for cooperation dialogue");
        }
    }
    
    public void ShowSecondYesNoDecisions()
    {
        Debug.Log("ShowSecondYesNoDecisions() called - second chance");
        
        // Mark this as second chance
        isSecondChance = true;
        
        // Reset the yes/no decision flag to allow showing buttons again
        yesNoDecisionMade = false;
        
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
            SetupYesNoButtons();
            
            // Enable UI interaction in PlayerController
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.EnableUIInteraction();
            }
            
            // Change cursor to free mode for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("Second chance Yes/No decision panel shown");
        }
    }
    
    void ShowGameOver()
    {
        Debug.Log("Game Over - Player refused cooperation twice");
        
        // Find and trigger the existing GameOverManager
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager not found - falling back to scene transition");
            // Fallback to scene transition if GameOverManager not found
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionToScene("Survey_Scene");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Survey_Scene");
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