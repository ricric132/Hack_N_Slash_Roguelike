using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pasueUI;

    public void Start()
    {
        pasueUI.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Change to your desired pause key
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0; // Pause the game
            pasueUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1; // Resume the game
            pasueUI.SetActive(false);
        }
    }
    public void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Time.timeScale = 1;
        pasueUI.SetActive(false);
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("WinScreen");
        Time.timeScale = 1;
        pasueUI.SetActive(false); // Load the scene named "Menu"
    }
}