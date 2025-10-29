using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Path Following")]
    public Transform[] waypoints; // Will be set by spawner
    public float baseSpeed = 15f; // Base speed (adjust for track length)
    public float waypointThreshold = 1f; // Distance to next waypoint before switching

    [Header("Optional Variations")]
    [Range(0.5f, 1.5f)]
    public float speedVariation = 1f; // Multiplier for random speed (set in spawner or randomize here)

    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("NPCController requires a Rigidbody on " + gameObject.name);
            return;
        }

        // Randomize speed slightly for variety (or set via spawner)
        currentSpeed = baseSpeed * Random.Range(0.8f, 1.2f) * speedVariation;

        if (waypoints != null && waypoints.Length > 0)
        {
            // Start at first waypoint
            transform.position = waypoints[0].position;
            if (waypoints.Length > 1)
            {
                // Face the second waypoint
                Vector3 direction = (waypoints[1].position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            Debug.LogWarning("No waypoints assigned to " + gameObject.name + ". Car won't move.");
        }
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex >= waypoints.Length) currentWaypointIndex = 0; // Loop

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Move towards waypoint
        rb.MovePosition(transform.position + direction * currentSpeed * Time.fixedDeltaTime);

        // Rotate towards waypoint (smooth turning)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f)); // Turn speed
        }

        // Check if close enough to switch waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Optional: Destroy car if it falls off track (e.g., y < -10)
    void Update()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}