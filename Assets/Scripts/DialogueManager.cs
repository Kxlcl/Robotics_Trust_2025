using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMPro.TMP_Text dialogueText;
    public float textSpeed = 0.05f;

    [Header("Inline Decision Buttons")]
    public GameObject inlineButtonContainer;
    public UnityEngine.UI.Button inlineButton1;
    public UnityEngine.UI.Button inlineButton2;
    public TMPro.TMP_Text inlineButton1Text;
    public TMPro.TMP_Text inlineButton2Text;
    private CanvasGroup inlineButtonCanvasGroup;

    [Header("Single Button")]
    public GameObject singleButtonContainer;
    public UnityEngine.UI.Button singleButton;
    public TMPro.TMP_Text singleButtonText;
    
    [Header("Player Repositioning")]
    public Vector3 newPlayerPosition = new Vector3(170f, 0f, -95f);
    
    [Header("Audio")]
    public AudioSource backgroundAudioSource;
    public AudioClip newAudioClip;
    
    private string[] dialogueLines;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool inlineButtonsActive = false;
    private bool singleButtonActive = false;
    private System.Action currentOption1Callback;
    private System.Action currentOption2Callback;
    private System.Action currentSingleButtonCallback;
    
    void Start()
    {
        // FIRST: Set flags to false BEFORE anything else
        inlineButtonsActive = false;
        singleButtonActive = false;
        Debug.Log($"INITIAL FLAGS SET - inlineButtonsActive: {inlineButtonsActive}, singleButtonActive: {singleButtonActive}");

        LoadDialogue();

        // Setup inline buttons with CanvasGroup for visibility control
        if (inlineButtonContainer != null)
        {
            // Make sure container and children are active
            inlineButtonContainer.SetActive(true);
            if (inlineButton1 != null) inlineButton1.gameObject.SetActive(true);
            if (inlineButton2 != null) inlineButton2.gameObject.SetActive(true);

            // Get or add CanvasGroup component for visibility control
            inlineButtonCanvasGroup = inlineButtonContainer.GetComponent<CanvasGroup>();
            if (inlineButtonCanvasGroup == null)
            {
                inlineButtonCanvasGroup = inlineButtonContainer.AddComponent<CanvasGroup>();
                Debug.Log("Added CanvasGroup to InlineButtonContainer");
            }

            // TEMP: Don't hide buttons - leave them visible for testing
            inlineButtonCanvasGroup.alpha = 1f;
            inlineButtonCanvasGroup.interactable = true;
            inlineButtonCanvasGroup.blocksRaycasts = true;
            Debug.Log("TEMP: Inline button container LEFT VISIBLE for testing");
        }

        if (singleButtonContainer != null)
        {
            if (singleButton != null) singleButton.gameObject.SetActive(true);
            singleButtonContainer.SetActive(false);
            Debug.Log("Single button container initialized and hidden");
        }

        Debug.Log($"After button setup - inlineButtonsActive: {inlineButtonsActive}");

        // Canvas stays active - dialogue panel will just show empty text if no content

        // Check if we're in ID_Scene and show appropriate dialogue
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentScene}");

        if (currentScene == "ID_Scene")
        {
            ShowIDSceneDialogue();
            Debug.Log($"After ShowIDSceneDialogue - inlineButtonsActive: {inlineButtonsActive}");
        }
        else if (currentScene == "Waiting_Scene")
        {
            // In Waiting_Scene, keep dialogue hidden until triggered by PlayerController
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            Debug.Log($"DialogueManager active in {currentScene} - waiting for trigger");
        }
        else
        {
            // In other scenes, keep dialogue completely hidden
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            // Disable this component in non-ID scenes
            this.enabled = false;
            Debug.Log($"DialogueManager disabled in {currentScene}");
        }
    }

    void Update()
    {
        // Handle keyboard input for inline buttons
        if (inlineButtonsActive)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentOption1Callback?.Invoke();
                HideInlineButtons();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                currentOption2Callback?.Invoke();
                HideInlineButtons();
            }
        }

        // Handle keyboard input for single button
        if (singleButtonActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentSingleButtonCallback?.Invoke();
                HideSingleButton();
            }
        }
    }
    
    void ShowIDSceneDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "Choose a robot to follow.";
        Debug.Log("Showing ID_Scene dialogue: Choose a robot to follow");

        // NOTE: No longer auto-triggering DecisionManager Q/E buttons
        // Players now use the crosshair hover + E interaction system instead
        // The single button will appear when they press E while looking at a robot
    }
    
    System.Collections.IEnumerator TriggerDecisionAfterDelay()
    {
        // Wait a few seconds for player to read the text
        Debug.Log("Starting 3 second delay before showing decisions");
        yield return new WaitForSeconds(3f);
        
        Debug.Log("3 seconds passed, looking for DecisionManager");
        
        // Find and trigger decision manager
        DecisionManager decisionManager = FindObjectOfType<DecisionManager>();
        if (decisionManager != null)
        {
            Debug.Log("DecisionManager found, calling TriggerDecision()");
            decisionManager.TriggerDecision();
        }
        else
        {
            Debug.LogWarning("DecisionManager not found in scene");
        }
    }
    
    public void ShowIDVerificationDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "Please provide a form of identification to verify your presence at the train station today. Time is critical. Please cooperate for your safety and others.";
        Debug.Log("Showing ID verification dialogue");
        
        // Trigger yes/no decision after a delay
        StartCoroutine(TriggerYesNoDecisionAfterDelay());
    }
    
    System.Collections.IEnumerator TriggerYesNoDecisionAfterDelay()
    {
        // Wait a few seconds for player to read the ID verification text
        Debug.Log("Starting delay before showing yes/no decisions");
        yield return new WaitForSeconds(4f);
        
        Debug.Log("Triggering yes/no decision options");
        
        // Find and trigger decision manager for yes/no options
        DecisionManager decisionManager = FindObjectOfType<DecisionManager>();
        if (decisionManager != null)
        {
            decisionManager.ShowYesNoDecisions();
        }
        else
        {
            Debug.LogWarning("DecisionManager not found for yes/no options");
        }
    }
    
    public void ShowFollowDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "Thank you for your cooperation. Please follow me and the other passengers in my group to be escorted to a safe area.";
        Debug.Log("Showing follow dialogue after yes choice");
        
        // Reposition player and transition to Waiting_Scene after 3 seconds
        StartCoroutine(RepositionPlayerAndTransition());
    }
    
    System.Collections.IEnumerator RepositionPlayerAndTransition()
    {
        yield return new WaitForSeconds(3f);
        
        // Reposition the player to new location
        RepositionPlayer();
        
        // Wait a moment then transition
        yield return new WaitForSeconds(2f);
        
        dialoguePanel.SetActive(false);
        Debug.Log("Player repositioned, transitioning to Waiting_Scene");
        
        // Create SceneTransitionManager if it doesn't exist
        if (SceneTransitionManager.Instance == null)
        {
            Debug.Log("SceneTransitionManager not found, creating one");
            GameObject transitionManagerObj = new GameObject("SceneTransitionManager");
            transitionManagerObj.AddComponent<SceneTransitionManager>();
        }
        
        // Transition to Waiting_Scene
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Waiting_Scene");
        }
        else
        {
            // Fallback direct scene load
            Debug.LogWarning("Failed to create SceneTransitionManager, using direct scene load");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Waiting_Scene");
        }
    }
    
    void RepositionPlayer()
    {
        // Find the player and move them to new position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            // Use configured position, but keep current Y if newPlayerPosition.y is 0
            Vector3 targetPosition = newPlayerPosition;
            if (newPlayerPosition.y == 0f)
            {
                targetPosition.y = player.transform.position.y; // Keep current height
            }
            
            player.transform.position = targetPosition;
            Debug.Log($"Player repositioned to {targetPosition}");
        }
        else
        {
            Debug.LogWarning("PlayerController not found for repositioning");
        }
    }
    
    System.Collections.IEnumerator HideDialogueAndTransition()
    {
        yield return new WaitForSeconds(5f);
        
        dialoguePanel.SetActive(false);
        Debug.Log("Dialogue hidden, transitioning to Waiting_Scene");
        
        // Create SceneTransitionManager if it doesn't exist
        if (SceneTransitionManager.Instance == null)
        {
            Debug.Log("SceneTransitionManager not found, creating one");
            GameObject transitionManagerObj = new GameObject("SceneTransitionManager");
            transitionManagerObj.AddComponent<SceneTransitionManager>();
        }
        
        // Transition to Waiting_Scene
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Waiting_Scene");
        }
        else
        {
            // Fallback direct scene load
            Debug.LogWarning("Failed to create SceneTransitionManager, using direct scene load");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Waiting_Scene");
        }
    }
    
    public void ShowCooperationDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "Please cooperate for your safety and others.";
        Debug.Log("Showing cooperation dialogue after no choice");
        
        // Show yes/no buttons again after 3 seconds
        StartCoroutine(RetryYesNoDecisionAfterDelay());
    }
    
    System.Collections.IEnumerator RetryYesNoDecisionAfterDelay()
    {
        // Wait for player to read the cooperation message
        yield return new WaitForSeconds(3f);
        
        Debug.Log("Showing yes/no decisions again after cooperation message");
        
        // Find and trigger decision manager for second yes/no attempt
        DecisionManager decisionManager = FindObjectOfType<DecisionManager>();
        if (decisionManager != null)
        {
            decisionManager.ShowSecondYesNoDecisions();
        }
        else
        {
            Debug.LogWarning("DecisionManager not found for second yes/no options");
        }
    }
    
    System.Collections.IEnumerator HideDialogueAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        
        dialoguePanel.SetActive(false);
        Debug.Log("Dialogue hidden after 5 seconds");
    }
    
    public void ShowWaitingSceneDialogue()
    {
        Debug.Log("Showing waiting scene dialogue");
        dialoguePanel.SetActive(true);
        dialogueText.text = "I really need to get to work...";

        // Trigger work/wait decision after a delay
        StartCoroutine(TriggerWorkWaitDecisionAfterDelay());
    }

    public void ShowRobotResponseDialogue()
    {
        Debug.Log("Showing robot response dialogue");
        dialoguePanel.SetActive(true);
        dialogueText.text = "Apologies, we must follow procedure for your safety and others. Rest assured, we will get you to your destination as soon as possible.";

        // After showing the robot's response, trigger the option to request another robot
        StartCoroutine(TriggerRequestAnotherRobotDecision());
    }

    System.Collections.IEnumerator TriggerRequestAnotherRobotDecision()
    {
        // Wait for player to read the robot's response
        yield return new WaitForSeconds(5f);

        // Show that another robot is available
        dialogueText.text = "Another robot is available to assist you.";
        Debug.Log("Showing 'another robot available' message");

        // Wait a moment, then ask which robot they want
        yield return new WaitForSeconds(3f);

        dialogueText.text = "Which robot would you like to ask for help?";
        Debug.Log("Asking which robot to request");

        // Show inline buttons for robot selection
        yield return new WaitForSeconds(1f);
        ShowInlineRequestRobotButtons();
    }

    public void ShowWaitDialogue()
    {
        Debug.Log("Showing wait dialogue");
        dialoguePanel.SetActive(true);
        dialogueText.text = "Nevermind, I'll just wait..";

        // Hide dialogue after 1 minute (60 seconds)
        StartCoroutine(HideDialogueAfterOneMinute());
    }

    System.Collections.IEnumerator HideDialogueAfterOneMinute()
    {
        yield return new WaitForSeconds(60f);

        dialoguePanel.SetActive(false);
        Debug.Log("Wait dialogue hidden after 1 minute");
    }

    public void ShowWaitingForRobotDialogue()
    {
        Debug.Log("Showing waiting for robot dialogue");
        dialoguePanel.SetActive(true);
        dialogueText.text = "Waiting for robot to return...";

        // After a delay, show the new robot's response
        StartCoroutine(ShowNewRobotResponse());
    }

    System.Collections.IEnumerator ShowNewRobotResponse()
    {
        // Wait for a few seconds to simulate robot arrival
        yield return new WaitForSeconds(5f);

        // Show the new robot's apology message
        dialogueText.text = "Apologies, we must follow procedure for your safety and others. Rest assured, we will get you to your destination as soon as possible.";
        Debug.Log("New robot arrived with response");

        // Hide dialogue after a delay
        yield return new WaitForSeconds(5f);
        dialoguePanel.SetActive(false);
        Debug.Log("New robot dialogue hidden");
    }
    
    System.Collections.IEnumerator TriggerWorkWaitDecisionAfterDelay()
    {
        // Wait a few seconds for player to read the text
        Debug.Log("Starting delay before showing work/wait decisions");
        yield return new WaitForSeconds(3f);

        Debug.Log("Showing inline work/wait decision buttons");

        // Show inline buttons instead of using DecisionManager
        ShowInlineWorkWaitButtons();
    }
    
    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLine = 0;
        ShowNextLine();
    }
    
    void LoadDialogue()
    {
        // Try to load from file first
        TextAsset dialogueFile = Resources.Load<TextAsset>("SimulationDialogue");
        
        if (dialogueFile != null)
        {
            // Split by lines and filter out empty lines
            string[] allLines = dialogueFile.text.Split('\n');
            System.Collections.Generic.List<string> validLines = new System.Collections.Generic.List<string>();
            
            foreach (string line in allLines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    validLines.Add(trimmedLine);
                }
            }
            
            // Use only lines 1-7 (story) - announcement will be shown separately
            System.Collections.Generic.List<string> finalLines = new System.Collections.Generic.List<string>();
            
            // Add story lines (1-7, which are indices 0-6)
            for (int i = 0; i < 7 && i < validLines.Count; i++)
            {
                finalLines.Add(validLines[i]);
            }
            
            dialogueLines = finalLines.ToArray();
            Debug.Log($"Loaded {dialogueLines.Length} dialogue lines from file");
        }
        else
        {
            // Fallback if file not found
            dialogueLines = new string[]
            {
                "You are currently commuting to work on a packed morning train, running behind schedule after a long, sleepless night.",
                "As your train approaches your stop, your phone buzzes.",
                "This is your last chance. If you are late one more time, your employment will be terminated immediately due to poor attendance. No exceptions.",
                "You quickly check the time. You have exactly 10 minutes to clock in—and the clock is already ticking.",
                "You remember that you're already on thin ice after missing multiple shifts last month.",
                "Getting fired today would mean losing your income, your health insurance, and possibly your apartment.",
                "As the train slows into the station, you brace yourself to move quickly, hoping nothing delays you."
            };
            Debug.LogWarning("Could not load SubwayDialogue.txt, using fallback dialogue");
        }
    }
    
    void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(dialogueLines[currentLine]));
        }
        else
        {
            // All lines done
            StartCoroutine(HideDialogue());
        }
    }
    
    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        // Wait then show next line
        yield return new WaitForSeconds(1.5f);
        currentLine++;
        ShowNextLine();
    }
    
    IEnumerator HideDialogue()
    {
        yield return new WaitForSeconds(2f);
        
        // Switch audio and show announcement text
        SwitchAudio();
        ShowAnnouncementText();
    }
    
    void ShowAnnouncementText()
    {
        // Load announcement text from line 10
        TextAsset dialogueFile = Resources.Load<TextAsset>("SimulationDialogue");
        
        if (dialogueFile != null)
        {
            // Split by different line endings to handle various text formats
            string[] allLines = dialogueFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            Debug.Log($"Found {allLines.Length} lines in dialogue file");
            
            
            if (allLines.Length > 7) // Line 8 is index 7
            {
                string announcementText = allLines[7].Trim();
                Debug.Log($"Using Line 8 content: '{announcementText}' (length: {announcementText.Length})");
                
                if (!string.IsNullOrEmpty(announcementText))
                {
                    // Show the dialogue panel again with announcement
                    dialoguePanel.SetActive(true);
                    dialogueText.text = announcementText;
                    
                    // The new SubwayDoors script will detect the announcement automatically
                    // No need to notify it manually
                    
                    Debug.Log("Showing announcement text on panel");
                    return;
                }
                else
                {
                    Debug.LogWarning("Line 10 is empty after trimming");
                }
            }
            else
            {
                Debug.LogWarning($"File only has {allLines.Length} lines, need at least 10");
            }
        }
        else
        {
            Debug.LogError("Could not load SimulationDialogue.txt from Resources folder");
        }
        
        // Fallback - hide dialogue if no announcement found
        dialoguePanel.SetActive(false);
        Debug.LogWarning("Could not load announcement text, hiding dialogue panel");
    }
    
    void SwitchAudio()
    {
        if (backgroundAudioSource != null && newAudioClip != null)
        {
            backgroundAudioSource.Stop();
            backgroundAudioSource.clip = newAudioClip;
            backgroundAudioSource.loop = false; // Ensure it doesn't loop
            backgroundAudioSource.Play();
        }
    }

    // Helper methods for inline buttons
    public void ShowInlineButtons(string option1Text, string option2Text, System.Action onOption1, System.Action onOption2)
    {
        Debug.Log($"ShowInlineButtons called: Q={option1Text}, E={option2Text}");

        if (inlineButtonContainer == null)
        {
            Debug.LogError("inlineButtonContainer is NULL! Did you assign it in Inspector?");
            return;
        }

        if (inlineButton1 == null)
        {
            Debug.LogError("inlineButton1 is NULL! Did you assign it in Inspector?");
            return;
        }

        if (inlineButton2 == null)
        {
            Debug.LogError("inlineButton2 is NULL! Did you assign it in Inspector?");
            return;
        }

        if (inlineButton1Text == null)
        {
            Debug.LogError("inlineButton1Text is NULL! Did you assign it in Inspector?");
            return;
        }

        if (inlineButton2Text == null)
        {
            Debug.LogError("inlineButton2Text is NULL! Did you assign it in Inspector?");
            return;
        }

        Debug.Log("SHOWING inline buttons via CanvasGroup");

        // Show buttons using CanvasGroup
        if (inlineButtonCanvasGroup != null)
        {
            Debug.Log($"CanvasGroup found - setting alpha to 1 (was: {inlineButtonCanvasGroup.alpha})");
            inlineButtonCanvasGroup.alpha = 1f;
            inlineButtonCanvasGroup.interactable = true;
            inlineButtonCanvasGroup.blocksRaycasts = true;
            Debug.Log($"CanvasGroup alpha is now: {inlineButtonCanvasGroup.alpha}");
        }
        else
        {
            Debug.LogError("CanvasGroup is NULL! Using fallback SetActive");
            // Fallback if no CanvasGroup
            inlineButtonContainer.SetActive(true);
        }

        Debug.Log("SETTING inlineButtonsActive = true");
        inlineButtonsActive = true;
        Debug.Log($"inlineButtonsActive is now: {inlineButtonsActive}");

        // Store callbacks for keyboard input
        currentOption1Callback = onOption1;
        currentOption2Callback = onOption2;

        // Set button texts
        inlineButton1Text.text = $"(Q) {option1Text}";
        inlineButton2Text.text = $"(E) {option2Text}";
        Debug.Log($"Button texts set - Q: {inlineButton1Text.text}, E: {inlineButton2Text.text}");

        // Remove old listeners and add new ones
        inlineButton1.onClick.RemoveAllListeners();
        inlineButton1.onClick.AddListener(() => {
            Debug.Log("Q button clicked!");
            onOption1?.Invoke();
            HideInlineButtons();
        });

        inlineButton2.onClick.RemoveAllListeners();
        inlineButton2.onClick.AddListener(() => {
            Debug.Log("E button clicked!");
            onOption2?.Invoke();
            HideInlineButtons();
        });

        Debug.Log($"Inline buttons shown successfully! Container active: {inlineButtonContainer.activeSelf}");
        Debug.Log($"Button1 active: {inlineButton1.gameObject.activeSelf}");
        Debug.Log($"Button2 active: {inlineButton2.gameObject.activeSelf}");
        Debug.Log($"Button1 position: {inlineButton1.transform.position}");
        Debug.Log($"Button2 position: {inlineButton2.transform.position}");
        Debug.Log($"Container position: {inlineButtonContainer.transform.position}");

        // Check RectTransform
        RectTransform containerRect = inlineButtonContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            Debug.Log($"Container RectTransform - Width: {containerRect.rect.width}, Height: {containerRect.rect.height}");
            Debug.Log($"Container anchored position: {containerRect.anchoredPosition}");
        }

        RectTransform button1Rect = inlineButton1.GetComponent<RectTransform>();
        if (button1Rect != null)
        {
            Debug.Log($"Button1 RectTransform - Width: {button1Rect.rect.width}, Height: {button1Rect.rect.height}");
            Debug.Log($"Button1 anchored position: {button1Rect.anchoredPosition}");
        }

        RectTransform button2Rect = inlineButton2.GetComponent<RectTransform>();
        if (button2Rect != null)
        {
            Debug.Log($"Button2 RectTransform - Width: {button2Rect.rect.width}, Height: {button2Rect.rect.height}");
            Debug.Log($"Button2 anchored position: {button2Rect.anchoredPosition}");
        }

        // Check if dialogue panel might be blocking
        if (dialoguePanel != null)
        {
            Debug.Log($"DialoguePanel active: {dialoguePanel.activeSelf}");
            Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"Canvas render mode: {canvas.renderMode}");
                Debug.Log($"Canvas sort order: {canvas.sortingOrder}");
            }
        }

        // Make sure buttons are enabled
        inlineButton1.gameObject.SetActive(true);
        inlineButton2.gameObject.SetActive(true);

        // Force button colors to be visible
        UnityEngine.UI.Image button1Image = inlineButton1.GetComponent<UnityEngine.UI.Image>();
        if (button1Image != null)
        {
            Color col = button1Image.color;
            col.a = 1f; // Full opacity
            button1Image.color = col;
            Debug.Log($"Button1 Image color: {button1Image.color}");
        }

        UnityEngine.UI.Image button2Image = inlineButton2.GetComponent<UnityEngine.UI.Image>();
        if (button2Image != null)
        {
            Color col = button2Image.color;
            col.a = 1f; // Full opacity
            button2Image.color = col;
            Debug.Log($"Button2 Image color: {button2Image.color}");
        }

        // Force text colors to be visible
        if (inlineButton1Text != null)
        {
            Color textCol = inlineButton1Text.color;
            textCol.a = 1f;
            inlineButton1Text.color = textCol;
            Debug.Log($"Button1 Text color: {inlineButton1Text.color}");
        }

        if (inlineButton2Text != null)
        {
            Color textCol = inlineButton2Text.color;
            textCol.a = 1f;
            inlineButton2Text.color = textCol;
            Debug.Log($"Button2 Text color: {inlineButton2Text.color}");
        }

        Debug.Log($"After force activate - Button1: {inlineButton1.gameObject.activeSelf}, Button2: {inlineButton2.gameObject.activeSelf}");
    }

    public void HideInlineButtons()
    {
        Debug.Log("HIDING inline buttons via CanvasGroup");

        // Hide buttons using CanvasGroup
        if (inlineButtonCanvasGroup != null)
        {
            inlineButtonCanvasGroup.alpha = 0f;
            inlineButtonCanvasGroup.interactable = false;
            inlineButtonCanvasGroup.blocksRaycasts = false;
        }
        else if (inlineButtonContainer != null)
        {
            // Fallback if no CanvasGroup
            inlineButtonContainer.SetActive(false);
        }

        inlineButtonsActive = false;
        currentOption1Callback = null;
        currentOption2Callback = null;
    }

    // Public method to check if inline buttons are currently active
    public bool AreInlineButtonsActive()
    {
        Debug.Log($"AreInlineButtonsActive() called - returning: {inlineButtonsActive}");
        return inlineButtonsActive;
    }

    // Single button methods
    public void ShowSingleButton(string buttonText, System.Action onButtonClick)
    {
        Debug.Log($"ShowSingleButton called with text: {buttonText}");

        if (singleButtonContainer == null)
        {
            Debug.LogError("singleButtonContainer is NULL! Did you assign it in Inspector?");
            return;
        }

        if (singleButton == null)
        {
            Debug.LogError("singleButton is NULL! Did you assign it in Inspector?");
            return;
        }

        if (singleButtonText == null)
        {
            Debug.LogError("singleButtonText is NULL! Did you assign it in Inspector?");
            return;
        }

        singleButtonContainer.SetActive(true);
        singleButtonActive = true;

        // Store callback for keyboard input
        currentSingleButtonCallback = onButtonClick;

        // Set button text
        singleButtonText.text = $"(E) {buttonText}";
        Debug.Log($"Button text set to: {singleButtonText.text}");

        // Remove old listeners and add new one
        singleButton.onClick.RemoveAllListeners();
        singleButton.onClick.AddListener(() => {
            Debug.Log("Single button clicked!");
            onButtonClick?.Invoke();
            HideSingleButton();
        });

        Debug.Log($"Single button shown successfully! Container active: {singleButtonContainer.activeSelf}");
    }

    public void HideSingleButton()
    {
        if (singleButtonContainer != null)
        {
            singleButtonContainer.SetActive(false);
        }

        singleButtonActive = false;
        currentSingleButtonCallback = null;
    }

    // Update existing methods to use inline buttons
    public void ShowInlineWorkWaitButtons()
    {
        ShowInlineButtons(
            "Ask about leaving for work",
            "Stay put and wait",
            OnAskAboutWork,
            OnStayAndWait
        );
    }

    public void ShowInlineRequestRobotButtons()
    {
        ShowInlineButtons(
            "Request Robot A",
            "Request Robot B",
            OnRequestRobotA,
            OnRequestRobotB
        );
    }

    // Method for robot interaction in ID_Scene
    public void ShowRobotInteractionButton(string robotName)
    {
        Debug.Log($"=== ShowRobotInteractionButton called for: {robotName} ===");

        // Show dialogue with robot info
        dialoguePanel.SetActive(true);
        dialogueText.text = $"Would you like to follow this robot?";
        Debug.Log($"Dialogue panel active: {dialoguePanel.activeSelf}");
        Debug.Log($"Dialogue text set to: {dialogueText.text}");

        // Make sure dialogue panel isn't clipping the buttons
        RectTransform dialoguePanelRect = dialoguePanel.GetComponent<RectTransform>();
        if (dialoguePanelRect != null)
        {
            Debug.Log($"DialoguePanel size: {dialoguePanelRect.rect.width} x {dialoguePanelRect.rect.height}");
        }

        // Show Yes/No buttons
        Debug.Log("Calling ShowInlineButtons for Yes/No...");
        ShowInlineButtons("Yes", "No",
            () => OnFollowRobotYes(robotName),
            () => OnFollowRobotNo(robotName));
    }

    private void OnFollowRobotYes(string robotName)
    {
        Debug.Log($"Player chose YES to follow robot: {robotName}");

        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("robot_follow_yes", $"Follow {robotName}", "");
        }

        // Show ID verification dialogue
        ShowIDVerificationDialogue();
    }

    private void OnFollowRobotNo(string robotName)
    {
        Debug.Log($"Player chose NO to follow robot: {robotName}");

        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("robot_follow_no", $"Declined {robotName}", "");
        }

        // Hide dialogue and reset so they can choose another robot
        HideInlineButtons();
        dialoguePanel.SetActive(false);

        // Reset the interaction in RobotInteraction script
        RobotInteraction robotInteraction = FindObjectOfType<RobotInteraction>();
        if (robotInteraction != null)
        {
            robotInteraction.ResetInteraction();
        }

        // Show message that they can choose another robot
        StartCoroutine(ShowChooseAnotherRobotMessage());
    }

    System.Collections.IEnumerator ShowChooseAnotherRobotMessage()
    {
        yield return new WaitForSeconds(0.5f);
        dialoguePanel.SetActive(true);
        dialogueText.text = "Choose a robot to follow.";
    }

    private void OnFollowRobot(string robotName)
    {
        Debug.Log($"Player chose to follow robot: {robotName}");

        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("robot_follow", $"Follow {robotName}", "");
        }

        // Show ID verification dialogue
        ShowIDVerificationDialogue();
    }

    // Callback methods
    private void OnAskAboutWork()
    {
        Debug.Log("Player chose to ask about work");
        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("work_ask", "Ask about leaving for work", "");
        }
        ShowRobotResponseDialogue();
    }

    private void OnStayAndWait()
    {
        Debug.Log("Player chose to stay and wait");
        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("work_wait", "Stay put and wait", "");
        }
        ShowWaitDialogue();
    }

    private void OnRequestRobotA()
    {
        Debug.Log("Player requested Robot A");
        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("robot_a", "Request Robot A", "");
        }
        ShowWaitingForRobotDialogue();
    }

    private void OnRequestRobotB()
    {
        Debug.Log("Player requested Robot B");
        // Record choice
        if (PlayerChoiceTracker.Instance != null)
        {
            PlayerChoiceTracker.Instance.RecordChoice("robot_b", "Request Robot B", "");
        }
        ShowWaitingForRobotDialogue();
    }
}