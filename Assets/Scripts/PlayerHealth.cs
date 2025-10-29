using System.Collections;
using System.Collections.Generic; // We need this for Dictionaries
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	//[Header("States")]
	public bool isShielded { get; private set; } = false;
	public bool isPhantomed { get; private set; } = false;

	[Header("Shield")]
	private GameObject activeShieldVisual;

	[Header("Phantom")]
	[Tooltip("The transparent 'ghost' material to apply to the kart")]
	public Material ghostMaterial; // Assign this in the Inspector

	// --- NEW ---
	// We use a Dictionary to store all renderers and their original materials
	private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
	// --- END NEW ---

	// Physics Layer Management
	private int originalLayer;
	private int ghostLayer;

	private void Start()
	{
		// Cache the kart's original layer
		originalLayer = gameObject.layer;
		ghostLayer = LayerMask.NameToLayer("Ghost");

		// --- NEW ---
		// Find ALL renderers in this object and its children
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers)
		{
			// Store a copy of their current materials array
			originalMaterials[renderer] = (Material[])renderer.materials.Clone();
		}
		// --- END NEW ---
	}

	public void TakeHit()
	{
		if (isPhantomed)
		{
			Debug.Log("Hit passed through! (Phantom)");
			return;
		}

		if (isShielded)
		{
			BreakShield();
			return;
		}

		Debug.Log("Ouch! I'm hit!");
		// GetComponent<KartController>().SpinOut();
	}

	public void ActivateShield(float duration, GameObject shieldPrefab)
	{
		if (isShielded)
		{
			StopAllCoroutines();
		}

		isShielded = true;

		if (activeShieldVisual == null && shieldPrefab != null)
		{
			activeShieldVisual = Instantiate(shieldPrefab, transform.position, transform.rotation, transform);
		}

		StartCoroutine(ShieldTimer(duration));
	}

	private IEnumerator ShieldTimer(float duration)
	{
		yield return new WaitForSeconds(duration);
		BreakShield();
	}

	private void BreakShield()
	{
		isShielded = false;
		if (activeShieldVisual != null)
		{
			Destroy(activeShieldVisual);
			activeShieldVisual = null;
		}
	}

	// --- PHANTOM METHODS (Updated) ---

	public void ActivatePhantom(float duration)
	{
		if (isPhantomed)
		{
			StopAllCoroutines();
		}

		if (isShielded)
		{
			BreakShield();
		}

		StartCoroutine(PhantomRoutine(duration));
	}

	private IEnumerator PhantomRoutine(float duration)
	{
		isPhantomed = true;
		SetLayerRecursive(transform, ghostLayer);

		// --- NEW ---
		// Set all renderers to the ghost material
		SetAllMaterials(ghostMaterial);
		// --- END NEW ---

		yield return new WaitForSeconds(duration);

		isPhantomed = false;
		SetLayerRecursive(transform, originalLayer);

		// --- NEW ---
		// Revert all renderers to their original materials
		RevertAllMaterials();
		// --- END NEW ---
	}

	// --- NEW HELPER METHODS ---

	/// <summary>
	/// Applies a single material to ALL renderers on the kart.
	/// This is for the simple "ghost" effect.
	/// </summary>
	private void SetAllMaterials(Material material)
	{
		if (material == null)
		{
			Debug.LogError("Ghost Material is not assigned in PlayerHealth!");
			return;
		}

		foreach (Renderer renderer in originalMaterials.Keys)
		{
			// Create a new material array filled with just the ghost material
			int matCount = renderer.materials.Length;
			Material[] ghostMats = new Material[matCount];
			for (int i = 0; i < matCount; i++)
			{
				ghostMats[i] = material;
			}
			// Assign the new array
			renderer.materials = ghostMats;
		}
	}

	/// <summary>
	/// Restores all original materials to their respective renderers.
	/// </summary>
	private void RevertAllMaterials()
	{
		foreach (KeyValuePair<Renderer, Material[]> entry in originalMaterials)
		{
			// entry.Key is the Renderer
			// entry.Value is the original Material[]
			entry.Key.materials = entry.Value;
		}
	}

	// --- END NEW HELPER METHODS ---

	private void SetLayerRecursive(Transform root, int layer)
	{
		root.gameObject.layer = layer;
		foreach (Transform child in root)
		{
			SetLayerRecursive(child, layer);
		}
	}
}
