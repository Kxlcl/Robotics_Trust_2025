using UnityEngine;
using System.Collections;

public class SubwayDoorController : MonoBehaviour
{
    [Header("Door Movement")]
    public bool isLeftDoor = true; // true for left door, false for right door
    public float openDistance = 1.5f; // how far to slide open
    public float animationTime = 2f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Audio")]
    public AudioClip doorOpenSound;
    
    [Header("Auto Open Settings")]
    public bool autoOpenAfterAnnouncement = true;
    public float delayAfterAnnouncement = 1f; // delay after announcement ends
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isAnimating = false;
    private AudioSource audioSource;
    private DialogueManager dialogueManager;
    
    void Start()
    {
        // Store initial position as closed
        closedPosition = transform.localPosition;
        
        // Calculate open position based on door side
        Vector3 openDirection = isLeftDoor ? Vector3.left : Vector3.right;
        openPosition = closedPosition + (openDirection * openDistance);
        
        // Get or create audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // DISABLED - Let SimpleSubwayDoors handle door opening instead
        /*
        // Find dialogue manager to listen for announcement end
        if (autoOpenAfterAnnouncement)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            if (dialogueManager != null)
            {
                // Start checking for dialogue end
                StartCoroutine(WaitForAnnouncementEnd());
            }
        }
        */
        
        Debug.Log($"Subway door initialized - {(isLeftDoor ? "Left" : "Right")} door");
    }
    
    IEnumerator WaitForAnnouncementEnd()
    {
        // Wait until dialogue panel becomes inactive (announcement ends)
        while (dialogueManager.dialoguePanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Wait additional delay
        yield return new WaitForSeconds(delayAfterAnnouncement);
        
        // Open the doors
        OpenDoor();
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
    
    public void ToggleDoor()
    {
        if (isAnimating) return;
        
        isOpen = !isOpen;
        StartCoroutine(AnimateDoor());
    }
    
    IEnumerator AnimateDoor()
    {
        isAnimating = true;
        
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        
        // Play sound when opening
        if (isOpen && audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            
            // Apply animation curve
            float curveValue = animationCurve.Evaluate(progress);
            
            // Interpolate position
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            transform.localPosition = currentPosition;
            
            yield return null;
        }
        
        // Ensure final position is exact
        transform.localPosition = targetPosition;
        isAnimating = false;
        
        Debug.Log($"{(isLeftDoor ? "Left" : "Right")} subway door {(isOpen ? "opened" : "closed")}");
    }
    
    // Manual controls for testing
    void Update()
    {
        // For testing - remove these if not needed
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenDoor();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseDoor();
        }
    }
    
    // Visual debugging
    void OnDrawGizmosSelected()
    {
        Vector3 basePos = Application.isPlaying ? closedPosition : transform.localPosition;
        Vector3 openDirection = isLeftDoor ? Vector3.left : Vector3.right;
        Vector3 openPos = basePos + (openDirection * openDistance);
        
        // Draw movement path
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.TransformPoint(basePos), transform.TransformPoint(openPos));
        
        // Draw open position
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.TransformPoint(openPos), transform.localScale);
    }
}