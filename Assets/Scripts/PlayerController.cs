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
    
    [Header("ID Scene Movement Lock")]
    public bool movementLocked = false;
    public bool allowUIInteraction = false;
    
    [Header("Waiting Scene Settings")]
    public bool isInWaitingScene = false;
    public Vector3 waitingPosition = new Vector3(134f, 0f, -101f);
    
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
        Debug.Log($"PlayerController Start() - Current scene: '{currentScene}'");
        
        if (currentScene == "Start_Scene")
        {
            // In Start_Scene, allow looking but no movement initially
            StartCoroutine(CenterCursorOnStart());
        }
        else if (currentScene == "Waiting_Scene" || currentScene.ToLower().Contains("waiting"))
        {
            // In Waiting_Scene, teleport player and keep them locked in position
            gameStarted = true;
            movementEnabled = false; // Disable movement in waiting scene
            movementLocked = true; // Lock movement completely
            isInWaitingScene = true; // Mark as being in waiting scene
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // Teleport player to specified coordinates with delay
            StartCoroutine(DelayedTeleportToWaitingPosition());
            
            Debug.Log($"PlayerController initialized in {currentScene} - will teleport to waiting position, movement locked");
        }
        else
        {
            // For other scenes (like ID_Scene), start with game already active
            gameStarted = true;
            movementEnabled = true; // Enable movement immediately in ID_Scene
            movementLocked = false; // Reset movement lock
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"PlayerController initialized in {currentScene} - full game controls active");
        }
        
        Debug.Log("PlayerController initialized");
    }
    
    System.Collections.IEnumerator CenterCursorOnStart()
    {
        // Wait one frame for screen to initialize
        yield return null;
        
        // Keep cursor free for menu interaction until game starts
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log("PlayerController ready - waiting for game to start");
    }
    
    void Update()
    {
        // Don't process any input until game has started
        if (!gameStarted)
        {
            return;
        }
        
        // Force cursor to stay locked when game is active (unless UI interaction is allowed)
        if (gameStarted && !allowUIInteraction && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"Forcing cursor lock - was {Cursor.lockState}");
        }
        
        // Always allow mouse look after game starts
        HandleMouseLook();
        
        // Always apply gravity to keep player grounded
        ApplyGravity();
        
        // Only allow movement if specifically enabled and not locked
        if (movementEnabled && !movementLocked)
        {
            HandleMovement();
        }
        
        // In waiting scene, ensure player stays at the waiting position
        if (isInWaitingScene)
        {
            EnforceWaitingPosition();
            
            // Additional aggressive positioning for first few seconds
            ForceTeleportToWaitingPosition();
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
        
        // Only process mouse look when cursor is locked (or when UI interaction is allowed but not over UI)
        if (Cursor.lockState != CursorLockMode.Locked && !allowUIInteraction)
        {
            return;
        }
        
        // If UI interaction is allowed, check if mouse is over UI
        if (allowUIInteraction && UnityEngine.EventSystems.EventSystem.current != null)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // Mouse is over UI, don't do mouse look
                return;
            }
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
    
    void ApplyGravity()
    {
        // Only apply gravity if controller exists and is enabled
        if (controller == null || !controller.enabled) return;
        
        // Apply gravity
        if (controller.isGrounded)
        {
            velocity.y = 0f;
        }
        velocity.y += gravity * Time.deltaTime;
        
        // Apply gravity movement
        Vector3 gravityMovement = new Vector3(0, velocity.y, 0);
        controller.Move(gravityMovement * Time.deltaTime);
    }
    
    void HandleMovement()
    {
        // Only handle movement if controller exists and is enabled
        if (controller == null || !controller.enabled) return;
        
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
        
        // Calculate horizontal movement only
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed;
        
        // Move the character (horizontal only, gravity handled separately)
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
        movementEnabled = false; // Keep movement disabled in Start_Scene
        
        // Force cursor to locked mode for mouse look only
        StartCoroutine(EnableConfinedMode());
        
        // Trigger dialogue system
        TriggerStartDialogue();
        
        Debug.Log("Game started - Camera enabled, movement disabled for Start_Scene");
    }
    
    System.Collections.IEnumerator EnableConfinedMode()
    {
        // Wait a frame to ensure UI is done
        yield return null;
        
        // Force locked mode for reliable mouse look - keeps cursor in center
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Wait another frame and force lock again if needed
        yield return null;
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Had to force cursor lock again");
        }
        
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
    
    // Methods for UI interaction control
    public void EnableUIInteraction()
    {
        allowUIInteraction = true;
        Debug.Log("UI interaction enabled - cursor can interact with UI");
    }
    
    public void DisableUIInteraction()
    {
        allowUIInteraction = false;
        Debug.Log("UI interaction disabled - cursor locked for mouse look");
    }
    
    // Collision detection for Part111
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Part111")
        {
            movementLocked = true;
            Debug.Log("Player touched Part111 - movement locked");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Part111")
        {
            movementLocked = true;
            Debug.Log("Player collided with Part111 - movement locked");
        }
    }
    
    private System.Collections.IEnumerator DelayedTeleportToWaitingPosition()
    {
        // Wait a few frames for scene to fully load
        yield return null;
        yield return null;
        yield return null;
        
        // First teleportation attempt
        TeleportToWaitingPosition();
        
        // Wait another frame and try again to ensure it sticks
        yield return null;
        TeleportToWaitingPosition();
        
        // One more attempt after a short delay
        yield return new WaitForSeconds(0.1f);
        TeleportToWaitingPosition();
        
        Debug.Log("Completed multiple teleportation attempts");
    }
    
    private void TeleportToWaitingPosition()
    {
        // Keep the same Y coordinate from ID scene
        waitingPosition = new Vector3(134f, transform.position.y, -101f);
        
        Debug.Log($"=== TELEPORTATION ATTEMPT ===");
        Debug.Log($"GameObject name: {gameObject.name}");
        Debug.Log($"Transform name: {transform.name}");
        Debug.Log($"Parent: {(transform.parent != null ? transform.parent.name : "None")}");
        Debug.Log($"Children count: {transform.childCount}");
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Debug.Log($"Child {i}: {transform.GetChild(i).name}");
            }
        }
        Debug.Log($"Controller exists: {controller != null}");
        Debug.Log($"Controller enabled: {(controller != null ? controller.enabled.ToString() : "N/A")}");
        Debug.Log($"Before teleport position: {transform.position}");
        Debug.Log($"Target waiting position: {waitingPosition}");
        
        // Find the firstperson GameObject specifically
        GameObject firstPersonObject = GameObject.Find("firstperson");
        if (firstPersonObject == null)
        {
            // Try alternative names
            firstPersonObject = GameObject.Find("FirstPerson");
            if (firstPersonObject == null)
            {
                firstPersonObject = GameObject.Find("First Person");
                if (firstPersonObject == null)
                {
                    // Search through all objects for one containing "firstperson" in name
                    GameObject[] allObjects = FindObjectsOfType<GameObject>();
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.name.ToLower().Contains("firstperson") || obj.name.ToLower().Contains("first person"))
                        {
                            firstPersonObject = obj;
                            break;
                        }
                    }
                }
            }
        }
        
        Debug.Log($"FirstPerson object found: {(firstPersonObject != null ? firstPersonObject.name : "NOT FOUND")}");
        if (firstPersonObject != null)
        {
            Debug.Log($"FirstPerson position before teleport: {firstPersonObject.transform.position}");
        }
        
        // Also check for other player-related objects
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Found {allPlayers.Length} objects with Player tag");
        foreach (GameObject player in allPlayers)
        {
            Debug.Log($"Player object: {player.name} at position {player.transform.position}");
        }
        
        // PRIORITY: Move the firstperson object if found
        if (firstPersonObject != null)
        {
            Debug.Log("PRIORITY TELEPORT: Moving firstperson object...");
            
            // Check if firstperson has a CharacterController
            CharacterController firstPersonController = firstPersonObject.GetComponent<CharacterController>();
            if (firstPersonController != null)
            {
                Debug.Log("FirstPerson has CharacterController - disabling for teleport");
                firstPersonController.enabled = false;
                firstPersonObject.transform.position = waitingPosition;
                firstPersonController.enabled = true;
            }
            else
            {
                Debug.Log("FirstPerson direct teleport");
                firstPersonObject.transform.position = waitingPosition;
            }
            
            Debug.Log($"FirstPerson teleported to: {firstPersonObject.transform.position}");
        }
        
        // Also try the regular teleportation methods as backup
        if (controller != null)
        {
            Debug.Log("BACKUP: Attempting teleportation with CharacterController method...");
            controller.enabled = false;
            transform.position = waitingPosition;
            
            // Also try to move any parent or root object
            if (transform.root != transform)
            {
                Debug.Log($"Also moving root object: {transform.root.name}");
                transform.root.position = waitingPosition;
            }
            
            // Wait a bit before re-enabling
            StartCoroutine(ReEnableController());
        }
        else
        {
            Debug.Log("BACKUP: Attempting direct transform teleportation...");
            transform.position = waitingPosition;
            
            // Also try to move any parent or root object
            if (transform.root != transform)
            {
                Debug.Log($"Also moving root object: {transform.root.name}");
                transform.root.position = waitingPosition;
            }
        }
        
        Debug.Log($"After teleport attempt position: {transform.position}");
        
        // Also try teleporting all Player tagged objects
        foreach (GameObject player in allPlayers)
        {
            if (player != gameObject) // Don't double-teleport this object
            {
                Debug.Log($"Also teleporting player object: {player.name}");
                CharacterController playerController = player.GetComponent<CharacterController>();
                if (playerController != null)
                {
                    playerController.enabled = false;
                    player.transform.position = waitingPosition;
                    playerController.enabled = true;
                }
                else
                {
                    player.transform.position = waitingPosition;
                }
                Debug.Log($"Player {player.name} moved to: {player.transform.position}");
            }
        }
        
        // Force a physics update to ensure proper positioning
        Physics.SyncTransforms();
        
        Debug.Log($"After physics sync position: {transform.position}");
        Debug.Log($"=== END TELEPORTATION ATTEMPT ===");
    }
    
    private System.Collections.IEnumerator ReEnableController()
    {
        yield return new WaitForFixedUpdate();
        if (controller != null)
        {
            controller.enabled = true;
            Debug.Log($"Controller re-enabled. Final position: {transform.position}");
        }
    }
    
    private void ForceTeleportToWaitingPosition()
    {
        // Find the firstperson object for force teleportation
        GameObject firstPersonObject = GameObject.Find("firstperson");
        if (firstPersonObject == null)
        {
            firstPersonObject = GameObject.Find("FirstPerson");
            if (firstPersonObject == null)
            {
                // Search for any object containing "firstperson" in name
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("firstperson"))
                    {
                        firstPersonObject = obj;
                        break;
                    }
                }
            }
        }
        
        Vector3 targetPos = new Vector3(134f, transform.position.y, -101f);
        
        // PRIORITY: Force teleport the firstperson object
        if (firstPersonObject != null)
        {
            Vector3 firstPersonCurrentPos = firstPersonObject.transform.position;
            if (Vector3.Distance(firstPersonCurrentPos, targetPos) > 0.1f)
            {
                CharacterController firstPersonController = firstPersonObject.GetComponent<CharacterController>();
                if (firstPersonController != null)
                {
                    firstPersonController.enabled = false;
                    firstPersonObject.transform.position = targetPos;
                    firstPersonController.enabled = true;
                }
                else
                {
                    firstPersonObject.transform.position = targetPos;
                }
                
                Debug.Log($"FORCE TELEPORT FIRSTPERSON: Moving from {firstPersonCurrentPos} to {targetPos}");
            }
        }
        
        // Backup: Also try this object
        Vector3 currentPos = transform.position;
        if (Vector3.Distance(currentPos, targetPos) > 0.1f)
        {
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = targetPos;
                controller.enabled = true;
            }
            else
            {
                transform.position = targetPos;
            }
            
            Debug.Log($"FORCE TELEPORT BACKUP: Moving from {currentPos} to {targetPos}");
        }
    }
    
    private void EnforceWaitingPosition()
    {
        // Check if player has moved too far from waiting position (allowing small Y variance for ground contact)
        Vector3 currentPos = transform.position;
        float horizontalDistance = Vector2.Distance(
            new Vector2(currentPos.x, currentPos.z), 
            new Vector2(waitingPosition.x, waitingPosition.z)
        );
        
        // If player has moved more than 0.5 units horizontally from waiting position, teleport back
        if (horizontalDistance > 0.5f)
        {
            Vector3 correctedPosition = new Vector3(waitingPosition.x, currentPos.y, waitingPosition.z);
            
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = correctedPosition;
                controller.enabled = true;
            }
            else
            {
                transform.position = correctedPosition;
            }
            
            Debug.Log($"Player position corrected back to waiting area: {correctedPosition}");
        }
    }
}