using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Bomb")]
public class BombPowerUp : PowerUpEffect
{
	[Header("Bomb-Specific Settings")]
	public GameObject bombPrefab; // Assign your Bomb prefab here
	//public Transform throwPoint;
	public float dropDistanceBehind = 5f; // How far behind the player to drop it
	//public BombThrower BombThrower;

	public override void Activate(GameObject user)
	{		
		// Play the activation sound at the user's position
		//if (activationSound)
		//{
		//	AudioSource.PlayClipAtPoint(activationSound, user.transform.position);
		//}
				
		// Calculate the spawn position behind the player
		//BombThrower.hasBomb = true;
		Vector3 spawnPosition = user.transform.position - (user.transform.forward * dropDistanceBehind);

		// Ensure the bomb is level with the player (or you could raycast to the ground)
		spawnPosition.y = user.transform.position.y;

		// Spawn the bomb
		Instantiate(bombPrefab, spawnPosition, user.transform.rotation);

		// Note: The BombPrefab itself would have a script (e.g., "BombProjectile.cs")
		// that handles its own trigger/collision and explosion logic.
	}
}
