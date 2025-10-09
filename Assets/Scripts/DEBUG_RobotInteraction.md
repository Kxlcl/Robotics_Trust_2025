# Debug: Crosshair Hovering Over Robot Not Working

Follow these steps in order to diagnose why hovering over robots isn't working.

---

## Step 1: Verify RobotInteractionManager Exists

1. Open your scene (ID_Scene)
2. Look in the Hierarchy for **RobotInteractionManager** GameObject
3. **If it doesn't exist:**
   - Right-click in Hierarchy → Create Empty
   - Name it "RobotInteractionManager"
   - Add Component → RobotInteraction script

**Result:** RobotInteractionManager should be visible in Hierarchy

---

## Step 2: Check if Script is Enabled

1. Select RobotInteractionManager in Hierarchy
2. Look at Inspector
3. Find the **RobotInteraction** component
4. **Check the checkbox** next to the component name is ✓ checked
5. **If unchecked:** Click it to enable the script

**Result:** RobotInteraction component should be enabled (checkbox checked)

---

## Step 3: Check Console Logs on Play

1. Press Play
2. Open Console (Window → General → Console)
3. **Look for this output:**

```
=== RobotInteraction Debug Info ===
RobotInteraction initialized in scene: ID_Scene
Player Camera found: True - Main Camera
Interaction Prompt assigned: True
Prompt Text assigned: True
Decision Manager found: True
Interaction Range: 5
Found X objects with 'Robot' tag
  - Robot: [robot names]
===================================
```

### What Each Line Means:

| Line | What It Means | If False/0 |
|------|---------------|------------|
| Player Camera found: True | Camera detected | **FIX:** Check Step 4 |
| Interaction Prompt assigned: True | UI prompt set up | **FIX:** Check Step 5 |
| Prompt Text assigned: True | Text component set up | **FIX:** Check Step 5 |
| Found X objects with 'Robot' tag | Robots detected | **FIX:** Check Step 6 |

---

## Step 4: Fix Camera Not Found

**If you see "Player Camera found: False":**

### Option A: Tag your camera as MainCamera
1. Find your camera in Hierarchy
2. Select it
3. In Inspector, set **Tag** to **MainCamera**

### Option B: Make sure camera exists
1. Check if there's a Camera in your scene
2. If not, create one: Right-click → Camera

---

## Step 5: Fix Interaction Prompt Not Assigned

**If you see "Interaction Prompt assigned: False" or "Prompt Text assigned: False":**

1. Select RobotInteractionManager
2. In Inspector, find RobotInteraction component
3. Under "UI Elements":

### If fields are empty:
- **Interaction Prompt**: Drag your InteractionPrompt Panel here
- **Prompt Text**: Drag the TextMeshPro Text component here

### If you haven't created the UI yet:
Go back to [README_UI_Setup.md](README_UI_Setup.md) Part 2 and create the InteractionPrompt UI

---

## Step 6: Fix Robots Not Detected

**If you see "Found 0 objects with 'Robot' tag":**

### Check 1: Do robots exist in the scene?
1. Look in Hierarchy for robot GameObjects
2. If none exist, you need to add robots to your scene

### Check 2: Are robots tagged?
1. Select a robot in Hierarchy
2. Look at top of Inspector
3. Check the **Tag** dropdown
4. **If tag is "Untagged":**
   - Click Tag dropdown
   - If "Robot" isn't listed:
     - Select **Add Tag...**
     - Click **+** button
     - Type "Robot"
     - Click Save
   - Go back to robot GameObject
   - Set Tag to **Robot**
5. Repeat for ALL robots in scene

### Check 3: Do robots have colliders?
1. Select a robot
2. In Inspector, look for **Collider** component
3. **If missing:**
   - Click Add Component
   - Search for "Box Collider" (or Capsule/Sphere)
   - Adjust size to fit robot model
4. **If exists but unchecked:** Check the box to enable it

---

## Step 7: Test with Verbose Logging

