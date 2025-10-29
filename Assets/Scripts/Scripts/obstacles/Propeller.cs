using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
     [Header("Spin Settings")]
    [Tooltip("Speed in degrees per second.")]
    public float speed = 360f; // degrees/sec

    [Tooltip("Axis of rotation in local space.")]
    public Vector3 axis = Vector3.up;

    [Tooltip("Use local space (true) or world space (false) for rotation axis.")]
    public bool useLocalSpace = true;

    [Tooltip("If true, uses Rigidbody.angularVelocity (physics). If false, rotates transform directly.")]
    public bool useRigidbody = false;

    [Header("Advanced")]
    [Tooltip("Smoothly accelerate to target speed (0 = instant).")]
    public float acceleration = 0f; // degrees/sec^2

    [Tooltip("Randomize initial rotation speed sign (flip direction).")]
    public bool randomizeDirection = false;

    // Internal
    Rigidbody rb;
    float currentSpeed; // degrees/sec
    float targetSpeed;

    void Awake()
    {
        if (useRigidbody)
            rb = GetComponent<Rigidbody>();

        // target speed set from inspector (may be negated if randomize)
        targetSpeed = speed * (randomizeDirection && Random.value < 0.5f ? -1f : 1f);

        // initialize current speed immediately (or 0 if using acceleration)
        currentSpeed = acceleration > 0f ? 0f : targetSpeed;
    }

    void Start()
    {
        // If using Rigidbody but none present, warn and fall back to transform
        if (useRigidbody && rb == null)
        {
            Debug.LogWarning($"PropellerSpin on '{name}': useRigidbody is true but no Rigidbody found. Falling back to Transform rotation.");
            useRigidbody = false;
        }

        // If using Rigidbody and it's kinematic, angularVelocity won't affect physics — it's okay for visual spin
        if (useRigidbody && rb != null && rb.isKinematic == false)
        {
            // Ensure interpolation for smoother rotations (optional)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        // Update speed (acceleration applied in update to keep it frame-rate independent)
        if (acceleration != 0f)
        {
            float sign = Mathf.Sign(targetSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Mathf.Abs(acceleration) * Time.deltaTime);
        }

        if (!useRigidbody)
        {
            // Transform-based rotation (simple, reliable)
            Vector3 axisDir = useLocalSpace ? transform.TransformDirection(axis) : axis.normalized;
            float deltaDegrees = currentSpeed * Time.deltaTime;
            transform.Rotate(axisDir, deltaDegrees, Space.World);
        }
    }

    void FixedUpdate()
    {
        if (!useRigidbody || rb == null) return;

        // Rigidbody-based spin: set angular velocity (radians/sec)
        Vector3 axisDir = useLocalSpace ? transform.TransformDirection(axis).normalized : axis.normalized;
        float targetRad = currentSpeed * Mathf.Deg2Rad;
        Vector3 desiredAngularVel = axisDir * targetRad;

        // If acceleration is used, smoothly approach desired angular velocity
        if (acceleration != 0f)
        {
            Vector3 currentAngular = rb.angularVelocity;
            // Move current angular velocity toward desired one; converting acceleration to rad/s^2
            float accelRad = Mathf.Abs(acceleration) * Mathf.Deg2Rad;
            Vector3 newAngular = Vector3.MoveTowards(currentAngular, desiredAngularVel, accelRad * Time.fixedDeltaTime);
            rb.angularVelocity = newAngular;
        }
        else
        {
            // Instant
            rb.angularVelocity = desiredAngularVel;
        }
    }

    // Editor helper: show a small icon/gizmo axis in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Vector3 axisDir = useLocalSpace ? transform.TransformDirection(axis) : axis.normalized;
        Gizmos.DrawLine(origin, origin + axisDir.normalized * 0.5f);
        Gizmos.DrawWireSphere(origin + axisDir.normalized * 0.5f, 0.03f);
    }
}
