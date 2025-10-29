using TMPro;
using UnityEngine;
using UnityEngine.UI; // For the icon

public class PlayerPowerUpController : MonoBehaviour
{
	[Header("Power-Up State")]
	public PowerUpEffect currentPowerUp; // The SO we are currently holding

	[Header("UI")]
	public Image powerUpIconUI; // Drag your UI Image component here
	public Sprite defaultIcon; // A "blank" or "empty" sprite

	[Header("Feedback")]
	public AudioClip getPowerUpSound;
	private AudioSource audioSource; // For playing player-centric sounds
	public TextMeshProUGUI powerUpText;

	void Start()
	{
		//audioSource = GetComponent<AudioSource>(); // Assumes player has an AudioSource
		ClearPowerUp();
	}

	void Update()
	{
		// Example: Use the 'Fire1' button (e.g., Left Ctrl, Left Mouse)
		if (Input.GetButtonDown("Fire1") && currentPowerUp != null)
		{
			// Use the power-up!
			currentPowerUp.Activate(this.gameObject);

			// Clear it after use
			ClearPowerUp();
		}
	}

	// This is called by your Power-Up Boxes when you drive through them
	public void SetPowerUp(PowerUpEffect newPowerUp)
	{
		if (currentPowerUp != null)
		{
			// Already have one, do nothing
			return;
		}

		powerUpText.gameObject.SetActive(true);
		currentPowerUp = newPowerUp;

		// --- UI & Feedback ---
		powerUpIconUI.sprite = currentPowerUp.powerUpIcon;
		powerUpIconUI.enabled = true;

		if (audioSource && getPowerUpSound)
		{
			audioSource.PlayOneShot(getPowerUpSound);
		}
	}

	private void ClearPowerUp()
	{
		currentPowerUp = null;
		powerUpText.gameObject.SetActive(false);
		powerUpIconUI.sprite = defaultIcon;
		// You might want to disable the UI element entirely
		// powerUpIconUI.enabled = false; 
	}
}
