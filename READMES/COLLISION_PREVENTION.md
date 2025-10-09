# Collision Prevention Guide

This document explains how the two decision systems coexist without conflicts.

---

## Two Decision Systems

### 1. **DecisionManager System** (Original)
- Located in: `DecisionManager.cs`
- Uses: Separate decision panel with buttons
- Handles Q/E input in its own `Update()` method
- Used for: ID_Scene robot selection and Yes/No decisions

### 2. **Inline Button System** (New)
- Located in: `DialogueManager.cs`
- Uses: Buttons inside the dialogue box
- Handles Q/E input in its own `Update()` method
- Used for: Waiting_Scene work/wait decisions and robot requests

---

## Scene Breakdown

### **ID_Scene**
✅ **Uses**: DecisionManager system
- Robot selection (Q/E to choose which robot to follow)
- Yes/No decisions (Q/E for ID verification)
- DecisionManager is active and handles keyboard input

❌ **Does NOT use**: Inline buttons
- DialogueManager shows text but no inline buttons in this scene

### **Waiting_Scene**
✅ **Uses**: Inline button system (DialogueManager)
- Work/Wait decisions (Q/E to ask about work or wait)
- Robot request decisions (Q/E to choose which robot)
- DialogueManager handles keyboard input via inline buttons

❌ **Does NOT use**: DecisionManager buttons
- DecisionManager is present but its decision panel stays hidden
- DecisionManager's Q/E input is NOT triggered

### **Start_Scene**
- Only shows dialogue text
- No decision systems active

---

## How Conflicts Are Prevented

### 1. **Scene-Based Activation**
```csharp
// DecisionManager.cs - Start()
if (currentScene == "ID_Scene") {
    // DecisionManager active
} else if (currentScene == "Waiting_Scene") {
    // DecisionManager present but decision panel hidden
}
```

```csharp
// DialogueManager.cs - Only shows inline buttons when triggered
// Inline buttons only appear in Waiting_Scene after specific dialogue
```

### 2. **State Flags**
- **DecisionManager**: Uses `awaitingKeyInput` flag
  - Only true when decision panel is shown
  - Q/E input only processed when this flag is true

- **DialogueManager**: Uses `inlineButtonsActive` flag
  - Only true when inline buttons are shown
  - Q/E input only processed when this flag is true

### 3. **Mutually Exclusive Usage**
- **In ID_Scene**:
  - DecisionManager `awaitingKeyInput = true`
  - DialogueManager `inlineButtonsActive = false`

- **In Waiting_Scene**:
  - DecisionManager `awaitingKeyInput = false` (panel hidden)
  - DialogueManager `inlineButtonsActive = true` (when buttons shown)

---

## Code Flow

### ID_Scene Flow
```
Player interacts with robot
  ↓
DecisionManager.TriggerDecision()
  ↓
DecisionManager.ShowDecisions()
  ↓
awaitingKeyInput = true
  ↓
DecisionManager.Update() listens for Q/E
  ↓
Player presses Q or E
  ↓
DecisionManager.SelectOption()
  ↓
awaitingKeyInput = false
```

### Waiting_Scene Flow
```
Player enters Waiting_Scene
  ↓
DialogueManager.ShowWaitingSceneDialogue()
  ↓
After delay...
  ↓
DialogueManager.ShowInlineWorkWaitButtons()
  ↓
inlineButtonsActive = true
  ↓
DialogueManager.Update() listens for Q/E
  ↓
Player presses Q or E (or clicks button)
  ↓
Callback invoked (OnAskAboutWork or OnStayAndWait)
  ↓
DialogueManager.HideInlineButtons()
  ↓
inlineButtonsActive = false
```

---

## Testing for Conflicts

### Test 1: ID_Scene
1. Play and go to ID_Scene
2. Look at a robot and press E to interact
3. Q/E choices should appear (DecisionManager panel)
4. Press Q or E
5. ✅ **Expected**: DecisionManager handles input correctly
6. ❌ **Conflict**: If DialogueManager also responds, there's a collision

### Test 2: Waiting_Scene
1. Play and progress to Waiting_Scene
2. Wait for "I really need to get to work..." dialogue
3. Inline buttons should appear inside dialogue box
4. Press Q or E
5. ✅ **Expected**: DialogueManager inline buttons handle input
6. ❌ **Conflict**: If DecisionManager also responds, there's a collision

### Test 3: Switching Scenes
1. Play through ID_Scene (use DecisionManager)
2. Progress to Waiting_Scene (use inline buttons)
3. ✅ **Expected**: Each scene uses its own system independently
4. ❌ **Conflict**: If old system still responds in new scene

---

## Debugging Conflicts

### If Q/E presses trigger both systems:

1. **Check Console Logs**
   - Look for duplicate "Decision made" logs
   - Both DecisionManager and DialogueManager logging

2. **Add Debug Logs**
   ```csharp
   // In DecisionManager.Update()
   if (Input.GetKeyDown(KeyCode.Q)) {
       Debug.Log("DecisionManager handling Q press");
   }

   // In DialogueManager.Update()
   if (Input.GetKeyDown(KeyCode.Q)) {
       Debug.Log("DialogueManager handling Q press");
   }
   ```

3. **Check State Flags**
   ```csharp
   // In DecisionManager
   Debug.Log($"DecisionManager awaitingKeyInput: {awaitingKeyInput}");

   // In DialogueManager
   Debug.Log($"DialogueManager inlineButtonsActive: {inlineButtonsActive}");
   ```

### If one system isn't working:

1. **DecisionManager not responding**
   - Check if `awaitingKeyInput` is true
   - Verify decision panel is active
   - Check if DecisionManager.Update() is running

2. **Inline buttons not responding**
   - Check if `inlineButtonsActive` is true
   - Verify inline button container is active
   - Check if DialogueManager.Update() is running
   - Verify callbacks are set (`currentOption1Callback`, `currentOption2Callback`)

---

## Summary

✅ **No Conflicts**: Systems use different scenes and state flags
✅ **Clear Separation**: ID_Scene = DecisionManager, Waiting_Scene = Inline Buttons
✅ **State-Based**: Only one system listens for Q/E at a time
✅ **Tested Flow**: Each scene independently manages its decision system

**Key Rule**: Only ONE system should have its "active" flag set to true at any given time.

---

## Future Considerations

If you want to use inline buttons in ID_Scene:
1. Disable DecisionManager's Q/E input in ID_Scene
2. Replace DecisionManager.ShowDecisions() calls with DialogueManager.ShowInlineButtons()
3. Update all decision triggers to use the inline system

If you want to keep both systems in the same scene:
1. Use a priority system (e.g., inline buttons override DecisionManager)
2. Add mutual exclusion checks
3. Disable one system when the other is active
