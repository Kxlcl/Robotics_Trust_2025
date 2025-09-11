using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    
    [Header("Game State")]
    public bool gameStarted = false;
    public bool movementEnabled = false;
    
    [Header("Dialogue")]
    public GameObject dialoguePrefab;
    
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Camera playerCamera;
    private bool wasMouseOutside = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera == null)
        {
            // Try to find main camera in scene
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            if (playerCamera != null)
            {
                Debug.Log($"Found camera: {playerCamera.name} at position: {playerCamera.transform.position}");
            }
            else
            {
                Debug.LogError("No Camera found anywhere in scene!");
            }
        }
        else
        {
            Debug.Log($"Found child camera: {playerCamera.name}");
        }
        
        // Check current scene
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Start_Scene")
        {
            // Force cursor to center of screen for start menu
            StartCoroutine(CenterCursorOnStart());
        }
        else
        {
            // For other scenes (like ID_Scene), start with game already active
            gameStarted = true;
            movementEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"PlayerController initialized in {currentScene} - game controls active");
        }
        
        Debug.Log("PlayerController initialized");
    }
    
    System.Collections.IEnumerator CenterCursorOnStart()
    {
        // Wait one frame for screen to initialize
        yield return null;
        
        // Keep cursor free for menu interaction until game starts
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("PlayerController ready - waiting for game to start");
    }
    
    void Update()
    {
        // Don't process any input until game has started
        if (!gameStarted)
        {
            return;
        }
        
        // Force cursor to stay locked when game is active
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Forcing cursor back to locked state");
        }
        
        // Check if mouse is outside screen bounds
        Vector3 mousePos = Input.mousePosition;
        bool mouseOutside = mousePos.x < 0 || mousePos.x > Screen.width || 
                           mousePos.y < 0 || mousePos.y > Screen.height;
        
        // If mouse just returned from outside, reset to center
        if (wasMouseOutside && !mouseOutside && Cursor.lockState == CursorLockMode.Confined)
        {
            // Reset mouse to center to prevent jumps
            Cursor.lockState = CursorLockMode.Locked;
            // Wait one frame then return to confined mode
            StartCoroutine(ResetToConfinedMode());
        }
        
        wasMouseOutside = mouseOutside;
        
        // Always allow mouse look after game starts
        HandleMouseLook();
        
        // Only allow movement if specifically enabled
        if (movementEnabled)
        {
            HandleMovement();
        }
    }
    
    System.Collections.IEnumerator ResetToConfinedMode()
    {
        yield return null; // Wait one frame
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    void HandleMouseLook()
    {
        if (playerCamera == null) return;
        
        // Only process mouse look when cursor is locked
        if (Cursor.lockState != CursorLockMode.Locked && Cursor.lockState != CursorLockMode.Confined)
        {
            Debug.Log($"Mouse look blocked - Cursor state: {Cursor.lockState}");
            return;
        }
            
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        
        // Prevent extreme jumps when mouse re-enters window
        mouseX = Mathf.Clamp(mouseX, -10f, 10f);
        mouseY = Mathf.Clamp(mouseY, -10f, 10f);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Only rotate the camera, keep it at fixed local position
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerCamera.transform.localPosition = Vector3.zero; // Keep camera at origin of parent
        
        // Rotate the player body left/right
        transform.Rotate(Vector3.up * mouseX);
        
    }
    
    void HandleMovement()
    {
        // Get input using both methods for compatibility
        float moveX = 0f;
        float moveZ = 0f;
        
        // Legacy Input System
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        
        // Also check direct key input as backup
        if (moveX == 0 && moveZ == 0)
        {
            if (Input.GetKey(KeyCode.W)) moveZ = 1f;
            if (Input.GetKey(KeyCode.S)) moveZ = -1f;
            if (Input.GetKey(KeyCode.A)) moveX = -1f;
            if (Input.GetKey(KeyCode.D)) moveX = 1f;
        }
        
        
        // Calculate movement
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed;
        
        // Apply gravity
        if (controller.isGrounded)
        {
            velocity.y = 0f;
        }
        velocity.y += gravity * Time.deltaTime;
        
        // Combine horizontal movement with vertical velocity
        move.y = velocity.y;
        
        // Move the character
        Vector3 finalMovement = move * Time.deltaTime;
        controller.Move(finalMovement);
        
    }
    
    // Call this from UI buttons to enter FPS mode
    public void EnableFPSMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // Call this from UI buttons to enter UI mode
    public void EnableUIMode()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    
    // Call this from the Start button to begin the game
    public void StartGame()
    {
        gameStarted = true;
        movementEnabled = true; // Enable movement immediately
        
        // Force cursor to confined mode for free roam
        StartCoroutine(EnableConfinedMode());
        
        // Trigger dialogue system
        TriggerStartDialogue();
        
        Debug.Log("Game started - Movement and camera enabled, confined mode active");
    }
    
    System.Collections.IEnumerator EnableConfinedMode()
    {
        // Wait a frame to ensure UI is done
        yield return null;
        
        // Force locked mode for reliable mouse look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Keep in locked mode for reliable mouse look
        // yield return null;
        // Cursor.lockState = CursorLockMode.Confined;
        
        Debug.Log($"Cursor state set to: {Cursor.lockState}, Visible: {Cursor.visible}");
    }
    
    private void TriggerStartDialogue()
    {
        // Find and start the dialogue manager
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue();
            Debug.Log("DialogueManager found and started");
        }
        else
        {
            Debug.LogWarning("DialogueManager not found in scene");
        }
    }
    
    // Call this to enable WASD movement
    public void EnableMovement()
    {
        movementEnabled = true;
        Debug.Log("Movement enabled");
    }
    
    // Call this to disable WASD movement
    public void DisableMovement()
    {
        movementEnabled = false;
        Debug.Log("Movement disabled");
    }
    
    
}