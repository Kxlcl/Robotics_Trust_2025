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
    public List<DecisionOption> workWaitDecisions = new List<DecisionOption>();
    
    private bool robotDecisionMade = false;
    private bool yesNoDecisionMade = false;
    private bool isSecondChance = false;
    private bool awaitingKeyInput = false;
    private List<DecisionOption> currentOptions = new List<DecisionOption>();
    
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
        else if (currentScene == "Waiting_Scene")
        {
            // In Waiting_Scene, hide decision panel until triggered by dialogue
            if (decisionPanel != null)
            {
                decisionPanel.SetActive(false);
            }
            Debug.Log("DecisionManager active in Waiting_Scene - waiting for dialogue trigger");
        }
        else
        {
            // In other scenes, deactivate this component
            this.enabled = false;
            Debug.Log($"DecisionManager disabled in {currentScene}");
        }
    }
    
    void Update()
    {
        if (awaitingKeyInput && currentOptions.Count >= 2)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectOption(0);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SelectOption(1);
            }
        }
    }
    
    private void SelectOption(int optionIndex)
    {
        if (optionIndex < currentOptions.Count)
        {
            DecisionOption selectedOption = currentOptions[optionIndex];
            awaitingKeyInput = false;
            currentOptions.Clear();
            
            // Determine which type of decision this is based on the selected option
            if (workWaitDecisions.Contains(selectedOption))
            {
                OnWorkWaitDecisionMade(selectedOption.choiceId, selectedOption.choiceText, selectedOption.targetScene);
            }
            else if (robotDecisionMade == false)
            {
                OnDecisionMade(selectedOption.choiceId, selectedOption.choiceText, selectedOption.targetScene);
            }
            else
            {
                OnYesNoDecisionMade(selectedOption.choiceId, selectedOption.choiceText, selectedOption.targetScene);
            }
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
            SetupDecisionDisplay();
            
            // Set up key input
            currentOptions.Clear();
            currentOptions.AddRange(decisions);
            awaitingKeyInput = true;
            
            // Keep cursor locked since we're using keyboard input
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
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
    
    void SetupDecisionDisplay()
    {
        Debug.Log($"SetupDecisionDisplay called - Decisions count: {decisions.Count}, DecisionButtons count: {decisionButtons.Count}");
        
        // Make sure we have enough buttons for the decisions
        int decisionsCount = Mathf.Min(decisions.Count, decisionButtons.Count);
        
        Debug.Log($"Will setup {decisionsCount} options for key selection");
        
        // Set up each button display (no click listeners needed)
        for (int i = 0; i < decisionsCount; i++)
        {
            DecisionOption option = decisions[i];
            Button button = decisionButtons[i];
            
            if (button != null)
            {
                // Show the button
                button.gameObject.SetActive(true);
                
                // Set button text with key indicator
                Text buttonText = button.GetComponentInChildren<Text>();
                string keyText = i == 0 ? "(Q) " : "(E) ";
                string displayText = keyText + option.choiceText;
                
                if (buttonText == null)
                {
                    // Try TextMeshPro if regular Text not found
                    TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = displayText;
                    }
                }
                else
                {
                    buttonText.text = displayText;
                }
                
                // Remove all click listeners since we're using keys
                button.onClick.RemoveAllListeners();
                
                Debug.Log($"Setup option {i}: '{displayText}' - Press {(i == 0 ? "Q" : "E")} to select");
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
        
        Debug.Log($"Display setup complete. {decisionsCount} options shown. Press Q or E to select.");
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
        
        awaitingKeyInput = false;
        currentOptions.Clear();
        
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
            SetupYesNoDisplay();
            
            // Set up key input for yes/no decisions
            currentOptions.Clear();
            currentOptions.AddRange(yesNoDecisions);
            awaitingKeyInput = true;
            
            Debug.Log("Yes/No decision panel shown - Press Q or E to select");
        }
    }
    
    void SetupYesNoDisplay()
    {
        Debug.Log($"SetupYesNoDisplay called - YesNoDecisions count: {yesNoDecisions.Count}, DecisionButtons count: {decisionButtons.Count}");
        
        // Make sure we have enough buttons for the yes/no decisions
        int decisionsCount = Mathf.Min(yesNoDecisions.Count, decisionButtons.Count);
        
        Debug.Log($"Will setup {decisionsCount} yes/no options for key selection");
        
        // Set up each button display for yes/no decision
        for (int i = 0; i < decisionsCount; i++)
        {
            DecisionOption option = yesNoDecisions[i];
            Button button = decisionButtons[i];
            
            if (button != null)
            {
                // Show the button
                button.gameObject.SetActive(true);
                
                // Set button text with key indicator
                Text buttonText = button.GetComponentInChildren<Text>();
                string keyText = i == 0 ? "(Q) " : "(E) ";
                string displayText = keyText + option.choiceText;
                
                if (buttonText == null)
                {
                    // Try TextMeshPro if regular Text not found
                    TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = displayText;
                    }
                }
                else
                {
                    buttonText.text = displayText;
                }
                
                // Remove all click listeners since we're using keys
                button.onClick.RemoveAllListeners();
                
                Debug.Log($"Setup yes/no option {i}: '{displayText}' - Press {(i == 0 ? "Q" : "E")} to select");
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
        
        Debug.Log($"Yes/No display setup complete. {decisionsCount} options shown. Press Q or E to select.");
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
            SetupYesNoDisplay();
            
            // Set up key input for second chance yes/no decisions
            currentOptions.Clear();
            currentOptions.AddRange(yesNoDecisions);
            awaitingKeyInput = true;
            
            Debug.Log("Second chance Yes/No decision panel shown - Press Q or E to select");
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
            Debug.LogWarning("GameOverManager not found - falling back to direct survey redirect");
            
            // Fallback: redirect directly to survey if GameOverManager not found
#if UNITY_WEBGL && !UNITY_EDITOR
            // Call the same JavaScript function that GameOverManager uses
            Application.ExternalEval("window.location.href = 'survey.html';");
#else
            Debug.Log("Survey redirect would happen here (WebGL only)");
#endif
        }
    }
    
    public void ShowWorkWaitDecisions()
    {
        Debug.Log("ShowWorkWaitDecisions() called");
        
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
            SetupWorkWaitDisplay();
            
            // Set up key input for work/wait decisions
            currentOptions.Clear();
            currentOptions.AddRange(workWaitDecisions);
            awaitingKeyInput = true;
            
            Debug.Log("Work/Wait decision panel shown - Press Q or E to select");
        }
    }
    
    void SetupWorkWaitDisplay()
    {
        Debug.Log($"SetupWorkWaitDisplay called - WorkWaitDecisions count: {workWaitDecisions.Count}, DecisionButtons count: {decisionButtons.Count}");
        
        // Make sure we have enough buttons for the work/wait decisions
        int decisionsCount = Mathf.Min(workWaitDecisions.Count, decisionButtons.Count);
        
        Debug.Log($"Will setup {decisionsCount} work/wait options for key selection");
        
        // Set up each button display for work/wait decision
        for (int i = 0; i < decisionsCount; i++)
        {
            DecisionOption option = workWaitDecisions[i];
            Button button = decisionButtons[i];
            
            if (button != null)
            {
                // Show the button
                button.gameObject.SetActive(true);
                
                // Set button text with key indicator
                Text buttonText = button.GetComponentInChildren<Text>();
                string keyText = i == 0 ? "(Q) " : "(E) ";
                string displayText = keyText + option.choiceText;
                
                if (buttonText == null)
                {
                    // Try TextMeshPro if regular Text not found
                    TMPro.TextMeshProUGUI tmpText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = displayText;
                    }
                }
                else
                {
                    buttonText.text = displayText;
                }
                
                // Remove all click listeners since we're using keys
                button.onClick.RemoveAllListeners();
                
                Debug.Log($"Setup work/wait option {i}: '{displayText}' - Press {(i == 0 ? "Q" : "E")} to select");
            }
        }
        
        // Hide unused buttons
        for (int i = decisionsCount; i < decisionButtons.Count; i++)
        {
            if (decisionButtons[i] != null)
            {
                decisionButtons[i].gameObject.SetActive(false);
                Debug.Log($"Hiding unused work/wait button {i}: {decisionButtons[i].name}");
            }
        }
        
        Debug.Log($"Work/Wait display setup complete. {decisionsCount} options shown. Press Q or E to select.");
    }
    
    public void OnWorkWaitDecisionMade(string choiceId, string choiceText, string targetScene)
    {
        Debug.Log($"OnWorkWaitDecisionMade called - Player chose: {choiceText} -> {targetScene}");
        
        // Record the choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice(choiceId, choiceText, targetScene);
        }
        
        // Hide decision buttons
        foreach (Button button in decisionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
                Debug.Log($"Hiding work/wait button: {button.name}");
            }
        }
        Debug.Log("All work/wait decision buttons hidden after choice made");
        
        // Handle different choices
        if (choiceText.ToLower().Contains("ask") || choiceText.ToLower().Contains("work"))
        {
            // Player chose to ask about leaving for work
            Debug.Log("Player chose to ask about leaving for work");
            // Add your logic here for what happens when they ask about work
        }
        else
        {
            // Player chose to stay put and wait
            Debug.Log("Player chose to stay put and wait");
            // Add your logic here for what happens when they wait
        }
    }
    
    // Call this method to trigger the decision system
    public void TriggerDecision()
    {
        Debug.Log("TriggerDecision() called");
        ShowDecisions();
    }
}