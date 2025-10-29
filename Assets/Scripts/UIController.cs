using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Image shieldIcon;        // Icon when collected
	public Image shieldActiveGlow;  // Glow when shield is active
	public float fadeDuration = 0.5f;

	private Coroutine fadeRoutine;

	void Start()
	{
		shieldIcon.gameObject.SetActive(false);
		shieldActiveGlow.gameObject.SetActive(false);
	}

	public void ShowShieldReady()
	{
		if (fadeRoutine != null) StopCoroutine(fadeRoutine);
		shieldIcon.gameObject.SetActive(true);
		shieldIcon.color = new Color(1, 1, 1, 1);
		shieldActiveGlow.gameObject.SetActive(false);
	}

	public void ShowShieldActive()
	{
		if (fadeRoutine != null) StopCoroutine(fadeRoutine);
		shieldActiveGlow.gameObject.SetActive(true);
		shieldActiveGlow.color = new Color(0, 1, 1, 1);
		StartCoroutine(FlashGlow());
	}

	public void HideAll()
	{
		if (fadeRoutine != null) StopCoroutine(fadeRoutine);
		fadeRoutine = StartCoroutine(FadeOut());
	}

	private IEnumerator FlashGlow()
	{
		// gentle pulse while active
		//float t = 0;
		while (true)
		{
			float alpha = Mathf.PingPong(Time.time * 2f, 0.5f) + 0.5f;
			shieldActiveGlow.color = new Color(0, 0, 1, alpha);
			yield return null;
		}
	}

	private IEnumerator FadeOut()
	{
		float t = 0;
		Color iconStart = shieldIcon.color;
		Color glowStart = shieldActiveGlow.color;

		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			float a = Mathf.Lerp(1, 0, t / fadeDuration);
			shieldIcon.color = new Color(iconStart.r, iconStart.g, iconStart.b, a);
			shieldActiveGlow.color = new Color(glowStart.r, glowStart.g, glowStart.b, a);
			yield return null;
		}

		shieldIcon.gameObject.SetActive(false);
		shieldActiveGlow.gameObject.SetActive(false);
	}
}
