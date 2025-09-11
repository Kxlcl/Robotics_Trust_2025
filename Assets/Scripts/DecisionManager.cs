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
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
            SetupDecisionButtons();
            Debug.Log("Decision panel shown with choices");
        }
    }
    
    void SetupDecisionButtons()
    {
        // Make sure we have enough buttons for the decisions
        int decisionsCount = Mathf.Min(decisions.Count, decisionButtons.Count);
        
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
                button.onClick.AddListener(() => OnDecisionMade(choiceId, choiceText, sceneName));
            }
        }
        
        // Hide unused buttons
        for (int i = decisionsCount; i < decisionButtons.Count; i++)
        {
            if (decisionButtons[i] != null)
            {
                decisionButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void OnDecisionMade(string choiceId, string choiceText, string targetScene)
    {
        Debug.Log($"Player chose: {choiceText} -> {targetScene}");
        
        // Record the choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice(choiceId, choiceText, targetScene);
        }
        
        // Hide decision panel
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(false);
        }
        
        // Transition to chosen scene
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(targetScene);
        }
        else
        {
            // Fallback direct scene load
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
        }
    }
    
    public void HideDecisions()
    {
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(false);
        }
        
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
        ShowDecisions();
    }
}