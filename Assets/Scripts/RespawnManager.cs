using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
   private Vector3 lastCheckpointPos;
    private Quaternion lastCheckpointRot;

    [SerializeField] private float respawnHeightOffset = 5f; // lift car up

    void Start()
    {
        // Default respawn = starting line
        lastCheckpointPos = transform.position;
        lastCheckpointRot = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Save checkpoint when passing through
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpointPos = other.transform.position;
            lastCheckpointRot = other.transform.rotation;
        }

        // Respawn when hitting boundary
        if (other.CompareTag("Boundary"))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Add vertical offset so car respawns above ground
        Vector3 respawnPos = lastCheckpointPos + Vector3.up * respawnHeightOffset;

        transform.SetPositionAndRotation(respawnPos, lastCheckpointRot);
    }
}
