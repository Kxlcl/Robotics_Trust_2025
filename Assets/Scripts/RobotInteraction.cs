using UnityEngine;
using UnityEngine.UI;

public class RobotInteraction : MonoBehaviour
{
    [Header("Camera (Assign Manually if Auto-Find Fails)")]
    [SerializeField] private Camera manualCamera;

    [Header("Raycast Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask robotLayer;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMPro.TMP_Text promptText;

    [Header("Managers")]
    [SerializeField] private DecisionManager decisionManager;
    [SerializeField] private DialogueManager dialogueManager;

    private Camera playerCamera;
    private GameObject currentRobotInView;
    private bool hasInteracted = false;
    private bool cameraSearchComplete = false;

    void Start()
    {
        // First, check if camera was manually assigned in Inspector
        if (manualCamera != null)
        {
            playerCamera = manualCamera;
            cameraSearchComplete = true;
            Debug.Log("Using manually assigned camera: " + playerCamera.name);
            // Don't return - continue with other setup
        }
        else
        {
            // Try to find camera on Start, but don't give up if it fails
            FindCamera();
        }


        // Find DecisionManager if not assigned
        if (decisionManager == null)
        {
            decisionManager = FindObjectOfType<DecisionManager>();
        }

        // Find DialogueManager if not assigned
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Debug logging
        Debug.Log("=== RobotInteraction Debug Info ===");
        Debug.Log($"RobotInteraction initialized in scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"Interaction Prompt assigned: {interactionPrompt != null}");
        Debug.Log($"Prompt Text assigned: {promptText != null}");
        Debug.Log($"Decision Manager found: {decisionManager != null}");
        Debug.Log($"Dialogue Manager found: {dialogueManager != null}");
        Debug.Log($"Interaction Range: {interactionRange}");

        // Find and log all robots in scene
        GameObject[] allRobots = GameObject.FindGameObjectsWithTag("Robot");
        Debug.Log($"Found {allRobots.Length} objects with 'Robot' tag");
        foreach (GameObject robot in allRobots)
        {
            Debug.Log($"  - Robot: {robot.name}");
        }

        // Also check for robot components
        SimpleRobotMovement[] simpleRobots = FindObjectsOfType<SimpleRobotMovement>();
        RobotNavMeshMovement[] navRobots = FindObjectsOfType<RobotNavMeshMovement>();
        Debug.Log($"Found {simpleRobots.Length} SimpleRobotMovement components");
        Debug.Log($"Found {navRobots.Length} RobotNavMeshMovement components");

        if (playerCamera == null)
        {
            Debug.Log("Camera not found on Start - will search in Update loop");
        }
        else
        {
            Debug.Log($"Camera found on Start: {playerCamera.name}");
        }
        Debug.Log("===================================");
    }

    void Update()
    {
        // If camera hasn't been found yet, keep trying
        if (playerCamera == null && !cameraSearchComplete)
        {
            FindCamera();
            return; // Skip robot checking until camera is found
        }

        // Always check for robots (even after interaction, to hide button when looking away)
        if (playerCamera != null)
        {
            CheckForRobot();

            // Check for E key press when looking at a robot
            // ONLY allow interaction if:
            // 1. Not already interacted with this robot
            // 2. Currently looking at a robot
            // 3. Dialogue manager buttons are not active
            if (!hasInteracted && currentRobotInView != null && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"E pressed while looking at {currentRobotInView.name}");

                // Make sure dialogue buttons aren't already showing
                bool buttonsActive = dialogueManager != null && dialogueManager.AreInlineButtonsActive();
                Debug.Log($"Buttons active check: {buttonsActive}");

                if (dialogueManager != null && !buttonsActive)
                {
                    Debug.Log("Triggering InteractWithRobot()");
                    InteractWithRobot();
                }
                else
                {
                    Debug.Log("Interaction blocked - buttons already active or no dialogue manager");
                }
            }
        }
    }

    void FindCamera()
    {
        // Try multiple methods to find the camera

        // Method 1: Camera.main (fastest if camera is tagged MainCamera)
        playerCamera = Camera.main;
        if (playerCamera != null)
        {
            Debug.Log($"Found camera via Camera.main: {playerCamera.name}");
            cameraSearchComplete = true;
            return;
        }

        // Method 2: Search PlayerController for child camera
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerCamera = player.GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                Debug.Log($"Found camera as child of PlayerController: {playerCamera.name}");
                cameraSearchComplete = true;
                return;
            }
        }

        // Method 3: Search FirstPersonCameraSetup for child camera
        FirstPersonCameraSetup fpSetup = FindObjectOfType<FirstPersonCameraSetup>();
        if (fpSetup != null)
        {
            playerCamera = fpSetup.GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                Debug.Log($"Found camera as child of FirstPersonCameraSetup: {playerCamera.name}");
                cameraSearchComplete = true;
                return;
            }
        }

