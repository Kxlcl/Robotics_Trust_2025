# Single Button Robot Interaction - Summary

The robot interaction now uses a **single E button** that appears inside the dialogue box instead of Q/E double buttons.

---

## What Changed

### Before:
1. Look at robot → "(E) Interact" prompt appears
2. Press E → Q/E buttons appear (separate panel)
3. Choose Q or E to select robot

### After:
1. Look at robot → "(E) Interact" prompt appears
2. Press E → Dialogue shows "Would you like to follow this robot?"
3. Single "(E) Follow this robot" button appears **inside dialogue box**
4. Press E or click button → Continues to ID verification

---

## Files Modified

### 1. **DialogueManager.cs**
- Added `singleButtonContainer`, `singleButton`, `singleButtonText` fields
- Added `singleButtonActive` state flag
- Added `ShowSingleButton()` and `HideSingleButton()` methods
- Added `ShowRobotInteractionButton(robotName)` method
- Updated `Update()` to handle E key for single button

### 2. **RobotInteraction.cs**
- Added `dialogueManager` reference
- Changed `InteractWithRobot()` to call `dialogueManager.ShowRobotInteractionButton()` instead of `decisionManager.TriggerDecision()`

### 3. **README_UI_Setup.md**
- Added Step 7: Create Single Button
- Updated Step 8 (was Step 7): Configure DialogueManager with single button fields
- Updated testing instructions

---

## Unity Setup Required

### 1. Create Single Button UI (Inside DialoguePanel)

```
DialoguePanel
  └── SingleButtonContainer
      └── SingleButton_E
          └── SingleButtonText (TextMeshPro)
```

### 2. Assign in DialogueManager Inspector

Under "Single Button" section:
- **Single Button Container** → SingleButtonContainer
- **Single Button** → SingleButton_E
- **Single Button Text** → SingleButtonText

---

## How It Works

### Robot Detection & Interaction Flow:

```
Player looks at robot
  ↓
RobotInteraction.CheckForRobot()
  → Raycast detects robot
  → Shows "(E) Interact" prompt
  ↓
Player presses E
  ↓
RobotInteraction.InteractWithRobot()
  ↓
DialogueManager.ShowRobotInteractionButton(robotName)
  → dialoguePanel.SetActive(true)
  → dialogueText.text = "Would you like to follow this robot?"
  → ShowSingleButton("Follow this robot", callback)
  ↓
Player presses E or clicks button
  ↓
DialogueManager.OnFollowRobot(robotName)
  → Records player choice
  → ShowIDVerificationDialogue()
  ↓
Continues with yes/no ID decisions...
```

### Keyboard Input:

```csharp
// In DialogueManager.Update()
if (singleButtonActive && Input.GetKeyDown(KeyCode.E))
{
    currentSingleButtonCallback?.Invoke();
    HideSingleButton();
}
```

---

## Benefits

✅ **Cleaner UI**: Single button instead of Q/E choice
✅ **More Intuitive**: "Follow this robot" is clearer than choosing Q vs E
✅ **Contextual**: Shows which robot you're interacting with
✅ **Integrated**: Button appears inside dialogue box, not separate panel
✅ **Flexible**: Still supports both keyboard (E) and mouse click

---

## Different Button Types Summary

### 1. **Single Button** (New)
- **Used for**: Robot interaction in ID_Scene
- **Shows**: "(E) Follow this robot"
- **Trigger**: Press E or click button
- **Location**: Inside dialogue box

### 2. **Inline Double Buttons** (Q/E)
- **Used for**: Work/Wait decisions, Robot requests in Waiting_Scene
- **Shows**: "(Q) Option 1" and "(E) Option 2"
- **Trigger**: Press Q, E, or click buttons
- **Location**: Inside dialogue box

### 3. **Original DecisionManager Buttons** (Still exists)
- **Used for**: Yes/No ID verification decisions
- **Shows**: "(Q) Yes" and "(E) No"
- **Trigger**: Press Q or E
- **Location**: Separate decision panel

---

## Testing Checklist

- [ ] Created SingleButtonContainer inside DialoguePanel
- [ ] Created SingleButton_E with TextMeshPro text
- [ ] Assigned all fields in DialogueManager Inspector
- [ ] Robot is tagged "Robot" or has movement component
- [ ] Robot has collider
- [ ] Look at robot → "(E) Interact" appears
- [ ] Press E → Dialogue shows "Would you like to follow this robot?"
- [ ] Single button "(E) Follow this robot" appears
- [ ] Press E or click → Proceeds to ID verification
- [ ] Button supports both keyboard and mouse

---

## Next Steps

Once this works:
1. You can customize the button text/styling
2. You can add different messages for different robots
3. You can add animations to button appearance
4. You can disable/remove the old DecisionManager system in ID_Scene if desired

---

## Quick Reference

**Show single button in dialogue:**
```csharp
dialogueManager.ShowRobotInteractionButton("RobotName");
```

**Show double buttons in dialogue:**
```csharp
dialogueManager.ShowInlineButtons("Option 1", "Option 2", callback1, callback2);
```

**Hide buttons:**
```csharp
dialogueManager.HideSingleButton();
dialogueManager.HideInlineButtons();
```
