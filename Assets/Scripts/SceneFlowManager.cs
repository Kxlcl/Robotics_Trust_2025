using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Central manager that handles all scene transitions, dialogue, and decisions
/// Automatically loads and executes SceneFlowData for each scene
/// </summary>
public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;

    [Header("Scene Flow Configuration")]
    public List<SceneFlowData> sceneFlows = new List<SceneFlowData>();

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject decisionPanel;
    public List<Button> decisionButtons = new List<Button>();

    [Header("Runtime State")]
    private SceneFlowData currentFlow;
    private int currentStepIndex = 0;
    private bool isProcessingStep = false;
    private List<SceneFlowData.DecisionOption> currentDecisions;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"SceneFlowManager: Scene loaded - {scene.name}");

        // Find UI elements in new scene
        FindUIElements();

        // Load and start the flow for this scene
        LoadSceneFlow(scene.name);
    }

    void FindUIElements()
    {
        // Find dialogue panel
        if (dialoguePanel == null)
        {
            GameObject panel = GameObject.Find("DialoguePanel");
            if (panel != null)
            {
                dialoguePanel = panel;
                dialogueText = panel.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // Find decision panel
        if (decisionPanel == null)
        {
            GameObject panel = GameObject.Find("DecisionPanel");
            if (panel != null)
            {
                decisionPanel = panel;
                // Get all buttons in decision panel
                Button[] buttons = panel.GetComponentsInChildren<Button>(true);
                decisionButtons.Clear();
                decisionButtons.AddRange(buttons);
            }
        }

        Debug.Log($"UI Found - Dialogue: {dialoguePanel != null}, Decisions: {decisionPanel != null}, Buttons: {decisionButtons.Count}");
    }

    void LoadSceneFlow(string sceneName)
    {
        // Find the flow data for this scene
        currentFlow = sceneFlows.Find(flow => flow.sceneName == sceneName);

        if (currentFlow == null)
        {
            Debug.LogWarning($"No SceneFlowData found for scene: {sceneName}");
            return;
        }

        Debug.Log($"Loaded flow for scene: {sceneName} with {currentFlow.dialogueSteps.Count} steps");

        // Reset state
        currentStepIndex = 0;
        isProcessingStep = false;

        // Start the flow
        StartCoroutine(ProcessSceneFlow());
    }

    IEnumerator ProcessSceneFlow()
    {
        if (currentFlow == null || currentFlow.dialogueSteps.Count == 0)
            yield break;

        while (currentStepIndex < currentFlow.dialogueSteps.Count)
        {
            yield return StartCoroutine(ExecuteStep(currentStepIndex));

            // If we're waiting for a trigger or decision, stop automatic progression
            var step = currentFlow.dialogueSteps[currentStepIndex];
            if (step.afterAction == SceneFlowData.StepAction.WaitForTrigger ||
                step.afterAction == SceneFlowData.StepAction.ShowDecisions)
            {
                yield break; // Exit and wait for external trigger
            }

            currentStepIndex++;
        }

        // All steps completed, transition to next scene if specified
        if (!string.IsNullOrEmpty(currentFlow.nextSceneName))
        {
            yield return new WaitForSeconds(currentFlow.transitionDelay);
            TransitionToScene(currentFlow.nextSceneName);
        }
    }

    IEnumerator ExecuteStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= currentFlow.dialogueSteps.Count)
            yield break;

        isProcessingStep = true;
        var step = currentFlow.dialogueSteps[stepIndex];

        Debug.Log($"Executing step {stepIndex}: {step.dialogueText.Substring(0, Mathf.Min(30, step.dialogueText.Length))}...");

        // Show dialogue
        ShowDialogue(step.dialogueText);

        // Wait for display duration
        yield return new WaitForSeconds(step.displayDuration);

        // Execute after-action
        switch (step.afterAction)
        {
            case SceneFlowData.StepAction.ShowDecisions:
                if (step.showDecisions && step.decisionOptions.Count > 0)
                {
                    ShowDecisions(step.decisionOptions);
                }
                break;

            case SceneFlowData.StepAction.TransitionScene:
                if (!string.IsNullOrEmpty(step.nextSceneIfChosen))
                {
                    TransitionToScene(step.nextSceneIfChosen);
                }
                break;

            case SceneFlowData.StepAction.WaitForTrigger:
                HideDialogue();
                break;

            case SceneFlowData.StepAction.EndScene:
                HideDialogue();
                break;

            case SceneFlowData.StepAction.NextDialogue:
                // Continue to next step automatically
                break;
        }

        isProcessingStep = false;
    }

    void ShowDialogue(string text)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            if (dialogueText != null)
            {
                dialogueText.text = text;
            }
        }
    }

    void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void ShowDecisions(List<SceneFlowData.DecisionOption> options)
    {
        currentDecisions = options;

        if (decisionPanel != null)
        {
            decisionPanel.SetActive(true);
        }

        // Setup decision buttons
        for (int i = 0; i < decisionButtons.Count; i++)
        {
            if (i < options.Count)
            {
                Button button = decisionButtons[i];
                button.gameObject.SetActive(true);

                // Get option for this button
                SceneFlowData.DecisionOption option = options[i];

                // Set button text
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText == null)
                {
                    Text legacyText = button.GetComponentInChildren<Text>();
                    if (legacyText != null)
                    {
                        string hotkeyText = GetHotkeyString(option.hotkey);
                        legacyText.text = $"({hotkeyText}) {option.choiceText}";
                    }
                }
                else
                {
                    string hotkeyText = GetHotkeyString(option.hotkey);
                    buttonText.text = $"({hotkeyText}) {option.choiceText}";
                }

                // Setup button click (capture index in closure)
                int choiceIndex = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnChoiceMade(choiceIndex));
            }
            else
            {
                decisionButtons[i].gameObject.SetActive(false);
            }
        }

        Debug.Log($"Showing {options.Count} decision options");
    }

    string GetHotkeyString(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Q: return "Q";
            case KeyCode.E: return "E";
            case KeyCode.W: return "W";
            case KeyCode.A: return "A";
            case KeyCode.S: return "S";
            case KeyCode.D: return "D";
            default: return key.ToString();
        }
    }

    void Update()
    {
        // Handle hotkey input for decisions
        if (currentDecisions != null && currentDecisions.Count > 0)
        {
            for (int i = 0; i < currentDecisions.Count; i++)
            {
                if (Input.GetKeyDown(currentDecisions[i].hotkey))
                {
                    OnChoiceMade(i);
                    break;
                }
            }
        }
    }

    public void OnChoiceMade(int choiceIndex)
    {
        if (currentDecisions == null || choiceIndex >= currentDecisions.Count)
            return;

        var choice = currentDecisions[choiceIndex];

        Debug.Log($"Choice made: {choice.choiceText}");

        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice(
                choice.choiceId,
                choice.choiceText,
                choice.targetScene ?? currentFlow.nextSceneName
            );
        }

        // Hide decisions
        HideDecisions();
        currentDecisions = null;

        // Execute outcome
        StartCoroutine(ExecuteChoiceOutcome(choice));
    }

    IEnumerator ExecuteChoiceOutcome(SceneFlowData.DecisionOption choice)
    {
        switch (choice.outcome)
        {
            case SceneFlowData.ChoiceOutcome.Continue:
                // Continue to next step
                currentStepIndex++;
                yield return StartCoroutine(ProcessSceneFlow());
                break;

            case SceneFlowData.ChoiceOutcome.JumpToStep:
                // Jump to specific step
                if (choice.nextStepIndex >= 0 && choice.nextStepIndex < currentFlow.dialogueSteps.Count)
                {
                    currentStepIndex = choice.nextStepIndex;
                    yield return StartCoroutine(ProcessSceneFlow());
                }
                break;

            case SceneFlowData.ChoiceOutcome.SceneTransition:
                // Transition to target scene
                if (!string.IsNullOrEmpty(choice.targetScene))
                {
                    yield return new WaitForSeconds(1f);

                    // Reposition player if needed
                    if (currentFlow.repositionPlayer)
                    {
                        RepositionPlayer(currentFlow.newPlayerPosition);
                        yield return new WaitForSeconds(1f);
                    }

                    TransitionToScene(choice.targetScene);
                }
                break;

            case SceneFlowData.ChoiceOutcome.GameOver:
                // Trigger game over
                GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
                if (gameOverManager != null)
                {
                    gameOverManager.ShowGameOver();
                }
                break;
        }
    }

    void HideDecisions()
    {
        if (decisionPanel != null)
        {
            decisionPanel.SetActive(false);
        }

        foreach (var button in decisionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    void RepositionPlayer(Vector3 newPosition)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.transform.position = newPosition;
            Debug.Log($"Player repositioned to {newPosition}");
        }
    }

    void TransitionToScene(string sceneName)
    {
        Debug.Log($"Transitioning to scene: {sceneName}");

        // Use SceneTransitionManager if available
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneName);
        }
        else
        {
            // Fallback to direct load
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Manually trigger continuation (for external events)
    /// </summary>
    public void ContinueFlow()
    {
        if (!isProcessingStep && currentFlow != null)
        {
            currentStepIndex++;
            StartCoroutine(ProcessSceneFlow());
        }
    }

    /// <summary>
    /// Manually jump to a specific step
    /// </summary>
    public void JumpToStep(int stepIndex)
    {
        if (currentFlow != null && stepIndex >= 0 && stepIndex < currentFlow.dialogueSteps.Count)
        {
            currentStepIndex = stepIndex;
            StartCoroutine(ProcessSceneFlow());
        }
    }
}
