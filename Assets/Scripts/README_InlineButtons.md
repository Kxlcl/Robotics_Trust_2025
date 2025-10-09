# Inline Dialogue Buttons Setup

The dialogue system now supports showing Q/E choice buttons directly inside the dialogue box.

## Setup Instructions

### 1. Create Button Container in Dialogue Panel

1. Open your scene with the Dialogue Panel
2. Find your existing Dialogue Panel GameObject
3. Right-click on the Dialogue Panel → UI → Vertical Layout Group (or create an empty GameObject)
4. Name it "InlineButtonContainer"

### 2. Create Two Buttons

1. Right-click on InlineButtonContainer → UI → Button
2. Name the first button "Button1_Q"
3. Right-click on InlineButtonContainer → UI → Button
4. Name the second button "Button2_E"

### 3. Add TextMeshPro Text to Each Button

1. Select Button1_Q
2. Delete the default Text child if it has one
3. Right-click on Button1_Q → UI → Text - TextMeshPro
4. Name it "Button1Text"
5. Repeat for Button2_E, creating "Button2Text"

### 4. Configure DialogueManager in Inspector

1. Find your DialogueManager GameObject
2. In the Inspector, under "Inline Decision Buttons":
   - **Inline Button Container**: Drag the InlineButtonContainer here
   - **Inline Button 1**: Drag Button1_Q here
   - **Inline Button 2**: Drag Button2_E here
   - **Inline Button 1 Text**: Drag Button1Text here
   - **Inline Button 2 Text**: Drag Button2Text here

### 5. Position and Style

1. Position InlineButtonContainer at the bottom of your Dialogue Panel
2. Style the buttons as desired (colors, size, spacing)
3. The buttons will automatically hide when not needed
4. Button texts will show "(Q) Option 1" and "(E) Option 2" format

## How It Works

- Buttons appear inline within the dialogue box after certain dialogue lines
- Players can click the buttons OR press Q/E keys
- Used for:
  - Work/Wait decisions in Waiting Scene
  - Robot selection decisions
  - Future Yes/No decisions
- Buttons automatically hide after selection

## Features

- **Keyboard Support**: Press Q or E to select options
- **Mouse Support**: Click buttons directly
- **Auto-hide**: Buttons only show when choices are available
- **Integrated**: Part of the dialogue panel, not separate UI
