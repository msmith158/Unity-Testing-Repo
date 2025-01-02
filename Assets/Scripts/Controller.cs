using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private Mitchel.UISystems.DialogueSystem dialogueSystem;
    void Update()
    {
        var gamepad = Gamepad.current; // Get the currently connected gamepad

        if (gamepad != null)
        {
            // --- Buttons ---
            if (gamepad.buttonSouth.wasPressedThisFrame) { Debug.Log("Button South (A / Cross) pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.buttonEast.wasPressedThisFrame) { Debug.Log("Button East (B / Circle) pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.buttonWest.wasPressedThisFrame) { Debug.Log("Button West (X / Square) pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.buttonNorth.wasPressedThisFrame) { Debug.Log("Button North (Y / Triangle) pressed"); dialogueSystem.ChangeGlyphs(1); }

            if (gamepad.leftShoulder.isPressed) { Debug.Log("Left Shoulder (LB / L1) pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.rightShoulder.isPressed) { Debug.Log("Right Shoulder (RB / R1) pressed"); dialogueSystem.ChangeGlyphs(1); }

            if (gamepad.startButton.isPressed) { Debug.Log("Start / Options button pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.selectButton.isPressed) { Debug.Log("Select / Share button pressed"); dialogueSystem.ChangeGlyphs(1); }

            if (gamepad.leftStickButton.isPressed) { Debug.Log("Left Stick Button (L3) pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.rightStickButton.isPressed) { Debug.Log("Right Stick Button (R3) pressed"); dialogueSystem.ChangeGlyphs(1); }

            if (gamepad.dpad.up.isPressed) { Debug.Log("D-Pad Up pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.dpad.down.isPressed) { Debug.Log("D-Pad Down pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.dpad.left.isPressed) { Debug.Log("D-Pad Left pressed"); dialogueSystem.ChangeGlyphs(1); }
            if (gamepad.dpad.right.isPressed) { Debug.Log("D-Pad Right pressed"); dialogueSystem.ChangeGlyphs(1); }

            // --- Axes ---
            Vector2 leftStick = gamepad.leftStick.ReadValue(); // Left stick (X, Y)
            Vector2 rightStick = gamepad.rightStick.ReadValue(); // Right stick (X, Y)

            float leftTrigger = gamepad.leftTrigger.ReadValue(); // Left trigger (0 to 1)
            float rightTrigger = gamepad.rightTrigger.ReadValue(); // Right trigger (0 to 1)

            // Log axes
            //Debug.Log($"Left Stick: {leftStick.x}, {leftStick.y}");
            //Debug.Log($"Right Stick: {rightStick.x}, {rightStick.y}");
            //Debug.Log($"Triggers: Left = {leftTrigger}, Right = {rightTrigger}");
        }
        else
        {
            if (Input.anyKeyDown)
            {
                dialogueSystem.ChangeGlyphs(0);
            }
            Debug.LogWarning("No gamepad connected.");
        }
    }
}
