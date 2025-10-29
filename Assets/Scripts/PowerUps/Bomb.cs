using UnityEngine;

public class Bomb : MonoBehaviour
{
	public float explosionDelay = 2f;
	public float explosionRadius = 5f;
	public float explosionForce = 700f;
	public GameObject explosionEffect;  // Assign a particle prefab
	private bool exploded = false;

	void Start()
	{
		Invoke("Explode", explosionDelay);
		//SoundManager.Instance.PlayExplosionSound(transform.position);
	}

	void Explode()
	{
		if (exploded) return;
		exploded = true;

		SoundManager.Instance.PlayExplosionSound(transform.position);

		// Spawn particle effect
		if (explosionEffect != null)
		{
			Instantiate(explosionEffect, transform.position, Quaternion.identity);			
		}

		// Apply physics explosion
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (Collider nearby in colliders)
		{
			PlayerShield shield = nearby.GetComponent<PlayerShield>();
			if (shield != null && shield.IsActive())
			{
				shield.AbsorbHit();
				Destroy(gameObject);
				return;
			}

			Rigidbody rb = nearby.GetComponent<Rigidbody>();
			if (rb != null)
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

			// If it hits another player, apply damage
			if (nearby.CompareTag("Player"))
			{
				Debug.Log(nearby.name + " hit by bomb!");
				// Optional: nearby.GetComponent<PlayerHealth>().TakeDamage(20);
			}
		}

		// Destroy bomb object
		Destroy(gameObject);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, explosionRadius);
	}

}

