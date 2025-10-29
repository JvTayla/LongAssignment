using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your Pause Menu Panel here in the Inspector
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Or any other desired key
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f;         // Pause game time
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;          // Resume game time
        isPaused = false;
    }

    public void CityTrack()
    {
        SceneManager.LoadScene("Mikisha");
    }

    public void MountainTrack()
    {
        SceneManager.LoadScene("djibScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("TrackSelection");
    }
}
