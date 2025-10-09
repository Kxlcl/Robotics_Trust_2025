# Scene Flow System - Quick Reference

## 🎯 System Architecture

```
┌─────────────────────────────────────────┐
│      SceneFlowData (ScriptableObject)   │
│  ┌───────────────────────────────────┐  │
│  │ Scene Name: "ID_Scene"            │  │
│  │ Dialogue Steps:                   │  │
│  │   Step 1: "Choose robot..."       │  │
│  │     → Show decisions (Q/E)        │  │
│  │   Step 2: "Provide ID..."         │  │
│  │     → Show decisions (Q/E)        │  │
│  │   Step 3: "Thank you..."          │  │
│  │     → Transition to Waiting       │  │
│  │ Next Scene: "Waiting_Scene"       │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
                    │
                    │ Assigned to
                    ↓
┌─────────────────────────────────────────┐
│      SceneFlowManager (GameObject)      │
│  ┌───────────────────────────────────┐  │
│  │ Scene Flows:                      │  │
│  │   [0] SceneFlow_Start             │  │
│  │   [1] SceneFlow_ID                │  │
│  │   [2] SceneFlow_Waiting           │  │
│  │                                   │  │
│  │ UI References:                    │  │
│  │   - DialoguePanel                 │  │
│  │   - DialogueText                  │  │
│  │   - DecisionPanel                 │  │
│  │   - DecisionButtons [Q] [E]       │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
                    │
                    │ Executes
                    ↓
┌─────────────────────────────────────────┐
│          Runtime Flow Execution         │
│                                         │
│  1. Scene loads → Load matching flow   │
│  2. Show dialogue step by step         │
│  3. Handle choices and branching       │
│  4. Record to PlayerChoiceTracker      │
│  5. Transition to next scene           │
└─────────────────────────────────────────┘
```

---

## 📋 SceneFlowData Cheat Sheet

### Step Actions

| Action | Description | When to Use |
|--------|-------------|-------------|
| `NextDialogue` | Auto-continue to next step | Sequential dialogue |
| `ShowDecisions` | Show choice buttons | When player needs to choose |
| `WaitForTrigger` | Pause until code calls Continue | External events |
| `TransitionScene` | Load new scene | End of flow |
| `EndScene` | Stop flow | Dead end |

### Choice Outcomes

| Outcome | Description | Required Fields |
|---------|-------------|-----------------|
| `Continue` | Next dialogue step | None |
| `JumpToStep` | Go to specific step | Next Step Index |
| `SceneTransition` | Load scene | Target Scene |
| `GameOver` | Trigger game over | None |

---

## 🎮 Hotkey Reference

| Key | Typical Use |
|-----|-------------|
| `Q` | Left/First choice |
| `E` | Right/Second choice |
| `W` | Third choice (optional) |
| `A` | Fourth choice (optional) |

---

## 📝 Common Patterns

### Pattern 1: Simple Dialogue Sequence

```
Step 0:
  Text: "Welcome!"
  Duration: 2
  Action: NextDialogue

Step 1:
  Text: "Let's begin."
  Duration: 2
  Action: NextDialogue

Step 2:
  Text: "Follow me."
  Duration: 2
  Action: TransitionScene
  Next Scene: "NextScene"
```

### Pattern 2: Yes/No Branch

```
Step 0:
  Text: "Do you agree?"
  Duration: 3
  Show Decisions: ✓
  Action: ShowDecisions
  Choices:
    [Q] Yes → JumpToStep: 1
    [E] No → JumpToStep: 2

Step 1: (Yes path)
  Text: "Great!"
  Action: TransitionScene
  Target: "SuccessScene"

Step 2: (No path)
  Text: "Too bad."
  Action: GameOver
```

### Pattern 3: Multi-Choice

```
Step 0:
  Text: "Choose your path:"
  Show Decisions: ✓
  Choices:
    [Q] Path A → SceneTransition: "PathA_Scene"
    [E] Path B → SceneTransition: "PathB_Scene"
    [W] Path C → SceneTransition: "PathC_Scene"
```

### Pattern 4: Second Chance

```
Step 0:
  Text: "Will you help?"
  Choices:
    [Q] Yes → JumpToStep: 3
    [E] No → JumpToStep: 1

Step 1: (First No)
  Text: "Are you sure?"
  Choices:
    [Q] Yes → JumpToStep: 3
    [E] No → JumpToStep: 2

Step 2: (Second No)
  Text: "Goodbye."
  Action: GameOver

Step 3: (Yes)
  Text: "Thank you!"
  Action: Continue
```

---

## 🔧 Setup Checklist

### Initial Setup (Once)
- [ ] Create SceneFlowManager GameObject in first scene
- [ ] Add SceneFlowManager component
- [ ] Assign UI references (DialoguePanel, DecisionPanel, Buttons)

### Per Scene
- [ ] Create SceneFlowData asset (`Create → Game Data → Scene Flow`)
- [ ] Set Scene Name (must match Unity scene name!)
- [ ] Add dialogue steps
- [ ] Configure choices and hotkeys
- [ ] Set scene transition settings
- [ ] Add to SceneFlowManager.sceneFlows list

### Testing
- [ ] Press Play in Unity
- [ ] Verify dialogue appears
- [ ] Test all choices work
- [ ] Verify scene transitions
- [ ] Check PlayerChoiceTracker records choices

---

## 🚨 Common Mistakes

| Mistake | Problem | Fix |
|---------|---------|-----|
| Scene Name mismatch | Flow doesn't load | Match Scene Name exactly with Unity scene |
| No dialogue steps | Nothing happens | Add at least one step |
| Forgot Show Decisions | Choices don't appear | Check "Show Decisions" ✓ |
| Wrong After Action | Flow stops/continues wrong | Set correct AfterAction for each step |
| Missing Target Scene | Transition fails | Fill in Next Scene Name or Target Scene |
| No hotkeys set | Can't choose | Set hotkey for each choice |

---

## 📊 SceneFlowData Template

```
Scene Name: ___________________
Description: ___________________

Step 0:
  Dialogue: _____________________
  Duration: ____
  Show Decisions: [ ]
  After Action: __________

  Choices:
    [Q] _________ → Outcome: _______, Target/Index: ____
    [E] _________ → Outcome: _______, Target/Index: ____

Step 1:
  ...

Scene Transition:
  Next Scene: ___________________
  Delay: ____
  Reposition Player: [ ]
  Position: (_____, _____, _____)
```

---

## 💻 Code Integration

### Call from external code:

```csharp
// Continue the flow (after WaitForTrigger)
SceneFlowManager.Instance.ContinueFlow();

// Jump to specific step
SceneFlowManager.Instance.JumpToStep(5);

// Check current flow
if (SceneFlowManager.Instance != null)
{
    // Flow is active
}
```

---

## 🎓 Learn by Example

### Your Current Scenes:

**ID_Scene:**
- 4 steps
- 2 decision points (robot choice, ID verification)
- Branches on No → second chance → game over
- Transitions to Waiting_Scene

**Waiting_Scene:**
- 1 step
- 1 decision point (work/wait)
- No immediate scene transition

Study the example configurations in **SCENE_FLOW_SETUP_GUIDE.md** for detailed setup!

---

*Quick Reference v1.0*
*For Scene Flow System*
