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
        // Simple fallback lines
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
        dialoguePanel.SetActive(false);
        
        // Switch audio when dialogue ends
        SwitchAudio();
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