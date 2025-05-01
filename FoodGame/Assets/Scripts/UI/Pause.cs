using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pause : MonoBehaviour
{
    public TMP_Text pausedText;
    private bool isPaused = false;
    private float previousTimeScale;

    private void Start()
    {
        pausedText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
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

    void PauseGame()
    {
        isPaused = true;
        pausedText.gameObject.SetActive(true);
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // Add any additional pause logic here, e.g., pausing audio, showing pause menu, etc.
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = previousTimeScale;
        pausedText.gameObject.SetActive(false);
        // Add any additional resume logic here, e.g., resuming audio, hiding pause menu, etc.
    }
}
