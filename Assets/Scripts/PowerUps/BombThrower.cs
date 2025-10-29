using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class BombThrower : MonoBehaviour
{
	public GameObject bombPrefab;   // Assign in Inspector
	public Transform throwPoint;    // Point behind the player (create empty GameObject)
	public bool hasBomb = false;
	//public Sprite bombIcon;

	void Update()
	{
		//if (bombIcon != null) { hasBomb = true; }
		if (hasBomb && Input.GetButtonDown("Fire1"))  // Press A to throw
		{
			ThrowBomb();
		}
	}

	void ThrowBomb()
	{
		
		GameObject bomb = Instantiate(bombPrefab, throwPoint.position, throwPoint.rotation);
		Rigidbody rb = bomb.GetComponent<Rigidbody>();

		// Throw slightly backward
		rb.velocity = -transform.forward * 8f + transform.up * 2f;

		hasBomb = false;
		Debug.Log("Bomb thrown!");
	}
}
