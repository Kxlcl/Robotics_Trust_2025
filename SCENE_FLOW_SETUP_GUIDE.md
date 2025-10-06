# Scene Flow System - Setup Guide

## 🎯 Overview

The new **Scene Flow System** makes it easy to create scene transitions with dialogue and choices **without writing any code!**

Instead of manually scripting each scene's dialogue and transitions, you now:
1. Create a SceneFlowData asset (right-click in Unity)
2. Configure dialogue, choices, and transitions in the Inspector
3. Assign it to the SceneFlowManager
4. Done! 🎉

---

## 📦 New Components

### 1. SceneFlowData (ScriptableObject)
**What it is:** A data file that defines the flow for one scene

**What it contains:**
- Dialogue text for each step
- Decision options and hotkeys
- Scene transitions
- Player repositioning

### 2. SceneFlowManager (Component)
**What it is:** The brain that executes scene flows

**What it does:**
- Automatically loads the correct flow for each scene
- Shows dialogue and decisions
- Handles transitions
- Records choices to database

---

## 🚀 Quick Setup (5 Steps)

### Step 1: Create SceneFlowManager GameObject

1. In your **first scene** (Start_Scene or ID_Scene)
2. Create empty GameObject: `GameObject → Create Empty`
3. Name it: **"SceneFlowManager"**
4. Add component: **SceneFlowManager**
5. The manager will persist across all scenes automatically (DontDestroyOnLoad)

### Step 2: Assign UI References

In the SceneFlowManager Inspector:

**UI References:**
- Drag your **DialoguePanel** GameObject → `Dialogue Panel`
- Drag your **DialogueText** (TextMeshPro) → `Dialogue Text`
- Drag your **DecisionPanel** GameObject → `Decision Panel`
- Drag all **decision buttons** → `Decision Buttons` (usually 2-4 buttons)

**Note:** The manager will also auto-find these in each scene by name.

### Step 3: Create SceneFlowData Assets

For each scene in your game, create a flow asset:

1. Right-click in Project window: `Create → Game Data → Scene Flow`
2. Name it: `SceneFlow_ID`, `SceneFlow_Waiting`, etc.
3. Configure the flow in Inspector (see examples below)

### Step 4: Assign Flows to Manager

In SceneFlowManager Inspector:
- Expand `Scene Flows` list
- Set size to match number of scenes (e.g., 3)
- Drag each SceneFlowData asset into the list

### Step 5: Test!

Press Play and watch your dialogue flow automatically!

---

## 📝 Example: ID Scene Flow

Here's how to recreate your ID_Scene flow:

### SceneFlow_ID Configuration:

**Scene Information:**
- Scene Name: `ID_Scene`
- Scene Description: `Robot selection and ID verification`

**Dialogue Steps (4 steps):**

#### Step 1: Choose Robot
- **Dialogue Text:** `"Choose a robot to follow."`
- **Display Duration:** `3`
- **Show Decisions:** ✓ (checked)
- **Decision Type:** `RobotChoice`
- **After Action:** `ShowDecisions`

**Decision Options:**
1. Choice ID: `robot_a`, Choice Text: `Follow Robot A`, Hotkey: `Q`, Outcome: `Continue`
2. Choice ID: `robot_b`, Choice Text: `Follow Robot B`, Hotkey: `E`, Outcome: `Continue`

#### Step 2: ID Verification Request
- **Dialogue Text:** `"Please provide a form of identification to verify your presence at the train station today. Time is critical. Please cooperate for your safety and others."`
- **Display Duration:** `4`
- **Show Decisions:** ✓
- **Decision Type:** `YesNo`
- **After Action:** `ShowDecisions`

**Decision Options:**
1. Choice ID: `id_yes`, Choice Text: `Yes`, Hotkey: `Q`, Outcome: `JumpToStep`, Next Step Index: `3`
2. Choice ID: `id_no`, Choice Text: `No`, Hotkey: `E`, Outcome: `JumpToStep`, Next Step Index: `4`

#### Step 3: Follow Dialogue (After YES)
- **Dialogue Text:** `"Thank you for your cooperation. Please follow me and the other passengers in my group to be escorted to a safe area."`
- **Display Duration:** `3`
- **After Action:** `TransitionScene`
- **Next Scene If Chosen:** `Waiting_Scene`

#### Step 4: Cooperation Message (After NO - First Time)
- **Dialogue Text:** `"Please cooperate for your safety and others."`
- **Display Duration:** `3`
- **Show Decisions:** ✓
- **After Action:** `ShowDecisions`

**Decision Options:**
1. Choice ID: `id_yes_2`, Choice Text: `Yes`, Hotkey: `Q`, Outcome: `JumpToStep`, Next Step Index: `3`
2. Choice ID: `id_no_2`, Choice Text: `No`, Hotkey: `E`, Outcome: `GameOver`

**Scene Transition Settings:**
- Next Scene Name: `Waiting_Scene`
- Transition Delay: `2`
- Reposition Player: ✓
- New Player Position: `(170, 0, -95)`

---

## 📝 Example: Waiting Scene Flow

### SceneFlow_Waiting Configuration:

**Scene Information:**
- Scene Name: `Waiting_Scene`
- Scene Description: `Work or wait decision`

**Dialogue Steps (1 step):**

#### Step 1: Work Decision
- **Dialogue Text:** `"I really need to get to work..."`
- **Display Duration:** `3`
- **Show Decisions:** ✓
- **Decision Type:** `WorkWait`
- **After Action:** `ShowDecisions`