1. Open **RobotInteraction.cs** in your code editor
2. Find line 78: `// Debug.Log("RobotInteraction Update running");`
3. **Uncomment it** (remove the //)
4. Save the file
5. Go back to Unity and press Play
6. **Check Console:**
   - If you see "RobotInteraction Update running" spam → Script is working
   - If you DON'T see it → Script isn't running (check Step 2)

---

## Step 8: Test Raycast Detection

1. Press Play
2. Look around at different objects
3. **Check Console for raycast output:**

### If you see raycast logs:
```
Raycast hit: [object name] at distance X
  - Tag: [tag]
  - Has SimpleRobotMovement: True/False
```
✅ **Raycast is working!**

### If you see nothing:
❌ **Raycast isn't hitting anything**

**Possible causes:**
- Robot is too far (increase Interaction Range in Inspector)
- Robot has no collider
- Camera isn't set up correctly

---

## Step 9: Increase Interaction Range

1. Select RobotInteractionManager
2. In Inspector, find **Interaction Range**
3. Change from **5** to **15** or **20**
4. Press Play and try again

---

## Step 10: Check Scene Setup

### Are you in the right scene?
1. Look at top of Unity: Current scene name should show
2. **RobotInteraction works in ID_Scene**
3. If you're in a different scene, switch to ID_Scene

### Is the scene set up correctly?
1. Does the scene have:
   - Player/Camera
   - Robot GameObjects
   - Canvas with InteractionPrompt

---

## Step 11: Manual Test - Remove Debug Logs

The debug logs in `CheckForRobot()` might be too verbose. Let me create a version with less spam:

1. Open **RobotInteraction.cs**
2. Find the `CheckForRobot()` method around line 97
3. Look for these lines (93-109):
```csharp
// Debug what we hit
Debug.Log($"Raycast hit: {hit.collider.name} at distance {hit.distance}");
Debug.Log($"  - Tag: {hit.collider.tag}");
Debug.Log($"  - Has SimpleRobotMovement: {hit.collider.GetComponent<SimpleRobotMovement>() != null}");
Debug.Log($"  - Has RobotNavMeshMovement: {hit.collider.GetComponent<RobotNavMeshMovement>() != null}");
```

4. **Comment them out temporarily** by adding // in front:
```csharp
// // Debug what we hit
// Debug.Log($"Raycast hit: {hit.collider.name} at distance {hit.distance}");
// Debug.Log($"  - Tag: {hit.collider.tag}");
// Debug.Log($"  - Has SimpleRobotMovement: {hit.collider.GetComponent<SimpleRobotMovement>() != null}");
// Debug.Log($"  - Has RobotNavMeshMovement: {hit.collider.GetComponent<RobotNavMeshMovement>() != null}");
```

5. Keep only the important log:
```csharp
Debug.Log($">>> ROBOT DETECTED: {currentRobotInView.name}");
```

6. Save and test again

---

## Step 12: Visual Test - Add Debug Ray

Add this line to `CheckForRobot()` to visualize the raycast:

1. Open **RobotInteraction.cs**
2. Find line 99: `Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));`
3. Add this line right after:
```csharp
Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
```

4. Save the file
5. Press Play
6. Click on the **Scene** tab (next to Game tab)
7. You should see a **red line** shooting from camera
8. This shows where the raycast is going

---

## Quick Checklist

Run through this quickly:

- [ ] RobotInteractionManager GameObject exists in scene
- [ ] RobotInteraction script is attached and enabled (checkbox checked)
- [ ] Camera exists and is tagged MainCamera
- [ ] InteractionPrompt UI created and assigned
- [ ] PromptText (TextMeshPro) created and assigned
- [ ] At least one robot exists in scene
- [ ] Robots have "Robot" tag
- [ ] Robots have colliders
- [ ] Colliders are enabled
- [ ] Interaction Range is 5 or higher
- [ ] In ID_Scene (or appropriate scene)
- [ ] Console shows debug output when playing

---

## Common Issues & Fixes

| Problem | Fix |
|---------|-----|
| Nothing in console | Script isn't running - check enabled checkbox |
| "Camera found: False" | Tag camera as MainCamera |
| "Found 0 robots" | Tag robots with "Robot" tag |
| Raycast hits nothing | Add colliders to robots |
| Prompt never shows | Assign InteractionPrompt in Inspector |
| Works only very close | Increase Interaction Range |

---

## Still Not Working?

**Share these details:**

1. **Console output** from `=== RobotInteraction Debug Info ===`
2. **Any errors** in Console (in red)
3. **Scene name** you're testing in
4. **Robot GameObject names** from Hierarchy
5. **Screenshot** of RobotInteraction component in Inspector (showing all fields)
6. **Does the raycast log appear?** (when looking at robots)

This will help pinpoint the exact issue!

---

## Next Step After It Works

Once you see ">>> ROBOT DETECTED" in console:
1. The "(E) Interact" prompt should appear on screen
2. If prompt appears but you can't see it, check UI positioning
3. If prompt doesn't appear but console shows detection, check InteractionPrompt GameObject is active

**Then test pressing E to see if dialogue appears!**
