using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWAYPOINT : MonoBehaviour
{
    [Header("Checkpoints")]
    public Transform[] waypoints; // Drag your empty checkpoint GameObjects here in the Inspector
    // Optional: Visualize waypoints in Scene view
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(waypoints[i].position, 1f);
            if (i < waypoints.Length - 1)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position); // Loop back
            }
        }
    }
}