        // Method 4: FindObjectOfType (any camera in scene)
        playerCamera = FindObjectOfType<Camera>();
        if (playerCamera != null)
        {
            Debug.Log($"Found camera via FindObjectOfType: {playerCamera.name}");
            cameraSearchComplete = true;
            return;
        }

        // Camera not found - will retry next frame
        // Don't mark as complete so we keep searching
        Debug.LogWarning("Camera not found yet, will retry next frame...");
    }

    void CheckForRobot()
    {
        // Perform raycast from center of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Draw debug ray in Scene view (only visible in Scene tab, not Game tab)
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 0.1f);

        bool hitRobot = false;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Draw line to what we hit (visible in Scene view)
            Debug.DrawLine(ray.origin, hit.point, Color.green, 0.1f);

            // Check if we hit a robot (check for tag or component)
            if (hit.collider.CompareTag("Robot") || hit.collider.GetComponent<SimpleRobotMovement>() != null || hit.collider.GetComponent<RobotNavMeshMovement>() != null)
            {
                hitRobot = true;
                GameObject hitRobotObject = hit.collider.gameObject;

                // Only update if this is a new robot (reduces log spam and UI flicker)
                if (currentRobotInView != hitRobotObject)
                {
                    currentRobotInView = hitRobotObject;
                    Debug.Log($">>> ROBOT DETECTED: {currentRobotInView.name}");
                }

                // Only show interaction prompt if buttons are not active
                if (dialogueManager == null || !dialogueManager.AreInlineButtonsActive())
                {
                    ShowInteractionPrompt();
                }
                else
                {
                    // Buttons are showing, hide the "(E) Interact" prompt
                    HideInteractionPrompt();
                }
            }
        }

        // If we didn't hit a robot, hide the prompt and dialogue button
        if (!hitRobot)
        {
            // Only process if we were previously looking at a robot (avoids redundant calls)
            if (currentRobotInView != null || hasInteracted)
            {
                currentRobotInView = null;
                HideInteractionPrompt();

                // Also hide the dialogue single button if it's showing
                if (dialogueManager != null && hasInteracted)
                {
                    dialogueManager.HideSingleButton();
                    dialogueManager.dialoguePanel.SetActive(false);
                    // Reset interaction so player can interact with another robot
                    hasInteracted = false;
                    Debug.Log("Player looked away from robot - hiding dialogue and resetting interaction");
                }
            }
        }
    }

    void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);

            if (promptText != null)
            {
                promptText.text = "(E) Interact";
            }
        }
    }

    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void InteractWithRobot()
    {
        Debug.Log($"=== INTERACTION TRIGGERED ===");
        Debug.Log($"Interacting with robot: {currentRobotInView.name}");

        hasInteracted = true;
        HideInteractionPrompt();

        // Show single button in dialogue to follow this robot
        if (dialogueManager != null)
        {
            Debug.Log("DialogueManager found, calling ShowRobotInteractionButton...");
            dialogueManager.ShowRobotInteractionButton(currentRobotInView.name);
        }
        else
        {
            Debug.LogError("DialogueManager is NULL - cannot show interaction button!");
        }
    }

    // Public method to reset interaction (useful if you need to re-enable it)
    public void ResetInteraction()
    {
        hasInteracted = false;
        currentRobotInView = null;
        HideInteractionPrompt();
    }
}
