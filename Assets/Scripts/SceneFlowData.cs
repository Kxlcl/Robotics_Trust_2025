using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that defines the flow for a single scene
/// Create instances via: Assets > Create > Game Data > Scene Flow
/// </summary>
[CreateAssetMenu(fileName = "SceneFlow_", menuName = "Game Data/Scene Flow", order = 1)]
public class SceneFlowData : ScriptableObject
{
    [Header("Scene Information")]
    public string sceneName;
    [TextArea(2, 4)]
    public string sceneDescription;

    [Header("Dialogue Sequence")]
    public List<DialogueStep> dialogueSteps = new List<DialogueStep>();

    [Header("Scene Transition")]
    public string nextSceneName;
    public float transitionDelay = 2f;
    public bool repositionPlayer = false;
    public Vector3 newPlayerPosition;

    [System.Serializable]
    public class DialogueStep
    {
        [TextArea(3, 6)]
        public string dialogueText;

        public float displayDuration = 3f;

        public bool showDecisions = false;
        public DecisionType decisionType = DecisionType.None;
        public List<DecisionOption> decisionOptions = new List<DecisionOption>();

        // What happens after this step
        public StepAction afterAction = StepAction.NextDialogue;
        public string nextSceneIfChosen; // For decisions that lead to different scenes
    }

    [System.Serializable]
    public class DecisionOption
    {
        public string choiceId;
        public string choiceText;
        public KeyCode hotkey = KeyCode.Q;

        // What happens when this choice is selected
        public ChoiceOutcome outcome = ChoiceOutcome.Continue;
        public string targetScene; // If outcome is SceneTransition
        public int nextStepIndex = -1; // If outcome is JumpToStep
    }

    public enum DecisionType
    {
        None,
        RobotChoice,
        YesNo,
        WorkWait,
        Custom
    }

    public enum StepAction
    {
        NextDialogue,      // Go to next dialogue step
        ShowDecisions,     // Show decision buttons
        WaitForTrigger,    // Wait for external trigger
        TransitionScene,   // Move to next scene
        EndScene          // End this scene flow
    }

    public enum ChoiceOutcome
    {
        Continue,          // Continue to next dialogue step
        JumpToStep,        // Jump to specific step (for branching)
        SceneTransition,   // Transition to target scene
        GameOver          // Trigger game over
    }
}
