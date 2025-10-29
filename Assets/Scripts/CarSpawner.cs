using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject npcCarPrefab; // Assign your NPC car prefab
    public int numberOfCars = 5; // Control the number of cars here
    public Transform waypointManager; // Assign the GameObject with NPCWAYPOINT script attached
    public Vector3 spawnPosition = Vector3.zero; // Base spawn position (e.g., before first checkpoint)
    public Vector3 spawnOffset = new Vector3(3f, 0f, 0f); // Offset per car to avoid overlap (e.g., side-by-side)

    [Header("Car Settings")]
    public float initialSpeedVariation = 1f; // Optional: Speed multiplier for all cars (1 = default random)

    void Start()
    {
        if (npcCarPrefab == null)
        {
            Debug.LogError("Assign an NPC car prefab to CarSpawner!");
            return;
        }

        if (waypointManager == null)
        {
            Debug.LogError("Assign the GameObject with NPCWAYPOINT to CarSpawner!");
            return;
        }

        // Get your custom NPCWAYPOINT component
        NPCWAYPOINT wm = waypointManager.GetComponent<NPCWAYPOINT>();
        if (wm == null)
        {
            Debug.LogError("The assigned waypointManager GameObject does not have an NPCWAYPOINT script!");
            return;
        }

        // Assuming NPCWAYPOINT has a public Transform[] waypoints field
        // If your field is named differently (e.g., checkpoints), change this line accordingly
        if (wm.waypoints == null || wm.waypoints.Length == 0)
        {
            Debug.LogError("NPCWAYPOINT has no waypoints assigned!");
            return;
        }

        Transform[] waypoints = wm.waypoints;

        for (int i = 0; i < numberOfCars; i++)
        {
            // Calculate staggered spawn position
            Vector3 spawnPos = spawnPosition + (spawnOffset * i);

            // Instantiate car
            GameObject newCar = Instantiate(npcCarPrefab, spawnPos, Quaternion.identity);
            newCar.name = "NPC Car " + (i + 1);

            // Assign waypoints and speed variation
            NPCController ai = newCar.GetComponent<NPCController>();
            if (ai != null)
            {
                ai.waypoints = waypoints;
                ai.speedVariation = initialSpeedVariation;
            }
            else
            {
                Debug.LogError("NPC car prefab missing NPCController script!");
            }

            // Optional: Face forward (along Z-axis, adjust if your track is oriented differently)
            newCar.transform.rotation = Quaternion.Euler(0, 90f * i % 180f, 0); // Simple stagger rotation
        }

        Debug.Log("Spawned " + numberOfCars + " NPC cars using NPCWAYPOINT.");
    }
}