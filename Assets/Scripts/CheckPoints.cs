using UnityEngine;

public class CheckPoints : MonoBehaviour
{
	[Header("Checkpoint Settings")]
	[Tooltip("The index of this checkpoint. 0 = first, 1 = second, etc.")]
	public int checkpointIndex;

	//[Header("Time Trial Settings")]
	//public float targetTime = 10f; // expected arrival (seconds since race start)
	//public float bonus = 5f;       // time added if early
	//public float penalty = 3f;     // time removed if late

	void OnTriggerEnter(Collider other)
	{
		// We only care about objects with the "PlayerProgress" script
		PlayerProgress player = other.GetComponent<PlayerProgress>();

		if (player != null)
		{
			// Tell the player's own script that it hit this checkpoint.
			// We pass 'this' so the player script can read our checkpointIndex, bonus, etc.
			player.HitCheckpoint(this);
		}
	}
}
