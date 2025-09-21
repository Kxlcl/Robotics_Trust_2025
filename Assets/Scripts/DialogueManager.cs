using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMPro.TMP_Text dialogueText;
    public float textSpeed = 0.05f;
    
    [Header("Player Repositioning")]
    public Vector3 newPlayerPosition = new Vector3(170f, 0f, -95f);
    
    [Header("Audio")]
    public AudioSource backgroundAudioSource;
    public AudioClip newAudioClip;
    
    private string[] dialogueLines;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    
    void Start()
    {
        LoadDialogue();
        // Canvas stays active - dialogue panel will just show empty text if no content
        
        // Check if we're in ID_Scene and show appropriate dialogue
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "ID_Scene")
        {
            ShowIDSceneDialogue();
        }
        else
        {
            // In other scenes (like Waiting_Scene), keep dialogue completely hidden
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            // Disable this component in non-ID scenes
            this.enabled = false;
            Debug.Log($"DialogueManager disabled in {currentScene}");
        }
    }
    
    void ShowIDSceneDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "Choose a robot to follow.";
        Debug.Log("Showing ID_Scene dialogue: Choose a robot to follow");
        
        // Trigger decision system after a delay
        StartCoroutine(TriggerDecisionAfterDelay());
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
}