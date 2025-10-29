using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBox : MonoBehaviour
{	
	public PowerUpEffect[] possiblePowerUps;
	public float respawnTime = 5f;
	public GameObject collectParticles;

	//Sound to play when collected.
	public AudioClip collectSound;

	// Internal references
	private Collider myCollider;
	private MeshRenderer[] childRenderers; // Gets all renderers, in case your box is a complex model

	private void Start()
	{
		// Cache components for performance
		myCollider = GetComponent<Collider>();
		childRenderers = GetComponentsInChildren<MeshRenderer>();

		// Ensure the collider is set to be a Trigger
		if (!myCollider.isTrigger)
		{
			Debug.LogWarning("PowerUpBox collider was not set to 'Is Trigger'. Forcing it.", this);
			myCollider.isTrigger = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("power up box was touched by: " + other.name);
		// 1. Check if the object that hit us has a PlayerPowerUpController
		PlayerPowerUpController player = other.GetComponent<PlayerPowerUpController>();

		// 2. If it's not a player, or if that player already has a power-up, do nothing.
		if (player == null || player.currentPowerUp != null)
		{
			return;
		}

		// 3. Check if we even have any power-ups to give
		if (possiblePowerUps.Length == 0)
		{
			Debug.LogError("PowerUpBox has no power-ups assigned in the 'possiblePowerUps' array!", this);
			return;
		}

		// 4. Pick a random power-up from the list
		int randomIndex = Random.Range(0, possiblePowerUps.Length);
		PowerUpEffect powerUpToGive = possiblePowerUps[randomIndex];

		// 5. Give the power-up to the player
		player.SetPowerUp(powerUpToGive);

		// 6. Start the respawn process
		StartCoroutine(RespawnRoutine());
	}

	private IEnumerator RespawnRoutine()
	{
		// --- Deactivate ---
		SetBoxActive(false);

		// --- Feedback ---
		if (collectParticles != null)
		{
			Instantiate(collectParticles, transform.position, Quaternion.identity);
		}
		if (collectSound != null)
		{
			AudioSource.PlayClipAtPoint(collectSound, transform.position);
		}

		// --- Wait ---
		yield return new WaitForSeconds(respawnTime);

		// --- Reactivate ---
		SetBoxActive(true);
	}

	/// <summary>
	/// Helper method to turn the box's visuals and collider on/off.
	/// </summary>
	private void SetBoxActive(bool active)
	{
		// Disable the collider so it can't be triggered again
		myCollider.enabled = active;

		// Hide all visual parts of the box
		foreach (MeshRenderer renderer in childRenderers)
		{
			renderer.enabled = active;
		}
	}
}
