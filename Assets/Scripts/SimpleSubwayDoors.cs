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
    
    [Header("Settings")]
    public float animationTime = 2f;
    
    private DialogueManager dialogueManager;
    
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        StartCoroutine(WaitForAnnouncementEnd());
    }
    
    IEnumerator WaitForAnnouncementEnd()
    {
        // Wait for dialogue to end first
        while (dialogueManager.dialoguePanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Now wait for the announcement audio to finish
        AudioSource backgroundAudio = dialogueManager.backgroundAudioSource;
        if (backgroundAudio != null && backgroundAudio.isPlaying)
        {
            while (backgroundAudio.isPlaying)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        yield return new WaitForSeconds(0.5f); // Small delay after audio ends
        OpenDoors();
    }
    
    void OpenDoors()
    {
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