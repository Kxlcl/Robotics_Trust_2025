using UnityEngine;
using System.Collections;

public class SimpleSubwayDoors : MonoBehaviour
{
    [Header("Door Objects")]
    public Transform leftDoor;  // Door1_Left
    public Transform rightDoor; // Door1_Right
    
    [Header("Windows to Remove")]
    public GameObject part75; // Window part 75
    public GameObject part76; // Window part 76
    
    [Header("Door Area Colliders to Remove")]
    public GameObject[] doorAreaObjects; // Any objects blocking the door area
    
    [Header("Settings")]
    public float animationTime = 2f;
    public float announcementDuration = 10f; // How long to wait after dialogue ends for announcement
    
    private DialogueManager dialogueManager;
    private PlayerController playerController;
    private bool doorsOpened = false;
    
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        playerController = FindObjectOfType<PlayerController>();
        StartCoroutine(WaitForGameToStart());
    }
    
    IEnumerator WaitForGameToStart()
    {
        // Wait for the Start button to be pressed (PlayerController.gameStarted becomes true)
        while (!playerController.gameStarted)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Wait a moment for dialogue to start
        yield return new WaitForSeconds(0.2f);
        
        // Now start waiting for announcement to end
        StartCoroutine(WaitForAnnouncementEnd());
    }
    
    IEnumerator WaitForAnnouncementEnd()
    {
        Debug.Log("Waiting for dialogue to end...");
        
        // Wait for dialogue to end first
        while (dialogueManager.dialoguePanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("Dialogue ended, waiting 32 seconds before opening doors...");
        
        // Wait 32 seconds after dialogue ends
        yield return new WaitForSeconds(32f);
        
        Debug.Log("32 seconds completed, opening doors!");
        OpenDoors();
    }
    
    void OpenDoors()
    {
        if (doorsOpened)
        {
            Debug.Log("Doors already opened, ignoring");
            return;
        }
        
        doorsOpened = true;
        Debug.Log("OPENING DOORS NOW!");
        
        // Remove window parts when doors start opening
        if (part75 != null)
        {
            Destroy(part75);
            Debug.Log("Removed window part 75");
        }
        
        if (part76 != null)
        {
            Destroy(part76);
            Debug.Log("Removed window part 76");
        }
        
        // Remove colliders from door area objects
        foreach (GameObject obj in doorAreaObjects)
        {
            if (obj != null)
            {
                Collider[] colliders = obj.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    Destroy(col);
                    Debug.Log($"Removed collider from {obj.name}");
                }
            }
        }
        
        // Open the doors
        if (leftDoor != null)
            StartCoroutine(MoveDoor(leftDoor, 3.3f, -2f));
        
        if (rightDoor != null)
            StartCoroutine(MoveDoor(rightDoor, -3.86f, 2f));
    }
    
    IEnumerator MoveDoor(Transform door, float startX, float endX)
    {
        Vector3 startPos = door.localPosition;
        startPos.x = startX;
        
        Vector3 endPos = startPos;
        endPos.x = endX;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationTime;
            
            door.localPosition = Vector3.Lerp(startPos, endPos, progress);
            yield return null;
        }
        
        door.localPosition = endPos;
    }
}