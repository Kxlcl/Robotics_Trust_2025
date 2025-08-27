using UnityEngine;

public class StationDecisionScenario : MonoBehaviour
{
    [Header("Station-Specific Behaviors")]
    public GameObject trustfulNPCs;
    public GameObject suspiciousNPCs;
    public GameObject neutralNPCs;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioClip trustfulMusic;
    public AudioClip suspiciousMusic;

    void Start()
    {
        // Check previous decisions and modify station accordingly
        CheckPreviousDecisions();
    }

    void CheckPreviousDecisions()
    {
        // Check if player made trustful decisions
        int trustLevel = DecisionActionManager.GetDecisionValue("HelpDecision_Do you help the person?");
        
        if (trustLevel == 1) // Trustful choice
        {
            EnableTrustfulEnvironment();
        }
        else if (trustLevel == 2) // Suspicious choice
        {
            EnableSuspiciousEnvironment();
        }
        else
        {
            EnableNeutralEnvironment();
        }
    }

    void EnableTrustfulEnvironment()
    {
        if (trustfulNPCs != null) trustfulNPCs.SetActive(true);
        if (suspiciousNPCs != null) suspiciousNPCs.SetActive(false);
        if (neutralNPCs != null) neutralNPCs.SetActive(false);

        // Play trustful background music
        if (backgroundMusic != null && trustfulMusic != null)
        {
            backgroundMusic.clip = trustfulMusic;
            backgroundMusic.Play();
        }

        Debug.Log("Station environment set to TRUSTFUL");
    }

    void EnableSuspiciousEnvironment()
    {
        if (trustfulNPCs != null) trustfulNPCs.SetActive(false);
        if (suspiciousNPCs != null) suspiciousNPCs.SetActive(true);
        if (neutralNPCs != null) neutralNPCs.SetActive(false);

        // Play suspicious background music
        if (backgroundMusic != null && suspiciousMusic != null)
        {
            backgroundMusic.clip = suspiciousMusic;
            backgroundMusic.Play();
        }

        Debug.Log("Station environment set to SUSPICIOUS");
    }

    void EnableNeutralEnvironment()
    {
        if (trustfulNPCs != null) trustfulNPCs.SetActive(false);
        if (suspiciousNPCs != null) suspiciousNPCs.SetActive(false);
        if (neutralNPCs != null) neutralNPCs.SetActive(true);

        Debug.Log("Station environment set to NEUTRAL");
    }
}