using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PhantomCloak")]
public class PhantomCloakPowerUp : PowerUpEffect
{
	[Header("Phantom-Specific Settings")]
	public float duration = 4f;

	public override void Activate(GameObject user)
	{
		// --- Feedback ---
		if (activationSound)
		{
			AudioSource.PlayClipAtPoint(activationSound, user.transform.position);
		}

		// --- Logic ---
		// We find the PlayerHealth script, which will manage the state
		PlayerHealth health = user.GetComponent<PlayerHealth>();

		if (health != null)
		{
			// We tell that script to activate its phantom mode.
			health.ActivatePhantom(duration);
		}
	}
}