using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    
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
            Debug.LogError("No Camera found as child of PlayerController GameObject.");
        }
        
        // Force cursor to center of screen
        StartCoroutine(CenterCursorOnStart());
        
        Debug.Log("PlayerController initialized");
    }
    
    System.Collections.IEnumerator CenterCursorOnStart()
    {
        // Wait one frame for screen to initialize
        yield return null;
        
        // Set cursor to screen center
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // Temporarily lock to force centering
        Cursor.lockState = CursorLockMode.Locked;
        yield return null; // Wait another frame
        
        // Now set to confined mode for UI interaction
        Cursor.lockState = CursorLockMode.Confined;
        
        Debug.Log($"Cursor centered at screen center: {center}");
    }
    
    void Update()
    {
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
        
        HandleMouseLook();
        HandleMovement();
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
            return;
            
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
        
        // Debug input
        if (moveX != 0 || moveZ != 0)
        {
            Debug.Log($"Movement input - X: {moveX}, Z: {moveZ}");
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
        
        // Debug movement
        if (finalMovement.magnitude > 0.01f)
        {
            Debug.Log($"Moving player by: {finalMovement}");
        }
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
    
    void OnGUI()
    {
        // Display debug info on screen
        GUI.Label(new Rect(10, 10, 200, 20), $"Horizontal: {Input.GetAxis("Horizontal")}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Vertical: {Input.GetAxis("Vertical")}");
        GUI.Label(new Rect(10, 50, 200, 20), $"Grounded: {controller.isGrounded}");
    }
}