using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
	[Header("Shield Settings")]
	public GameObject shieldVisual;   // Assign glowing shield object in Inspector
	public float shieldDuration = 5f;
	public int maxHits = 3;

	[Header("Power-Up Control")]
	public bool hasShieldPowerUp = false; // Player collected but hasn’t used it yet
	private bool isShieldActive = false;
	private int currentHits = 0;

	[Header("UI Feedback")]
	public UIController powerUpUI;

	private void Start()
	{
		if (shieldVisual != null)
			shieldVisual.SetActive(false);
	}

	private void Update()
	{
		// Use power-up when player presses a button
		// Controller: "joystick button 3" = Y button on Xbox controller
		// Keyboard fallback: Q key
		if (hasShieldPowerUp && !isShieldActive)
		{
			if (Input.GetKeyDown("Fire1"))
			{
				ActivateShield(shieldDuration);
				hasShieldPowerUp = false; // consume it
			}
		}
	}

	public void CollectShieldPowerUp()
	{
		hasShieldPowerUp = true;
		//Debug.Log("Shield Power-Up Collected! Press Q or Y to activate.");
		// (Optional) show icon or sound to indicate it's ready

		if (powerUpUI != null)
			powerUpUI.ShowShieldReady();
	}

	public void ActivateShield(float duration)
	{
		if (isShieldActive)
			StopAllCoroutines();

		StartCoroutine(ShieldRoutine(duration));

		if (powerUpUI != null)
			powerUpUI.ShowShieldActive();
	}

	private IEnumerator ShieldRoutine(float duration)
	{
		isShieldActive = true;
		currentHits = 0;

		if (shieldVisual != null)
			shieldVisual.SetActive(true);

		yield return new WaitForSeconds(duration);

		DeactivateShield();
	}

	public void AbsorbHit()
	{
		if (!isShieldActive) return;

		currentHits++;
		if (currentHits >= maxHits)
		{
			DeactivateShield();
		}
	}

	public bool IsActive()
	{
		return isShieldActive;
	}

	private void DeactivateShield()
	{
		isShieldActive = false;
		if (shieldVisual != null)
			shieldVisual.SetActive(false);

		Debug.Log("Shield expired!");

		if (powerUpUI != null)
			powerUpUI.HideAll();
	}
}
