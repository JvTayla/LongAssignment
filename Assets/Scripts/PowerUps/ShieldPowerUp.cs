using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerShield playerShield = other.GetComponent<PlayerShield>();
			if (playerShield != null)
			{
				playerShield.CollectShieldPowerUp();
			}

			// Destroy pickup from the world
			Destroy(gameObject);
		}
	}
}
