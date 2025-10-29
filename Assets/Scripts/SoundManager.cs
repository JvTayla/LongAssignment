using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance; // Singleton reference
	public AudioSource explosionAudioSource; // Assign in Inspector
	public AudioClip explosionClip; // Optional

	public AudioSource shieldAudioSource; // Assign in Inspector
	public AudioClip shieldClip; // Optional

	void Awake()
	{
		// Make it globally accessible
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void PlayExplosionSound(Vector3 position)
	{
		// Option 1: Play directly from AudioSource
		explosionAudioSource.PlayOneShot(explosionClip);

		// Option 2: 3D sound at position
		// AudioSource.PlayClipAtPoint(explosionClip, position);
	}

	public void PlayShieldSound()
	{
		// Option 1: Play directly from AudioSource
		shieldAudioSource.PlayOneShot(shieldClip);

		// Option 2: 3D sound at position
		// AudioSource.PlayClipAtPoint(explosionClip, position);
	}
}

