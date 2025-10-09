# Complete UI Setup Guide

This guide covers setting up the crosshair, robot interaction system, and inline dialogue buttons.

---

## Part 1: Crosshair Setup

### Step 1: Create Canvas
1. In Unity Hierarchy, right-click → **UI → Canvas**
2. The Canvas should use "Screen Space - Overlay" mode (default)

### Step 2: Create Crosshair
1. Right-click on Canvas → **Create Empty**
2. Rename it to **"Crosshair"**
3. In Inspector, click **Add Component**
4. Search for and add **"Crosshair"** script
5. Configure in Inspector:
   - **Dot Size**: 4 (default) - adjust to your preference
   - **Dot Color**: White (default) - change if needed

**Result**: A small dot will appear at the center of the screen when you play.

---

## Part 2: Robot Interaction System

### Step 1: Create Interaction Prompt Panel
1. Right-click on Canvas → **UI → Panel**
2. Rename it to **"InteractionPrompt"**
3. In RectTransform, set anchors to **bottom-center**:
   - Anchor Presets: Bottom-Center
   - Pos Y: 100 (adjust to position above bottom of screen)
4. Resize panel to be small (e.g., 300x80)

### Step 2: Add Text to Interaction Prompt
1. Right-click on InteractionPrompt → **UI → Text - TextMeshPro**
2. Rename it to **"PromptText"**
3. Set text to center alignment
4. Adjust font size (e.g., 24)

### Step 3: Create Robot Interaction Manager
1. In Hierarchy, right-click → **Create Empty**
2. Rename it to **"RobotInteractionManager"**
3. Click **Add Component**
4. Add the **"RobotInteraction"** script

### Step 4: Configure Robot Interaction in Inspector
1. Select RobotInteractionManager
2. In the RobotInteraction component:
   - **Interaction Range**: 5 (how close player must be)
   - **Robot Layer**: Leave as "Everything" or create a Robot layer
   - **Interaction Prompt**: Drag InteractionPrompt GameObject here
   - **Prompt Text**: Drag PromptText (the TextMeshPro component) here
   - **Decision Manager**: Leave empty (auto-finds) or drag your DecisionManager

### Step 5: Tag Your Robots
1. Select all robot GameObjects in your scene
2. In Inspector at the top, click **Tag → Add Tag**
3. Create a new tag called **"Robot"**
4. Go back and select each robot
5. Set Tag to **"Robot"**

**Alternatively**: If your robots have `SimpleRobotMovement` or `RobotNavMeshMovement` components, they'll be detected automatically without tags.

**Result**: When you look at a robot through the crosshair, "(E) Interact" appears. Press E to interact.

---

## Part 3: Inline Dialogue Buttons

### Step 1: Locate Your Dialogue Panel
1. Find your existing **DialoguePanel** GameObject in the Canvas
2. Make sure it has a TMPro Text component for dialogue text

### Step 2: Create Button Container
1. Right-click on DialoguePanel → **Create Empty**
2. Rename it to **"InlineButtonContainer"**
3. In Inspector, click **Add Component** → **Layout Group → Horizontal Layout Group**
4. Configure Horizontal Layout Group:
   - **Spacing**: 20
   - **Child Alignment**: Middle Center
   - **Child Force Expand**: Width ✓, Height ✓
   - **Padding**: Left 10, Right 10, Top 10, Bottom 10

### Step 3: Position the Button Container
1. Select InlineButtonContainer
2. In RectTransform:
   - **Anchors**: Stretch horizontal, bottom anchor
   - Set **Height**: 60-80 pixels
   - Position at bottom of dialogue panel

### Step 4: Create Button 1 (Q)
1. Right-click on InlineButtonContainer → **UI → Button - TextMeshPro**
2. Rename it to **"Button1_Q"**
3. If it has a child Text object (not TextMeshPro):
   - Delete the old Text child
   - Right-click on Button1_Q → **UI → Text - TextMeshPro**
4. Rename the TextMeshPro child to **"Button1Text"**
5. Configure Button1Text:
   - **Alignment**: Center
   - **Font Size**: 18-20
   - **Text**: Leave empty (script fills it in)

### Step 5: Create Button 2 (E)
1. Right-click on InlineButtonContainer → **UI → Button - TextMeshPro**
2. Rename it to **"Button2_E"**
3. If it has a child Text object (not TextMeshPro):
   - Delete the old Text child
   - Right-click on Button2_E → **UI → Text - TextMeshPro**
