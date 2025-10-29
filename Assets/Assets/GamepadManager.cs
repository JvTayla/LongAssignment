using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GamepadManager : MonoBehaviour
{
    public GameObject kart1;

    public Gamepad[] connectedGamepads = new Gamepad[4];

    // Start is called before the first frame update
    void Start()
    {
        // Example: Assign the first connected gamepad to the first slot in the array
        if (Gamepad.current != null)
        {
            connectedGamepads[0] = Gamepad.current;
            Debug.Log($"Gamepad 1 assigned: {connectedGamepads[0].displayName}");
        }
        else
        {
            Debug.Log("No gamepad currently connected.");
        }
    }

    void Awake()
    {
        int gamepadIndex = 0;
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad gamepad && gamepadIndex < connectedGamepads.Length)
            {
                connectedGamepads[gamepadIndex] = gamepad;
                Debug.Log($"Gamepad {gamepadIndex + 1} assigned: {gamepad.displayName}");
                gamepadIndex++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
