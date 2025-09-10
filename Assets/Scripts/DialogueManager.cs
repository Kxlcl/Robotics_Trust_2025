using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMPro.TMP_Text dialogueText;
    public float textSpeed = 0.05f;
    
    [Header("Audio")]
    public AudioSource backgroundAudioSource;
    public AudioClip newAudioClip;
    
    private string[] dialogueLines;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    
    void Start()
    {
        LoadDialogue();
        dialoguePanel.SetActive(false); // Keep hidden until triggered
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