**Decision Options:**
1. Choice ID: `ask_work`, Choice Text: `Ask about leaving for work`, Hotkey: `Q`, Outcome: `Continue`
2. Choice ID: `stay_wait`, Choice Text: `Stay put and wait`, Hotkey: `E`, Outcome: `Continue`

**Scene Transition Settings:**
- Next Scene Name: _(leave empty for now)_
- Transition Delay: `2`

---

## 🎨 Creating a New Scene Flow

To add a new scene with dialogue:

### 1. Create the SceneFlowData

```
Right-click in Project → Create → Game Data → Scene Flow
Name: SceneFlow_YourSceneName
```

### 2. Configure Basic Info

```
Scene Name: YourSceneName (must match Unity scene name!)
Scene Description: Brief description
```

### 3. Add Dialogue Steps

Click `+` to add dialogue steps. For each step:

**Simple Dialogue (no choices):**
- Dialogue Text: Your text
- Display Duration: How long to show
- After Action: `NextDialogue` (auto-continue) or `WaitForTrigger`

**Dialogue with Choices:**
- Dialogue Text: Your question
- Display Duration: How long before showing buttons
- Show Decisions: ✓
- After Action: `ShowDecisions`
- Add Decision Options (click `+` under Decision Options)

### 4. Configure Choices

For each decision option:
- **Choice ID:** Unique identifier (e.g., `choice_yes`)
- **Choice Text:** What player sees
- **Hotkey:** Q, E, W, etc.
- **Outcome:** What happens when chosen
  - `Continue` = Go to next step
  - `JumpToStep` = Jump to specific step (enter index)
  - `SceneTransition` = Go to scene (enter scene name in Target Scene)
  - `GameOver` = Trigger game over

### 5. Set Scene Transition

At the bottom of SceneFlowData:
- **Next Scene Name:** Scene to load after all steps
- **Transition Delay:** Pause before transition
- **Reposition Player:** Move player before transition?
- **New Player Position:** Where to move player

### 6. Add to SceneFlowManager

- Open SceneFlowManager in scene
- Add new flow to `Scene Flows` list
- Done!

---

## 🔄 Migration from Old System

### What to Remove (AFTER testing new system):

**Can be disabled/removed:**
- ✅ DialogueManager.cs (replaced by SceneFlowManager)
- ✅ DecisionManager.cs (replaced by SceneFlowManager)
- ✅ Manual scene transition code in DialogueManager
- ✅ Manual decision button setup

**Keep these:**
- ✅ SceneTransitionManager (still used for fades)
- ✅ PlayerChoiceTracker (still records choices)
- ✅ GlobalTimer (still tracks time)
- ✅ GameOverManager (still handles game over)
- ✅ DatabaseSubmitter (still submits data)

### Migration Steps:

1. **Set up SceneFlowManager** (follow Quick Setup above)
2. **Create SceneFlowData assets** for your scenes
3. **Test thoroughly** with new system
4. **Once working**, disable old DialogueManager and DecisionManager
5. **Remove old scripts** from scenes (keep the .cs files as backup)

---

## 💡 Advanced Features

### Branching Dialogue

Use `JumpToStep` to create branching:

```
Step 0: "Do you trust me?"
  Choice A (Yes) → JumpToStep: 1 (continue path)
  Choice B (No) → JumpToStep: 5 (different path)

Step 1: "Great! Let's go."
Step 2-4: ... continue yes path ...

Step 5: "That's unfortunate."
Step 6-8: ... continue no path ...
```

### Multiple Scene Transitions

Different choices can lead to different scenes:

```
Step 0: "Which way?"
  Choice A (Left) → Outcome: SceneTransition, Target Scene: "LeftPath"
  Choice B (Right) → Outcome: SceneTransition, Target Scene: "RightPath"
```

### Waiting for External Triggers

Use `WaitForTrigger` then call from code:

```csharp
// In your custom script:
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        SceneFlowManager.Instance.ContinueFlow();
    }
}
```

---

## 🐛 Troubleshooting

### Dialogue doesn't appear

**Check:**
- SceneFlowManager has UI references assigned
- UI GameObjects have correct names (DialoguePanel, DecisionPanel)
- SceneFlowData.sceneName matches Unity scene name exactly

### Choices don't work

**Check:**
- Decision buttons are in the Decision Buttons list
- Hotkeys are set (Q, E, etc.)
- Show Decisions is checked

### Scene doesn't transition

**Check:**
- Next Scene Name or Target Scene is spelled correctly
- Scene is added to Build Settings
- SceneTransitionManager exists in first scene

### Flow doesn't start

**Check:**
- SceneFlowData is in the Scene Flows list
- sceneName field matches current scene name
- At least one dialogue step exists

---

## ✅ Benefits of New System

### Before (Old System):
- ❌ Had to write code for each scene's dialogue
- ❌ Manual decision button setup
- ❌ Hard to change dialogue flow
- ❌ Dialogue/choices spread across multiple scripts

### After (New System):
- ✅ No coding needed for new scenes
- ✅ All configuration in Inspector
- ✅ Easy to edit and iterate
- ✅ Visual flow in ScriptableObject
- ✅ Reusable and modular
- ✅ Can create scene flows in minutes

---

## 📚 Summary

**To add a new scene with dialogue:**

1. Create SceneFlowData asset
2. Add dialogue steps in Inspector
3. Configure choices and transitions
4. Add to SceneFlowManager list
5. Done! ✨

**No coding required!**

---

*Scene Flow System v1.0*
*Created: October 6, 2025*
