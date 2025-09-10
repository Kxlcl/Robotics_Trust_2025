using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float animationTime = 1f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    
    [Header("Interaction")]
    public bool requirePlayerNearby = true;
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    
    private Vector3 closedRotation;
    private Vector3 openRotation;
    private bool isAnimating = false;
    private AudioSource audioSource;
    private Transform player;
    
    void Start()
    {
        // Store initial rotation as closed position
        closedRotation = transform.localEulerAngles;
        openRotation = closedRotation + new Vector3(0, openAngle, 0);
        
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Find player
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        
        // Set initial state
        if (isOpen)
        {
            transform.localEulerAngles = openRotation;
        }
    }
    
    void Update()
    {
        // Check for interaction input
        if (Input.GetKeyDown(interactionKey))
        {
            TryToggleDoor();
        }
    }
    
    public void TryToggleDoor()
    {
        // Check if player is close enough (if required)
        if (requirePlayerNearby && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > interactionDistance)
            {
                Debug.Log("Player too far from door");
                return;
            }
        }
        
        // Don't allow interaction while animating
        if (isAnimating)
        {
            return;
        }
        
        ToggleDoor();
    }
    
    public void ToggleDoor()
    {
        if (isAnimating) return;
        
        isOpen = !isOpen;
        StartCoroutine(AnimateDoor());
    }
    
    public void OpenDoor()
    {
        if (isOpen || isAnimating) return;
        
        isOpen = true;
        StartCoroutine(AnimateDoor());
    }
    
    public void CloseDoor()
    {
        if (!isOpen || isAnimating) return;
        
        isOpen = false;
        StartCoroutine(AnimateDoor());
    }
    
    IEnumerator AnimateDoor()
    {
        isAnimating = true;
        
        Vector3 startRotation = transform.localEulerAngles;
        Vector3 targetRotation = isOpen ? openRotation : closedRotation;
        
        // Play sound
        if (audioSource != null)
        {
            AudioClip soundToPlay = isOpen ? openSound : closeSound;
            if (soundToPlay != null)
            {
                audioSource.PlayOneShot(soundToPlay);
            }
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            
            // Apply animation curve
            float curveValue = animationCurve.Evaluate(progress);
            
            // Interpolate rotation
            Vector3 currentRotation = Vector3.Lerp(startRotation, targetRotation, curveValue);
            transform.localEulerAngles = currentRotation;
            
            yield return null;
        }
        
        // Ensure final position is exact
        transform.localEulerAngles = targetRotation;
        isAnimating = false;
        
        Debug.Log($"Door {(isOpen ? "opened" : "closed")}");
    }
    
    // Trigger-based interaction
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player near door - Press E to interact");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left door area");
        }
    }
    
    // Visual debugging
    void OnDrawGizmosSelected()
    {
        if (requirePlayerNearby)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}