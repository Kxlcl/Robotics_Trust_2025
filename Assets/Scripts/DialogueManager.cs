using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    // ...existing code...
    public Button choiceButton1;
    public Button choiceButton2;
    public int linesBeforeChoices = 3; // Show buttons after 3 lines
    public GameObject dialoguePanel;
    public TMPro.TMP_Text dialogueText;
    public TextAsset dialogueScript;
    public float textSpeed = 0.05f;

    private string[] dialogueLines;
    private int currentLine = 0;
    private Coroutine typingCoroutine;

    void Awake()
    {
        // Make the dialogue manager and panel persistent
        DontDestroyOnLoad(gameObject); // DialogueManager
        if (dialoguePanel != null)
        {
            DontDestroyOnLoad(dialoguePanel); // Dialogue box
        }
        // Also make timer persistent if found
        var timerObj = GameObject.Find("Timer");
        if (timerObj != null)
        {
            DontDestroyOnLoad(timerObj);
        }
    }

    void Start()
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        Application.targetFrameRate = 30;
        if (dialogueScript != null && (dialogueLines == null || dialogueLines.Length == 0))
        {
            dialogueLines = dialogueScript.text.Split('\n');
        }
        else if (dialogueLines == null || dialogueLines.Length == 0)
        {
            dialogueLines = new string[] { "No dialogue script assigned." };
        }
        StartDialogue();
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        // Do NOT reset currentLine here; only set active and show current line
        // currentLine = 0; // Commenting this line out to prevent resetting
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        ShowLine();
    }

    public void NextLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogueLines[currentLine];
            typingCoroutine = null;
            return;
        }
        currentLine++;
        if (currentLine == linesBeforeChoices)
        {
            if (choiceButton1 != null) choiceButton1.gameObject.SetActive(true);
            if (choiceButton2 != null) choiceButton2.gameObject.SetActive(true);
        }
        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowLine()
    {
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        typingCoroutine = null;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // Button handler for both choices
    public void OnChoiceButtonClicked()
    {
        SceneManager.LoadScene("station");
    }
}
