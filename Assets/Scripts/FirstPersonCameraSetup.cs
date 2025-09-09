using UnityEngine;

public class FirstPersonCameraSetup : MonoBehaviour
{
    [Header("Camera Setup")]
    public float eyeHeight = 1.8f; // Height of camera above ground
    public bool setupOnStart = true;
    
    private Camera playerCamera;
    private CharacterController characterController;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupFirstPersonCamera();
        }
    }
    
    [ContextMenu("Setup First Person Camera")]
    public void SetupFirstPersonCamera()
    {
        characterController = GetComponent<CharacterController>();
        
        // Find or create camera
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera == null)
        {
            // Create new camera as child
            GameObject cameraObj = new GameObject("First Person Camera");
            cameraObj.transform.SetParent(transform);
            playerCamera = cameraObj.AddComponent<Camera>();
            
            Debug.Log("Created new first person camera");
        }
        
        // Position camera correctly
        SetupCameraPosition();
        
        // Ensure camera is tagged as MainCamera
        if (playerCamera.tag != "MainCamera")
        {
            playerCamera.tag = "MainCamera";
        }
        
        Debug.Log($"First person camera setup complete. Position: {playerCamera.transform.localPosition}");
    }
    
    void SetupCameraPosition()
    {
        if (playerCamera == null) return;
        
        // Reset camera transform
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        
        // Position at eye height
        Vector3 eyePosition = Vector3.up * eyeHeight;
        
        // If we have a CharacterController, use its center and height
        if (characterController != null)
        {
            float controllerHeight = characterController.height;
            Vector3 controllerCenter = characterController.center;
            
            // Position camera near the top of the controller
            eyePosition = controllerCenter + Vector3.up * (controllerHeight * 0.45f);
        }
        
        playerCamera.transform.localPosition = eyePosition;
        
        Debug.Log($"Camera positioned at local position: {eyePosition}");
    }
    
    void OnDrawGizmos()
    {
        // Draw camera position
        if (playerCamera != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerCamera.transform.position, 0.1f);
            
            // Draw view direction
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2f);
        }
        
        // Draw character controller bounds
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            Gizmos.color = Color.green;
            Vector3 center = transform.position + cc.center;
            Gizmos.DrawWireCapsule(center, cc.radius, cc.height, 1);
        }
    }
    
    void OnValidate()
    {
        // Update camera position when values change in inspector
        if (Application.isPlaying && playerCamera != null)
        {
            SetupCameraPosition();
        }
    }
}