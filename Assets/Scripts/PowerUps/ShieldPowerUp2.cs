using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Shield")]
public class ShieldPowerUp2 : PowerUpEffect
{
	[Header("Shield-Specific Settings")]
	public float duration = 5f; // How long the shield lasts

	// This prefab would be a "bubble" visual that you parent to the player
	public GameObject shieldVisualPrefab;

	public override void Activate(GameObject user)
	{
		// --- Feedback ---
		//if (activationSound)
		//{
		//	AudioSource.PlayClipAtPoint(activationSound, user.transform.position);
		//}

		// --- Logic ---
		// We look for a "PlayerHealth" (or "KartController") script on the user
		PlayerHealth health = user.GetComponent<PlayerHealth>();

		if (health != null)
		{
			// We tell that script to activate its shield.
			// This is good practice. The PlayerHealth script should manage its
			// own shielded state, not the power-up.
			health.ActivateShield(duration, shieldVisualPrefab);
			SoundManager.Instance.PlayShieldSound();
		}
	}
}
