using UnityEngine;

public class ConditionalBehavior : MonoBehaviour
{
    [Header("Behavior Settings")]
    public string behaviorName;
    public bool startEnabled = false;
    
    [Header("Conditional Activation")]
    public string requiredDecisionKey;
    public int requiredDecisionValue = -1;
    
    void Start()
    {
        enabled = startEnabled;
        
        // Check if this behavior should be enabled based on previous decisions
        if (!string.IsNullOrEmpty(requiredDecisionKey))
        {
            int decisionValue = DecisionActionManager.GetDecisionValue(requiredDecisionKey);
            if (decisionValue == requiredDecisionValue)
            {
                enabled = true;
                OnBehaviorActivated();
            }
        }
    }

    protected virtual void OnBehaviorActivated()
    {
        Debug.Log($"Behavior '{behaviorName}' activated!");
    }

    protected virtual void OnBehaviorDeactivated()
    {
        Debug.Log($"Behavior '{behaviorName}' deactivated!");
    }

    void OnEnable()
    {
        OnBehaviorActivated();
    }

    void OnDisable()
    {
        OnBehaviorDeactivated();
    }
}