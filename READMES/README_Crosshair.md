# Crosshair & Robot Interaction System

A crosshair and raycast-based interaction system for interacting with robots in the game.

## Setup Instructions

### Part 1: Crosshair Setup

1. **Create a Canvas** (if you don't already have one):
   - Right-click in Hierarchy → UI → Canvas

2. **Create a Crosshair GameObject**:
   - Right-click on the Canvas → Create Empty
   - Name it "Crosshair"

3. **Add the Crosshair Script**:
   - Select the Crosshair GameObject
   - Add the `Crosshair.cs` script component

### Part 2: Robot Interaction Setup

1. **Create an Interaction Prompt**:
   - Right-click on the Canvas → UI → Panel
   - Name it "InteractionPrompt"
   - Position it where you want the "(E) Interact" text to appear (e.g., bottom center)
   - Add a TextMeshPro Text as a child to display the prompt text

2. **Add Robot Interaction Script**:
   - Create an empty GameObject in your scene (or add to an existing manager object)
   - Name it "RobotInteractionManager"
   - Add the `RobotInteraction.cs` script component

3. **Configure Robot Interaction in Inspector**:
   - **Interaction Range**: How far the player can interact with robots (default: 5)
   - **Robot Layer**: Set the layer mask to detect robots (optional, can also use tags)
   - **Interaction Prompt**: Drag the InteractionPrompt Panel here
   - **Prompt Text**: Drag the TextMeshPro Text from inside the InteractionPrompt
   - **Decision Manager**: Will auto-find, or drag your DecisionManager here

4. **Tag Your Robots**:
   - Select all robot GameObjects in your scene
   - In the Inspector, set Tag to "Robot" (create the tag if it doesn't exist)
   - Alternatively, robots will be detected if they have `SimpleRobotMovement` or `RobotNavMeshMovement` components

## Customization

### Crosshair
- **Dot Size**: Size of the crosshair dot in pixels (default: 4)
- **Dot Color**: Color of the crosshair dot (default: white)

### Robot Interaction
- **Interaction Range**: How close you need to be to interact with robots
- **Robot Layer**: Use layer masks for more precise detection

## How It Works

1. **Crosshair**: Creates a small circular UI dot positioned at the center of the screen
2. **Robot Detection**: Constantly raycasts from the center of the camera to detect robots
3. **Interaction Prompt**: When the crosshair hovers over a robot, "(E) Interact" appears
4. **Interaction**: Press E while looking at a robot to trigger the decision system (replaces the old Q/E system)
