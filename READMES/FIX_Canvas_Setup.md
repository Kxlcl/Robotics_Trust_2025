# Fix Canvas/UI Setup for Inline Buttons

The buttons are being created and activated correctly in code, but not visible due to Canvas/UI configuration issues.

---

## Step 1: Check Canvas Settings

1. **Stop the game**
2. In Hierarchy, find your **Canvas** (the root Canvas containing DialoguePanel)
3. Select it
4. In Inspector, check **Canvas** component:

### Required Settings:
- **Render Mode**: `Screen Space - Overlay` (NOT World Space or Camera)
- **Pixel Perfect**: Unchecked (usually)
- **Sort Order**: 100 (or any high number to render on top)
- **Target Display**: Display 1

### Canvas Scaler Component:
- **UI Scale Mode**: `Scale With Screen Size`
- **Reference Resolution**: 1920 x 1080 (or your target resolution)
- **Screen Match Mode**: Match Width Or Height
- **Match**: 0.5

### Graphic Raycaster Component:
- Should be present
- **Ignore Reversed Graphics**: Checked
- **Blocking Objects**: None

---

## Step 2: Check DialoguePanel Settings

1. Select **DialoguePanel** in Hierarchy
2. Check **RectTransform**:
   - **Width**: Should be large (e.g., 800-1200)
   - **Height**: Should be large (e.g., 400-600)
   - **Anchors**: Centered or stretched
   - Make sure it's big enough to contain the buttons

3. If DialoguePanel has an **Image** component:
   - **Color**: Make sure alpha is > 0 if you want to see the background
   - **Raycast Target**: Can be checked or unchecked

---

## Step 3: Check InlineButtonContainer

1. Select **InlineButtonContainer**
2. **RectTransform**:
   - **Anchors**: Bottom center (or wherever you want buttons)
   - **Width**: 600+ (wide enough for both buttons)
   - **Height**: 100+ (tall enough for buttons)
   - **Pos X**: 0 (centered)
   - **Pos Y**: -100 to -200 (near bottom of DialoguePanel)

3. **Layout Component** (if any):
   - If you have **Horizontal Layout Group**:
     - **Child Force Expand**: Width ✓, Height ✓
     - **Child Control Size**: Width ✓, Height ✓
     - **Spacing**: 20-50
     - **Padding**: Set all to 10

4. **Canvas Group** (added by script):
   - **Alpha**: Should be 0 initially, then 1 when shown
   - **Interactable**: Should toggle with alpha
   - **Block Raycasts**: Should toggle with alpha

---

## Step 4: Check Button1_Q Settings

1. Select **Button1_Q**
2. **RectTransform**:
   - **Width**: 200-300
   - **Height**: 50-80
   - **Anchors**: Left (if in Horizontal Layout) or as needed
   - **Make sure the GameObject checkbox at top is CHECKED** ✓

3. **Image Component**:
   - **Source Image**: UI/Skin/UISprite or similar
   - **Color**: RGBA(255, 255, 255, 255) - Full white, full opacity
   - **Material**: None (or UI/Default)
   - **Raycast Target**: Checked ✓

4. **Button Component**:
   - **Interactable**: Checked ✓
   - **Transition**: Color Tint (recommended)
   - **Normal Color**: White or light gray
   - **Highlighted Color**: Lighter
   - **Pressed Color**: Darker
   - **Selected Color**: Similar to highlighted

5. **Expand Button1_Q** in Hierarchy to find the **Text child**
   - Select the **Text (TMP)** child
   - **TextMeshPro - Text Component**:
     - **Text**: "(Q) Yes" (will be set by script)
     - **Font**: Any visible font
     - **Font Size**: 24-36
     - **Color**: RGBA(0, 0, 0, 255) - Black or RGBA(255, 255, 255, 255) - White
     - **Alignment**: Center
     - **Wrapping**: Disabled
     - **Overflow**: Overflow

---

## Step 5: Repeat for Button2_E

Same settings as Button1_Q

---

## Step 6: Test Visibility Manually

**While NOT playing:**

