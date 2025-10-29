 using UnityEngine;

public class BombPickup : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			BombThrower thrower = other.GetComponent<BombThrower>();
			if (thrower != null)
			{
				thrower.hasBomb = true;
				Debug.Log("Player picked up a bomb!");
			}

			// Destroy pickup
			Destroy(gameObject);
		}
	}
}
