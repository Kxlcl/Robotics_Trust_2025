using UnityEngine;

public class DecisionTrigger : MonoBehaviour
{
    [Header("Decision Setup")]
    public DecisionActionManager decisionManager;
    public bool triggerOnce = true;
    public bool requireInteraction = false;
    public KeyCode interactionKey = KeyCode.E;

    [Header("Trigger Conditions")]
    public string requiredTag = "Player";
    public float interactionRange = 3f;

    [Header("Visual Feedback")]
    public GameObject interactionPrompt;
    public string promptText = "Press E to interact";

    private bool hasTriggered = false;
    private bool playerInRange = false;
    private DecisionUI decisionUI;

    void Start()
    {
        decisionUI = FindObjectOfType<DecisionUI>();
        if (decisionUI == null)
        {
            Debug.LogError("DecisionUI not found in scene! Please add DecisionUI component.");
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (requireInteraction && playerInRange && !hasTriggered)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TriggerDecision();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            playerInRange = true;

            if (requireInteraction)
            {
                ShowInteractionPrompt();
            }
            else if (!hasTriggered)
            {
                TriggerDecision();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            playerInRange = false;
            HideInteractionPrompt();
        }
    }

    void TriggerDecision()
    {
        if (triggerOnce && hasTriggered) return;
        if (decisionManager == null || decisionUI == null) return;

        hasTriggered = true;
        HideInteractionPrompt();

        // Show the decision UI with our decision manager
        decisionUI.ShowDecision(decisionManager);
        decisionManager.ShowDecision();
    }

    void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void OnGUI()
    {
        if (requireInteraction && playerInRange && !hasTriggered)
        {
            GUI.Label(new Rect(Screen.width/2 - 100, Screen.height - 100, 200, 50), promptText);
        }
    }

    // Method to reset trigger (useful for testing)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}