using UnityEngine;
using System.Collections;

public class SubwayDoors : MonoBehaviour
{
    public float doorSpeed = 2f;
    
    void Start()
    {
        // Find the dialogue manager and wait for announcement
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        StartCoroutine(WaitForAnnouncement(dialogueManager));
    }
    
    IEnumerator WaitForAnnouncement(DialogueManager dialogueManager)
    {
        // Wait until the dialogue text contains "Attention, ladies and gentlemen" (line 8)
        while (!dialogueManager.dialogueText.text.Contains("Attention, ladies and gentlemen"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("Line 8 detected: 'Attention, ladies and gentlemen' - opening doors in 30 seconds");
        
        // Wait 30 seconds after line 8 appears
        yield return new WaitForSeconds(30f);
        
        // Open doors
        OpenDoors();
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
}