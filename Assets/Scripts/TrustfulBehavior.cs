using UnityEngine;

public class TrustfulBehavior : ConditionalBehavior
{
    [Header("Trustful Behavior")]
    public float friendlyDialogueChance = 0.8f;
    public string[] friendlyMessages = {
        "Thank you for helping!",
        "I appreciate your kindness.",
        "You seem trustworthy."
    };

    protected override void OnBehaviorActivated()
    {
        base.OnBehaviorActivated();
        
        // Modify dialogue system to use friendly messages
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            // You can modify dialogue behavior here
            ShowFriendlyMessage();
        }
    }

    void ShowFriendlyMessage()
    {
        if (Random.value < friendlyDialogueChance && friendlyMessages.Length > 0)
        {
            string message = friendlyMessages[Random.Range(0, friendlyMessages.Length)];
            Debug.Log($"Trustful NPC: {message}");
        }
    }

    void Update()
    {
        if (enabled)
        {
            // Trustful behavior logic here
            // For example: NPCs approach player more willingly
        }
    }
}