using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Needed for the player list
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour
{
	[Header("Player & Camera Setup")]
	public Camera cam1;
	public Camera cam2;
	public GameObject player1;
	public GameObject player2;
	public GameObject GameMode_pnl;
	public GameObject GamePanel1;
	public GameObject GamePanel2;
	public Button twoPlayer;

	[Header("Race Settings")]
	public int lapsToComplete = 3;
	public int totalCheckpoints = 0;
	public float raceTime = 60f;
	private bool raceStarted = true;

	[Header("Player 1 UI")]
	public TextMeshProUGUI timerText;
	public TextMeshProUGUI wrongCheckpointText;
	public TextMeshProUGUI CheckpointNumberText;
	public TextMeshProUGUI ElapsedTimeText;
	public Image infoBackgroud;

	// --- CHANGED ---
	[Header("Player 2 UI")] // Was "Player 1 UI"
	public TextMeshProUGUI timerText2; // --- NEW --- (For 2-player)
	public TextMeshProUGUI wrongCheckpointText2;
	public TextMeshProUGUI CheckpointNumberText2;
	public TextMeshProUGUI ElapsedTimeText2;
	public Image infoBackgroud2;

	[Header("Results Panel")]
	public GameObject raceResultsPanel;
	public TextMeshProUGUI timeResultText;
	public TextMeshProUGUI lapsResultText;

	// --- Player Tracking ---
	private List<PlayerProgress> allPlayers = new List<PlayerProgress>();
	// --- NEW --- (To store references)
	private PlayerProgress player1Progress;
	private PlayerProgress player2Progress;

	// --- CHANGED --- (One coroutine for each player)
	private Coroutine warningRoutineP1;
	private Coroutine warningRoutineP2;
	private Coroutine checkpointRoutineP1;
	private Coroutine checkpointRoutineP2;

	private bool isTwoPlayer = false;

	void Start()
	{
		SetupGameMode();
		raceTime = 0f;
		StopAllCoroutines();
		raceResultsPanel.SetActive(false);
	}

	private void Update()
	{
		if (!raceStarted) return;

		raceTime += Time.deltaTime;
		string timeString = "TIME: " + raceTime.ToString("F1");

		// Update UI for both players
		if (timerText != null)
			timerText.text = timeString;

		// --- NEW ---
		if (isTwoPlayer && timerText2 != null)
			timerText2.text = timeString;
	}


	public void CheckForAllPlayersFinished()
	{
		if (allPlayers.Count == 0) return;
		bool allFinished = allPlayers.All(player => player.hasFinishedRace);
		if (allFinished)
		{
			Debug.Log("GAME OVER! All players have finished 3 laps.");
			EndRace();
		}
	}

	public void EndRace()
	{
		if (!raceStarted) return;
		raceStarted = false;
		Time.timeScale = 0f;

		raceResultsPanel.SetActive(true);

		var sortedPlayers = allPlayers.OrderBy(player => player.finalRaceTime);
		StringBuilder resultsString = new StringBuilder();
		int position = 1;
		foreach (PlayerProgress player in sortedPlayers)
		{
			resultsString.AppendLine($"{position}. {player.gameObject.name}: {player.finalRaceTime.ToString("F1")}s");
			position++;
		}

		if (timeResultText != null)
			timeResultText.text = resultsString.ToString();

		if (lapsResultText != null)
			lapsResultText.text = "Laps Completed: " + lapsToComplete;
	}

	#region Game Mode Setup
	void SetupGameMode()
	{
		allPlayers.Clear();

		// --- NEW --- (Cache player progress scripts)
		player1Progress = player1.GetComponent<PlayerProgress>();
		player2Progress = player2.GetComponent<PlayerProgress>();

		if (isTwoPlayer)
		{
			player1.SetActive(true);
			player2.SetActive(true);
			cam1.gameObject.SetActive(true);
			cam2.gameObject.SetActive(true);
			cam1.rect = new Rect(0, 0.5f, 1, 0.5f);
			cam2.rect = new Rect(0, 0, 1, 0.5f);

			GamePanel1.SetActive(true); // --- NEW ---
			GamePanel2.SetActive(true);

			allPlayers.Add(player1Progress);
			allPlayers.Add(player2Progress);
		}
		else
		{
			player1.SetActive(true);
			cam1.gameObject.SetActive(true);
			player2.SetActive(false);
			cam2.gameObject.SetActive(false);
			cam1.rect = new Rect(0, 0, 1, 1);

			GamePanel1.SetActive(true); // --- NEW ---
			GamePanel2.SetActive(false); // --- NEW ---

			allPlayers.Add(player1Progress);
		}
	}

	public void EnableTwoPlayerMode()
	{
		isTwoPlayer = true;
		SetupGameMode();
		GameMode_pnl.SetActive(false);
	}

	public void EnableSinglePlayerMode()
	{
		isTwoPlayer = false;
		SetupGameMode();
		GameMode_pnl.SetActive(false);
	}
	#endregion

	#region Time and UI Functions

	// --- CHANGED --- (Now requires a 'player' argument)
	public void ShowWrongCheckpointWarning(PlayerProgress player)
	{
		if (player == player1Progress)
		{
			if (warningRoutineP1 != null) StopCoroutine(warningRoutineP1);
			warningRoutineP1 = StartCoroutine(WrongCheckpointFlash(wrongCheckpointText)); // Pass in P1's UI
		}
		else if (isTwoPlayer && player == player2Progress)
		{
			if (warningRoutineP2 != null) StopCoroutine(warningRoutineP2);
			warningRoutineP2 = StartCoroutine(WrongCheckpointFlash(wrongCheckpointText2)); // Pass in P2's UI
		}
	}

	// --- CHANGED --- (Now takes the UI text as an argument)
	private System.Collections.IEnumerator WrongCheckpointFlash(TextMeshProUGUI textToFlash)
	{
		textToFlash.gameObject.SetActive(true);
		while (true)
		{
			textToFlash.enabled = !textToFlash.enabled;
			yield return new WaitForSeconds(0.5f);
		}
	}

	// --- CHANGED --- (Now requires a 'player' argument)
	public void HideWrongCheckpointWarning(PlayerProgress player)
	{
		if (player == player1Progress)
		{
			if (warningRoutineP1 != null) StopCoroutine(warningRoutineP1);
			warningRoutineP1 = null;
			wrongCheckpointText.gameObject.SetActive(false);
		}
		else if (isTwoPlayer && player == player2Progress)
		{
			if (warningRoutineP2 != null) StopCoroutine(warningRoutineP2);
			warningRoutineP2 = null;
			wrongCheckpointText2.gameObject.SetActive(false);
		}
	}

	// --- CHANGED --- (Now requires a 'player' argument)
	public void ShowCheckpointInfo(PlayerProgress player, int checkpointIndex, float elapsed, bool isBonus, float duration = 2f, float fadeDuration = 1f)
	{
		if (player == player1Progress)
		{
			if (checkpointRoutineP1 != null) StopCoroutine(checkpointRoutineP1);
			// Call the coroutine with P1's UI elements
			checkpointRoutineP1 = StartCoroutine(CheckpointInfoRoutine(
				checkpointIndex, elapsed, isBonus, duration, fadeDuration,
				CheckpointNumberText, ElapsedTimeText, infoBackgroud
			));
		}
		else if (isTwoPlayer && player == player2Progress)
		{
			if (checkpointRoutineP2 != null) StopCoroutine(checkpointRoutineP2);
			// Call the coroutine with P2's UI elements
			checkpointRoutineP2 = StartCoroutine(CheckpointInfoRoutine(
				checkpointIndex, elapsed, isBonus, duration, fadeDuration,
				CheckpointNumberText2, ElapsedTimeText2, infoBackgroud2
			));
		}
	}

	// --- CHANGED --- (Now takes UI elements as arguments)
	private System.Collections.IEnumerator CheckpointInfoRoutine(
		int checkpointIndex, float elapsed, bool isBonus, float duration, float fadeDuration,
		TextMeshProUGUI numberText, TextMeshProUGUI elapsedText, Image background)
	{
		SetAlpha(numberText, 1f);
		SetAlpha(elapsedText, 1f);

		numberText.text = $"Checkpoint {checkpointIndex + 1}";
		elapsedText.text = $"Time: {elapsed:F2}s";

		// (Your bonus/penalty logic would go here if you re-add it)

		background.gameObject.SetActive(true);
		numberText.gameObject.SetActive(true);
		elapsedText.gameObject.SetActive(true);

		yield return new WaitForSeconds(duration);

		float t = 0f;
		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
			SetAlpha(numberText, alpha);
			SetAlpha(elapsedText, alpha);
			yield return null;
		}

		background.gameObject.SetActive(false);
		numberText.gameObject.SetActive(false);
		elapsedText.gameObject.SetActive(false);

		// --- CHANGED --- (Set the correct coroutine to null)
		// This is a bit of a hack, but it works
		if (numberText == CheckpointNumberText)
			checkpointRoutineP1 = null;
		else
			checkpointRoutineP2 = null;
	}

	private void SetAlpha(TextMeshProUGUI text, float alpha)
	{
		Color c = text.color;
		c.a = alpha;
		text.color = c;
	}
	#endregion
}


