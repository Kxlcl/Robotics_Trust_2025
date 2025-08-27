using UnityEngine;

public class SuspiciousBehavior : ConditionalBehavior
{
    [Header("Suspicious Behavior")]
    public float cautionLevel = 0.7f;
    public string[] cautiousMessages = {
        "I'm not sure I can trust you...",
        "Something seems off about you.",
        "I'll keep my distance."
    };

    protected override void OnBehaviorActivated()
    {
        base.OnBehaviorActivated();
        
        // Modify NPC behaviors to be more cautious
        ApplySuspiciousBehavior();
    }

    void ApplySuspiciousBehavior()
    {
        // Example: Make NPCs move away from player
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in npcs)
        {
            // Add cautious movement behavior
            CautiousMovement cautious = npc.GetComponent<CautiousMovement>();
            if (cautious == null)
                cautious = npc.AddComponent<CautiousMovement>();
            
            cautious.enabled = true;
        }
        
        ShowCautiousMessage();
    }

    void ShowCautiousMessage()
    {
        if (Random.value < cautionLevel && cautiousMessages.Length > 0)
        {
            string message = cautiousMessages[Random.Range(0, cautiousMessages.Length)];
            Debug.Log($"Suspicious NPC: {message}");
        }
    }
}

// Helper component for cautious NPC movement
public class CautiousMovement : MonoBehaviour
{
    public float avoidanceDistance = 5f;
    public float moveSpeed = 2f;

    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < avoidanceDistance)
            {
                // Move away from player
                Vector3 direction = (transform.position - player.transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }
}