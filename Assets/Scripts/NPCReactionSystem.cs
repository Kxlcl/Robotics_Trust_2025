using UnityEngine;

public class NPCReactionSystem : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName;
    public NPCType npcType;
    
    [Header("Reaction Messages")]
    public string[] trustfulReactions;
    public string[] suspiciousReactions;
    public string[] neutralReactions;

    [Header("Movement")]
    public float approachDistance = 3f;
    public float avoidDistance = 6f;
    public float moveSpeed = 2f;

    private Transform player;
    private bool hasReacted = false;

    public enum NPCType
    {
        Civilian,
        Security,
        Vendor,
        Tourist
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // React based on previous player decisions
        if (!hasReacted && distanceToPlayer < 5f)
        {
            ReactToPlayer();
            hasReacted = true;
        }

        // Move based on trust level
        MoveBasedOnTrust();
    }

    void ReactToPlayer()
    {
        int trustLevel = GetPlayerTrustLevel();
        string reaction = "";

        switch (trustLevel)
        {
            case 1: // Trustful
                reaction = GetRandomMessage(trustfulReactions);
                break;
            case 2: // Suspicious
                reaction = GetRandomMessage(suspiciousReactions);
                break;
            default: // Neutral
                reaction = GetRandomMessage(neutralReactions);
                break;
        }

        if (!string.IsNullOrEmpty(reaction))
        {
            Debug.Log($"{npcName}: {reaction}");
            
            // You can integrate this with your dialogue system
            ShowReactionMessage(reaction);
        }
    }

    void MoveBasedOnTrust()
    {
        if (player == null) return;

        int trustLevel = GetPlayerTrustLevel();
        float distance = Vector3.Distance(transform.position, player.position);

        if (trustLevel == 1) // Trustful - approach player
        {
            if (distance > approachDistance)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(player);
            }
        }
        else if (trustLevel == 2) // Suspicious - avoid player
        {
            if (distance < avoidDistance)
            {
                Vector3 direction = (transform.position - player.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    int GetPlayerTrustLevel()
    {
        // Check multiple decision points to determine overall trust
        int helpDecision = DecisionActionManager.GetDecisionValue("HelpDecision_Do you help the person?");
        int honestDecision = DecisionActionManager.GetDecisionValue("HonestyDecision_Are you honest about your intentions?");
        
        if (helpDecision == 1 || honestDecision == 1)
            return 1; // Trustful
        else if (helpDecision == 2 || honestDecision == 2)
            return 2; // Suspicious
        else
            return 0; // Neutral
    }

    string GetRandomMessage(string[] messages)
    {
        if (messages == null || messages.Length == 0) return "";
        return messages[Random.Range(0, messages.Length)];
    }

    void ShowReactionMessage(string message)
    {
        // Create a floating text above NPC
        GameObject floatingText = new GameObject("FloatingText");
        floatingText.transform.position = transform.position + Vector3.up * 2f;
        
        TextMesh textMesh = floatingText.AddComponent<TextMesh>();
        textMesh.text = message;
        textMesh.fontSize = 20;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Auto-destroy after 3 seconds
        Destroy(floatingText, 3f);
    }
}