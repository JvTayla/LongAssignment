using UnityEngine;

// This attribute allows you to right-click in the Project window and create a new Power-Up.
// You'll create this base class, but won't make assets from it.
public abstract class PowerUpEffect : ScriptableObject
{
	[Header("Power-Up UI & Feedback")]
	public string powerUpName;
	public Sprite powerUpIcon; // The icon for your UI
	public AudioClip activationSound; // Sound to play when USED

	// This is the core method. Every power-up must have an Activate function.
	// We pass in the GameObject that used the power-up.
	public abstract void Activate(GameObject user);
}
