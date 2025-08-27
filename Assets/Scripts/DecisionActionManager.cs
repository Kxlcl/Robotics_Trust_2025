using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecisionActionManager : MonoBehaviour
{
    [System.Serializable]
    public class DecisionChoice
    {
        [Header("Choice Settings")]
        public string choiceText;
        public int choiceValue;
        
        [Header("Actions")]
        public UnityEvent onChoiceSelected;
        
        [Header("Script References")]
        public MonoBehaviour[] scriptsToEnable;
        public MonoBehaviour[] scriptsToDisable;
        
        [Header("Scene/Area Transition")]
        public bool changeScene = false;
        public string targetScene;
        public bool changeArea = false;
        public string targetArea;
    }

    [Header("Decision Setup")]
    public string decisionQuestion;
    public DecisionChoice[] choices;
    
    [Header("UI References")]
    public Text questionText;
    public Transform buttonContainer;
    public Button buttonPrefab;
    
    [Header("Tracking")]
    public static Dictionary<string, int> decisionHistory = new Dictionary<string, int>();
    
    [Header("Game State Management")]
    public int decisionsBeforePivot = 3;
    public UnityEvent onGamePivot;
    public static int totalDecisions = 0;
    private static bool hasPivoted = false;
    
    private List<Button> createdButtons = new List<Button>();

    void Start()
    {
        SetupDecisionUI();
    }

    void SetupDecisionUI()
    {
        // Set question text
        if (questionText != null)
            questionText.text = decisionQuestion;

        // Clear existing buttons
        ClearButtons();

        // Create buttons for each choice
        for (int i = 0; i < choices.Length; i++)
        {
            CreateChoiceButton(i);
        }
    }

    public void SetupWithUI(DecisionUI ui)
    {
        if (ui != null)
        {
            questionText = ui.questionText;
            buttonContainer = ui.buttonContainer;
            buttonPrefab = ui.buttonPrefab;
        }
    }

    void CreateChoiceButton(int choiceIndex)
    {
        if (buttonPrefab == null || buttonContainer == null) return;

        Button newButton = Instantiate(buttonPrefab, buttonContainer);
        Text buttonText = newButton.GetComponentInChildren<Text>();
        
        if (buttonText != null)
            buttonText.text = choices[choiceIndex].choiceText;

        // Add click listener
        int index = choiceIndex; // Capture for closure
        newButton.onClick.AddListener(() => SelectChoice(index));
        
        createdButtons.Add(newButton);
    }

    public void SelectChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= choices.Length) return;

        DecisionChoice selectedChoice = choices[choiceIndex];
        
        // Record decision in history
        string decisionKey = gameObject.name + "_" + decisionQuestion;
        decisionHistory[decisionKey] = selectedChoice.choiceValue;

        // Execute choice actions
        ExecuteChoiceActions(selectedChoice);
        
        // Increment decision counter and check for pivot
        totalDecisions++;
        CheckForGamePivot();
        
        // Notify UI that choice was made
        DecisionUI ui = FindObjectOfType<DecisionUI>();
        if (ui != null)
            ui.OnChoiceMade();
        
        // Hide decision UI
        gameObject.SetActive(false);
    }

    void ExecuteChoiceActions(DecisionChoice choice)
    {
        // Enable/disable scripts
        EnableScripts(choice.scriptsToEnable);
        DisableScripts(choice.scriptsToDisable);

        // Invoke Unity Events
        choice.onChoiceSelected?.Invoke();

        // Handle scene/area transitions
        if (choice.changeScene && !string.IsNullOrEmpty(choice.targetScene))
        {
            FindObjectOfType<DecisionMenu>()?.GoToScene(choice.targetScene);
        }
        else if (choice.changeArea && !string.IsNullOrEmpty(choice.targetArea))
        {
            FindObjectOfType<SharedMapManager>()?.TransitionToArea(choice.targetArea);
        }
    }

    void EnableScripts(MonoBehaviour[] scripts)
    {
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
                script.enabled = true;
        }
    }

    void DisableScripts(MonoBehaviour[] scripts)
    {
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    void ClearButtons()
    {
        foreach (Button button in createdButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        createdButtons.Clear();
    }

    // Static method to check previous decisions
    public static int GetDecisionValue(string decisionKey)
    {
        return decisionHistory.ContainsKey(decisionKey) ? decisionHistory[decisionKey] : -1;
    }

    // Check if we should pivot the game state
    void CheckForGamePivot()
    {
        if (!hasPivoted && totalDecisions >= decisionsBeforePivot)
        {
            hasPivoted = true;
            onGamePivot?.Invoke();
        }
    }

    // Method to show this decision panel
    public void ShowDecision()
    {
        gameObject.SetActive(true);
        SetupDecisionUI();
    }
    
    // Static methods for external access
    public static bool HasGamePivoted()
    {
        return hasPivoted;
    }
    
    public static int GetTotalDecisions()
    {
        return totalDecisions;
    }
    
    public static void ResetGameState()
    {
        totalDecisions = 0;
        hasPivoted = false;
    }
}