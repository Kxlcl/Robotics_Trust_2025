using UnityEngine;

public class DecisionSetupHelper : MonoBehaviour
{
    [Header("Quick Setup")]
    public bool autoCreateExampleDecisions = false;
    
    void Start()
    {
        if (autoCreateExampleDecisions)
        {
            CreateExampleDecisions();
        }
    }

    [ContextMenu("Create Example Decisions")]
    void CreateExampleDecisions()
    {
        // Create Help Decision
        CreateHelpDecision();
        
        // Create Honesty Decision
        CreateHonestyDecision();
        
        Debug.Log("Example decisions created! Check the hierarchy for new GameObjects.");
    }

    void CreateHelpDecision()
    {
        GameObject helpDecisionGO = new GameObject("HelpDecision");
        DecisionActionManager helpDecision = helpDecisionGO.AddComponent<DecisionActionManager>();
        
        helpDecision.decisionQuestion = "Do you help the person?";
        
        // This would normally be set up in the inspector
        Debug.Log("Help Decision GameObject created. Please configure choices in Inspector.");
    }

    void CreateHonestyDecision()
    {
        GameObject honestyDecisionGO = new GameObject("HonestyDecision");
        DecisionActionManager honestyDecision = honestyDecisionGO.AddComponent<DecisionActionManager>();
        
        honestyDecision.decisionQuestion = "Are you honest about your intentions?";
        
        Debug.Log("Honesty Decision GameObject created. Please configure choices in Inspector.");
    }

    [ContextMenu("Setup Example NPCs")]
    void SetupExampleNPCs()
    {
        // Find existing NPCs and add reaction systems
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("npc") && obj.GetComponent<NPCReactionSystem>() == null)
            {
                NPCReactionSystem npcReaction = obj.AddComponent<NPCReactionSystem>();
                npcReaction.npcName = obj.name;
                
                // Setup default reactions
                npcReaction.trustfulReactions = new string[] { "Thank you for being kind!", "I appreciate your help." };
                npcReaction.suspiciousReactions = new string[] { "I don't trust you...", "Stay away from me." };
                npcReaction.neutralReactions = new string[] { "Hello there.", "Nice day, isn't it?" };
                
                Debug.Log($"Added NPCReactionSystem to {obj.name}");
            }
        }
    }
}