1. Select **InlineButtonContainer**
2. In Inspector, make sure it's **active** (checkbox checked)
3. Select **Button1_Q** - make sure **active**
4. Select **Button2_E** - make sure **active**
5. In **Scene view**, do you see the buttons?
6. If you see them in Scene view, they should work in Game view

**Press Play:**
- Buttons should be invisible at start (CanvasGroup alpha = 0)
- After pressing E on robot, they should become visible (alpha = 1)

---

## Step 7: Alternative - Recreate the Buttons from Scratch

If nothing works, recreate them properly:

1. **Delete** InlineButtonContainer and all its children
2. Right-click on **DialoguePanel** → **Create Empty**
3. Rename to **InlineButtonContainer**
4. Set RectTransform:
   - Anchors: Bottom-center
   - Width: 600
   - Height: 100
   - Pos Y: -150
5. Add Component → **Horizontal Layout Group**:
   - Spacing: 30
   - Child Force Expand: Width ✓, Height ✓
   - Child Control Size: Width ✓, Height ✓
6. Right-click InlineButtonContainer → **UI → Button - TextMeshPro**
7. Rename to **Button1_Q**
8. In Button1_Q Image component:
   - Color: White (255, 255, 255, 255)
9. Find Text child, set:
   - Font Size: 30
   - Color: Black (0, 0, 0, 255)
   - Alignment: Center
10. **Duplicate** Button1_Q (Ctrl+D)
11. Rename duplicate to **Button2_E**
12. **Assign in DialogueManager Inspector**:
    - Inline Button Container → InlineButtonContainer
    - Inline Button 1 → Button1_Q
    - Inline Button 2 → Button2_E
    - Inline Button 1 Text → Button1_Q/Text (TMP)
    - Inline Button 2 Text → Button2_E/Text (TMP)

---

## Step 8: Nuclear Option - Create Test Scene

If STILL not working, create a minimal test:

1. **File → New Scene**
2. Right-click Hierarchy → **UI → Canvas**
3. Right-click Canvas → **UI → Panel**
4. Right-click Panel → **Create Empty** (name it TestContainer)
5. Right-click TestContainer → **UI → Button - TextMeshPro**
6. Press Play
7. **Do you see the button?**

If YES → Your main scene has a configuration issue
If NO → Your Unity UI system might be broken (reinstall Unity UI package)

---

## Common Issues Checklist

- [ ] Canvas Render Mode is Screen Space - Overlay
- [ ] Canvas is active (not disabled)
- [ ] DialoguePanel is active
- [ ] DialoguePanel is large enough to contain buttons
- [ ] InlineButtonContainer is inside DialoguePanel
- [ ] Button GameObjects are active (checked in Hierarchy)
- [ ] Button Image colors have alpha > 0
- [ ] Button Text colors have alpha > 0
- [ ] No other UI blocking the buttons (higher sort order)
- [ ] Camera can see the Canvas (for non-overlay modes)
- [ ] No CanvasGroup with alpha = 0 on parent (except the one we control)
- [ ] Buttons have proper size (not 0x0)

---

## Quick Debug Test

Add this to a new script and attach to any GameObject:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisibilityTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Find all buttons
            Button[] buttons = FindObjectsOfType<Button>(true);
            Debug.Log($"Found {buttons.Length} buttons in scene");

            foreach (Button btn in buttons)
            {
                Debug.Log($"Button: {btn.name}");
                Debug.Log($"  Active: {btn.gameObject.activeInHierarchy}");
                Debug.Log($"  Position: {btn.transform.position}");

                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    Debug.Log($"  Image Color: {img.color}");
                }

                CanvasGroup cg = btn.GetComponentInParent<CanvasGroup>();
                if (cg != null)
                {
                    Debug.Log($"  Parent CanvasGroup Alpha: {cg.alpha}");
                }
            }
        }
    }
}
```

Press Space while playing to dump all button info.

---

## If All Else Fails

Share screenshots of:
1. Canvas Inspector (with Canvas component visible)
2. DialoguePanel RectTransform
3. InlineButtonContainer Inspector
4. Button1_Q Inspector (showing Image and Button components)
5. Hierarchy showing the full Canvas structure

This will help diagnose the exact issue!
