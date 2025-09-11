using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SubwayDoors : MonoBehaviour
{
    public float doorSpeed = 2f;
    
    void Start()
    {
        // Check current scene
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Start_Scene")
        {
            // Find the dialogue manager and wait for announcement
            DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
            if (dialogueManager != null)
            {
                StartCoroutine(WaitForAnnouncement(dialogueManager));
                Debug.Log("DialogueManager found, waiting for announcement");
            }
            else
            {
                Debug.LogError("DialogueManager not found in Start_Scene!");
            }
        }
        else if (currentScene == "ID_Scene")
        {
            // If we're in ID_Scene, play door opening animation
            OpenDoors();
            Debug.Log("ID_Scene detected - playing door opening animation");
        }
        else if (currentScene == "Waiting_Scene")
        {
            // In Waiting_Scene, doors should be closed (default state)
            Debug.Log("Waiting_Scene detected - doors remain in default closed position");
        }
        else
        {
            // In other scenes, do nothing
            Debug.Log($"SubwayDoors inactive in {currentScene}");
        }
    }
    
    IEnumerator WaitForAnnouncement(DialogueManager dialogueManager)
    {
        // Wait until the dialogue text contains "Attention, ladies and gentlemen" (line 8)
        while (!dialogueManager.dialogueText.text.Contains("Attention, ladies and gentlemen"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("Line 8 detected: 'Attention, ladies and gentlemen' - opening doors in 30 seconds");
        
        // Wait 32 seconds after line 8 appears (for announcement audio to end)
        yield return new WaitForSeconds(32f);
        
        // Transition to ID_Scene where doors will open
        TransitionToIDScene();
    }
    
    void OpenDoors()
    {
        Debug.Log("Opening subway doors");
        
        // Remove windows
        DestroyWindow("Part75");
        DestroyWindow("Part76");
        
        // Move doors
        MoveDoor("Door1_Left", Vector3.left * 3f);
        MoveDoor("Door1_Right", Vector3.right * 3f);
    }
    
    void OpenDoorsInstantly()
    {
        Debug.Log("Opening subway doors instantly");
        
        // Remove windows
        DestroyWindow("Part75");
        DestroyWindow("Part76");
        
        // Move doors instantly to open position
        MoveDoorInstantly("Door1_Left", Vector3.left * 3f);
        MoveDoorInstantly("Door1_Right", Vector3.right * 3f);
    }
    
    void DestroyWindow(string windowName)
    {
        GameObject window = GameObject.Find(windowName);
        if (window != null)
        {
            Destroy(window);
            Debug.Log($"Destroyed {windowName}");
        }
    }
    
    void MoveDoor(string doorName, Vector3 moveDirection)
    {
        GameObject door = GameObject.Find(doorName);
        if (door != null)
        {
            StartCoroutine(SlideDoor(door.transform, moveDirection));
        }
    }
    
    void MoveDoorInstantly(string doorName, Vector3 moveDirection)
    {
        GameObject door = GameObject.Find(doorName);
        if (door != null)
        {
            Vector3 newPosition = door.transform.position + moveDirection;
            door.transform.position = newPosition;
            Debug.Log($"Door {door.name} instantly moved to {newPosition}");
        }
        else
        {
            Debug.LogWarning($"Door {doorName} not found for instant movement");
        }
    }
    
    IEnumerator SlideDoor(Transform door, Vector3 moveDirection)
    {
        Vector3 startPos = door.transform.position;
        Vector3 endPos = startPos + moveDirection;
        
        float journey = 0f;
        
        while (journey <= 1f)
        {
            journey += Time.deltaTime * doorSpeed;
            door.transform.position = Vector3.Lerp(startPos, endPos, journey);
            yield return null;
        }
        
        door.transform.position = endPos;
        Debug.Log($"Door {door.name} moved to {endPos}");
    }
    
    void TransitionToIDScene()
    {
        Debug.Log("Transitioning to ID_Scene with fade");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("ID_Scene");
        }
        else
        {
            // Fallback to direct load if transition manager not found
            SceneManager.LoadScene("ID_Scene");
        }
    }
}