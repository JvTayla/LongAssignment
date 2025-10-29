using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
	[Header("Player State")]
	public int nextCheckpointIndex = 0;
	public int currentLap = 0;
	public bool hasFinishedRace = false;
	public float finalRaceTime = 0f;

	private GameManager gameManager;

	void Start()
	{
		// Find the manager and tell it about me
		gameManager = FindObjectOfType<GameManager>();
		if ( gameManager == null ) 
		{
			Debug.LogError("GameManager not found in scene!", this);
		}
	}

	/// <summary>
	/// This is called by the CheckPoints script when we hit one.
	/// </summary>
	public void HitCheckpoint(CheckPoints checkpoint)
	{
		// Don't do anything if I've already finished
		if (hasFinishedRace || gameManager == null) return;

		// Is this the correct checkpoint in the sequence?
		if (checkpoint.checkpointIndex == nextCheckpointIndex)
		{
			// --- Correct Checkpoint ---
			Debug.Log(gameObject.name + " hit correct checkpoint " + checkpoint.checkpointIndex);

			// Update this player's next expected checkpoint
			nextCheckpointIndex++;

			// --- Handle Time Bonus/Penalty ---
			// This still calls the GameManager's global UI, which isn't ideal
			// for 2 players (it will flicker), but it matches your existing code.
			float elapsed = Time.timeSinceLevelLoad;
			gameManager.ShowCheckpointInfo(this,checkpoint.checkpointIndex, elapsed, true);
			//if (elapsed <= checkpoint.targetTime)
			//{
			//	//gameManager.AddTime(checkpoint.bonus);
			//	gameManager.ShowCheckpointInfo(checkpoint.checkpointIndex, elapsed, checkpoint.bonus, true);
			//}
			//else
			//{
			//	//gameManager.SubtractTime(checkpoint.penalty);
			//	gameManager.ShowCheckpointInfo(checkpoint.checkpointIndex, elapsed, checkpoint.penalty, false);
			//}
			gameManager.HideWrongCheckpointWarning(this);

			// --- Handle Lap Completion ---
			if (nextCheckpointIndex >= gameManager.totalCheckpoints)
			{
				// We've hit the last checkpoint, so we finished a lap.
				nextCheckpointIndex = 0; // Loop back to the first checkpoint
				currentLap++;
				Debug.Log(gameObject.name + " completed lap! Total Laps: " + currentLap);

				// Check if this player has finished the whole race
				if (currentLap >= gameManager.lapsToComplete)
				{
					finalRaceTime = gameManager.raceTime;
					hasFinishedRace = true;
					Debug.Log(gameObject.name + " has FINISHED THE RACE AT: " + finalRaceTime.ToString("F1") + "s");

					// Tell the GameManager to check if EVERYONE is done
					gameManager.CheckForAllPlayersFinished();
				}
			}
		}
		else
		{
			// --- Wrong Checkpoint ---
			Debug.LogWarning(gameObject.name + " hit wrong checkpoint! Expected: " + nextCheckpointIndex + ", Got: " + checkpoint.checkpointIndex);
			gameManager.ShowWrongCheckpointWarning(this);
		}
	}
}