4. Rename the TextMeshPro child to **"Button2Text"**
5. Configure Button2Text same as Button1Text

### Step 6: Style the Buttons (Optional)
1. Select Button1_Q
2. In Image component, change **Color** to your theme
3. In Button component, configure **Transition** colors
4. Repeat for Button2_E

### Step 7: Create Single Button (For Robot Interaction)

1. **Create Single Button Container**:
   - Right-click on DialoguePanel → **Create Empty**
   - Rename it to **"SingleButtonContainer"**

2. **Create Single Button**:
   - Right-click on SingleButtonContainer → **UI → Button - TextMeshPro**
   - Rename it to **"SingleButton_E"**
   - If it has a child Text object (not TextMeshPro):
     - Delete the old Text child
     - Right-click on SingleButton_E → **UI → Text - TextMeshPro**
   - Rename the TextMeshPro child to **"SingleButtonText"**

3. **Position the Single Button**:
   - Select SingleButtonContainer
   - In RectTransform:
     - **Anchors**: Center horizontal, bottom anchor
     - Set **Height**: 60-80 pixels
     - Position at bottom center of dialogue panel

### Step 8: Configure DialogueManager
1. Find your **DialogueManager** GameObject
2. In Inspector, under "Inline Decision Buttons" section:
   - **Inline Button Container**: Drag InlineButtonContainer here
   - **Inline Button 1**: Drag Button1_Q here
   - **Inline Button 2**: Drag Button2_E here
   - **Inline Button 1 Text**: Drag Button1Text (TextMeshPro component) here
   - **Inline Button 2 Text**: Drag Button2Text (TextMeshPro component) here

3. Under "Single Button" section:
   - **Single Button Container**: Drag SingleButtonContainer here
   - **Single Button**: Drag SingleButton_E here
   - **Single Button Text**: Drag SingleButtonText (TextMeshPro component) here

**Result**:
- Q/E choice buttons appear for work/wait decisions
- Single E button appears for robot interaction

---

## Testing Your Setup

### Test Crosshair
1. Press Play
2. You should see a small white dot in the center of the screen

### Test Robot Interaction
1. Press Play (in ID_Scene)
2. Look at a robot through the crosshair
3. "(E) Interact" should appear near the bottom of screen
4. Press E
5. Dialogue box should show: "Would you like to follow this robot?"
6. A single button "(E) Follow this robot" should appear in the dialogue box
7. Press E or click the button to confirm

### Test Inline Buttons
1. Play through to the Waiting Scene
2. After "I really need to get to work..." dialogue
3. Two buttons should appear inside the dialogue box:
   - (Q) Ask about leaving for work
   - (E) Stay put and wait
4. Click a button OR press Q/E on keyboard

---

## Troubleshooting

### Crosshair doesn't appear
- Check that Crosshair script is attached
- Check that Canvas is set to "Screen Space - Overlay"
- Check that Crosshair GameObject is active

### Interaction prompt doesn't show
- Verify robots have "Robot" tag OR SimpleRobotMovement/RobotNavMeshMovement component
- Check that InteractionPrompt and PromptText are assigned in RobotInteraction
- Check interaction range (try increasing to 10)
- Make sure RobotInteractionManager GameObject is active

### Inline buttons don't appear
- Verify all fields in DialogueManager "Inline Decision Buttons" are assigned
- Check that InlineButtonContainer is a child of DialoguePanel
- Make sure Button1Text and Button2Text are TextMeshPro (not regular Text)
- Check Console for errors

### Buttons appear but don't work
- Verify button OnClick listeners are cleared (script handles this)
- Check that both Q/E keyboard input AND button clicks work
- Look for errors in Console when clicking

---

## File Structure

After setup, your Canvas hierarchy should look like:

```
Canvas
├── Crosshair
├── InteractionPrompt
│   └── PromptText (TextMeshPro)
└── DialoguePanel
    ├── DialogueText (TextMeshPro)
    └── InlineButtonContainer
        ├── Button1_Q
        │   └── Button1Text (TextMeshPro)
        └── Button2_E
            └── Button2Text (TextMeshPro)
```

And in your scene root:
```
RobotInteractionManager
```

---

## Summary

✅ **Crosshair**: Always visible at screen center
✅ **Robot Interaction**: Shows "(E) Interact" when looking at robots
✅ **Inline Buttons**: Q/E choices appear inside dialogue box
✅ **Keyboard + Mouse**: Both input methods supported

**Next Steps**: Test in your scenes and adjust styling/positioning to match your game's aesthetic!
