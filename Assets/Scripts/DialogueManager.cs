
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMPro.TMP_Text dialogueText;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;

    private int currentLine = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLine = 0;
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
}
