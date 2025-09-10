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
        
        // Let PlayerController handle cursor state - don't override it here
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        
        // Start the player controller game instead
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.StartGame();
        }
        
        Debug.Log("Game started - button hidden, PlayerController started");
    }
}