using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Load the game scene. Make sure you have another scene set up for your game.
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        // Exit the application when the "Exit" button is clicked.
        Application.Quit();
    }
}
