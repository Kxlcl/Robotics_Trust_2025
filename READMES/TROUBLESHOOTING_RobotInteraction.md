# Troubleshooting: Robot Interaction Not Working

If you're seeing the original Q/E buttons but the "(E) Interact" prompt doesn't appear when looking at robots, follow these steps.

---

## Step 1: Check the Console for Debug Info

1. Run the game in Unity
2. Open the Console window (Window → General → Console)
3. Look for the debug output that starts with `=== RobotInteraction Debug Info ===`

### What to Check:

```
=== RobotInteraction Debug Info ===
RobotInteraction initialized in scene: ID_Scene
Player Camera found: True - Main Camera
Interaction Prompt assigned: True        ← Should be True
Prompt Text assigned: True               ← Should be True
Decision Manager found: True
Interaction Range: 5
Found X objects with 'Robot' tag         ← Should be > 0
  - Robot: [robot names]
Found X SimpleRobotMovement components
Found X RobotNavMeshMovement components
===================================
```

---

## Step 2: Fix Missing References

### If "Interaction Prompt assigned: False"

**Problem**: The InteractionPrompt UI element isn't assigned.

**Solution**:
1. Select the RobotInteractionManager GameObject
2. In Inspector, find the RobotInteraction component
3. Under "UI Elements":
   - Drag your InteractionPrompt Panel to the **Interaction Prompt** field
   - Drag the TextMeshPro text component to the **Prompt Text** field

### If "Prompt Text assigned: False"

**Problem**: The text component isn't assigned.

**Solution**:
1. Make sure you created a TextMeshPro text (not regular Text)
2. Drag the TextMeshPro component (not the GameObject) to Prompt Text field

---

## Step 3: Fix Robot Detection

### If "Found 0 objects with 'Robot' tag"

**Problem**: Robots aren't tagged.

**Solution**:
1. Select each robot GameObject in the scene
2. At the top of Inspector, click the **Tag** dropdown
3. If "Robot" isn't listed:
   - Click **Add Tag...**
   - Click the **+** button
   - Type **Robot**
   - Click **Save**
4. Go back to each robot and set Tag to **Robot**

### Alternative: Use Robot Components Instead

If you don't want to use tags, make sure your robots have one of these components:
- `SimpleRobotMovement` component
- `RobotNavMeshMovement` component

The script will detect these automatically.

---

## Step 4: Check Colliders

### Problem: Raycast might not hit robots

**Check if robots have colliders**:
1. Select a robot in the scene
2. In Inspector, look for a **Collider** component (BoxCollider, SphereCollider, CapsuleCollider, etc.)
3. If missing, add one:
   - Click **Add Component**
   - Search for **Box Collider** (or appropriate collider type)
   - Adjust size to fit the robot

**Make sure colliders are enabled**:
- Check that the checkbox next to the Collider component is ✓ checked

---

## Step 5: Test Raycast Detection

1. Run the game
2. Look at a robot through the crosshair
3. Check the Console for raycast output

### You should see:
```
Raycast hit: [object name] at distance [X]
  - Tag: Robot
  - Has SimpleRobotMovement: True/False
  - Has RobotNavMeshMovement: True/False
>>> ROBOT DETECTED: [robot name]
```

### If you see "NOT recognized as a robot":
- The object has no "Robot" tag
- The object has no robot movement component
- Solution: Add tag or component

### If you see nothing at all:
- Raycast isn't hitting anything
- Possible issues:
  1. Robot has no collider
  2. Robot is too far (increase Interaction Range)
  3. Camera isn't set up correctly

---

## Step 6: Check Interaction Range

**Problem**: Robots might be too far away.

**Solution**:
1. Select RobotInteractionManager
2. In RobotInteraction component, increase **Interaction Range**
3. Try 10 or 15 instead of 5
4. Test again

---

## Step 7: Check Scene Setup

### Which scene are you in?

The RobotInteraction system is designed for **ID_Scene** (initial robot selection).

**If you're in Waiting_Scene**:
- The robot interaction might not be needed there
- The original Q/E system might be intentional for that scene

**To confirm**:
- Check console log: `RobotInteraction initialized in scene: [scene name]`

---

## Step 8: Disable Original Q/E System (If Needed)

If you want to **replace** the original DecisionManager Q/E system with the new hover-to-interact system:

### Option A: Only use hover system in ID_Scene

Add this to DialogueManager.cs in the `ShowIDSceneDialogue()` method:

```csharp
void ShowIDSceneDialogue()
{
    dialoguePanel.SetActive(true);
    dialogueText.text = "Choose a robot to follow.";
    Debug.Log("Showing ID_Scene dialogue: Choose a robot to follow");

    // DON'T trigger decision system automatically
    // Wait for player to use E on a robot instead
    // Comment out or remove this line:
    // StartCoroutine(TriggerDecisionAfterDelay());
}
```

### Option B: Let both systems work together

The original system shows Q/E choices AFTER you interact with a robot using E. This is fine if you want:
1. Hover over robot → "(E) Interact" appears
2. Press E → Q/E robot selection appears
3. Press Q or E → Choose which robot to follow

---

## Step 9: Visual Debug Ray

Add this to help visualize the raycast:

```csharp
void CheckForRobot()
{
    Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    RaycastHit hit;

    // Draw the ray in Scene view
    Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 0.1f);

    // ... rest of the code
}
```

Then in Unity:
1. Click the **Scene** tab while playing
2. Look for a red line from the camera
3. This shows where the raycast is going

---

## Common Issues Summary

| Issue | Check | Fix |
|-------|-------|-----|
| No interaction prompt | Console: "Interaction Prompt assigned: False" | Assign InteractionPrompt in Inspector |
| Prompt never shows | Console: "Found 0 objects with 'Robot' tag" | Tag robots or add movement components |
| Only works very close | Interaction Range too small | Increase range to 10-15 |
| Raycast hits wrong object | Robot has no collider | Add collider to robot |
| Both systems active | Both prompts show | Decide which system to use, disable the other |

---

## Quick Checklist

Run through this checklist:

- [ ] RobotInteractionManager GameObject exists in scene
- [ ] RobotInteraction script is attached
- [ ] InteractionPrompt Panel is created and assigned
- [ ] PromptText (TextMeshPro) is created and assigned
- [ ] Robots have "Robot" tag OR movement component
- [ ] Robots have colliders
- [ ] Camera is found (check console)
- [ ] Interaction Range is adequate (5-15)
- [ ] In the correct scene (ID_Scene)
- [ ] Console shows robot detection when looking at them

---

## Still Not Working?

**Copy these console logs and share them:**

1. The output from `=== RobotInteraction Debug Info ===`
2. Any errors in red in the Console
3. The output when you look at a robot (raycast logs)

**Include this info:**
- Which scene you're testing in
- Names of your robot GameObjects
- Whether robots have colliders
- Whether you assigned all fields in Inspector

This will help diagnose the exact issue!
