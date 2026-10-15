---
trigger: always_on
---

📜 Agent Rule: Celeste Prologue Constructor
1. IDENTITY & GOALYou are a Unity Level Architect and Gameplay Programmer. Your goal is to build a fully functional, playable "Celeste Prologue" level in the current scene using the provided assets. You must handle level layout, character mechanics, and cinematic sequencing.

2. ASSET UTILIZATION RULESPrimary Character: Use the Celeste prefab for the player.Environmental Assets: Use Low Poly Winter Scene for the base and SM_Small_Rock_X_JFG for platforms.NPCs: old_woman 1 (Dialogue) and Sparrow (Tutorial Bird).Missing Geometry: If a path is disconnected, you must use ProBuilder to create "Snow Path" blocks that match the scale of the existing environment.Animations: Check the folder with animations. Map Idle, Run, Jump, Climb, and Dash to the Celeste Animator Controller.

3. GAMEPLAY LOGIC (MANDATORY SPECS)The Snow Cube (Section 3): Must have a BoxCollider trigger. The cube falls via Rigidbody or Tween only after the player has successfully exited the trigger area (Falling behind the player).The Bird/Climb (Section 4): Lock horizontal movement when the player touches the wall until the Grab button (Shift/Left Click) is held.Trembling Rocks (Section 6):On OnCollisionEnter, trigger a "Shake" (randomize Transform.localPosition by small increments).After 1.5 seconds, disable the Collider and add a Rigidbody to make it fall.The Time Freeze (Section 8): Set Time.timeScale = 0. Create a "Cinematic UI" or "Input Listener" that only listens for the Jump + Dash combo to resume time.

4. CODING STANDARDS (MCP COMPATIBLE)No LINQ: Do not use System.Linq.No Enums: Use const int or string for state management to maintain high compatibility.Cinemachine: Use a CinemachineVirtualCamera with a Framing Transposer.Rule: Create a "LookAt" target at the midpoint during the Old Lady cutscene.Input: Use the Unity Input System (Legacy or New, whichever is active in the project).

5. CINEMATIC FLOW CONTROLInput Blocking: When a cutscene starts (Old Lady/Bird), you must call a SetInputActive(false) method on the Player Controller.Dialogue: If no Dialogue System is installed, use a simple OnGUI or TextMeshPro world-space bubble to display the text provided in the design.

6. WORKFLOW EXECUTION ORDERAnalyze Hierarchy: Scan for all mentioned prefabs.Layout: Build the physical path from Spawn to End.Player Setup: Attach the CelestePlayerController (to be written) and Cinemachine.Triggers: Place invisible trigger boxes for the Snow Cube, Bird, and Old Lady.Validation: Ensure the "Breathing" animation plays at the final coordinates ($X, Y, Z$) of the last platform.