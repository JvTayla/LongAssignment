using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
   [Header("Boost Settings")]
    public float boostForce = 20f;       // Strength of the boost
    public float upwardForce = 0f;       // Optional: small lift effect
    public string targetTag = "Player";  // Tag to detect

    [Header("Effects")]
    public ParticleSystem boostEffect;   // Optional particle effect
    public AudioSource boostSound;       // Optional sound effect

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.cyan; // Arrow color in Scene view
    public float gizmoLength = 2f;        // How long the arrow should be


    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the object has the right tag
        if (other.CompareTag(targetTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Clear any downward velocity before boosting (optional)
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // Apply boost force in the pad's forward direction
                Vector3 boostDirection = transform.forward + (Vector3.up * upwardForce);
                rb.AddForce(boostDirection.normalized * boostForce, ForceMode.VelocityChange);

                // Play effects
                if (boostEffect != null)
                    boostEffect.Play();

                if (boostSound != null)
                    boostSound.Play();
            }
        }
    }
      private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Draw arrow line
        Vector3 startPos = transform.position;
        Vector3 direction = (transform.forward + Vector3.up * upwardForce).normalized;

        Vector3 endPos = startPos + direction * gizmoLength;
        Gizmos.DrawLine(startPos, endPos);

        // Draw arrowhead
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
        Gizmos.DrawLine(endPos, endPos + right * 0.5f);
        Gizmos.DrawLine(endPos, endPos + left * 0.5f);
    }
}
