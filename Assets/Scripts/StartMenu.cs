using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject startButton;
    
    public void StartGame()
    {
        // Hide the start button
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
        
        // Unlock camera
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Start dialogue
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue();
        }
        
        Debug.Log("Game started - button hidden, camera unlocked, dialogue started");
    }
}