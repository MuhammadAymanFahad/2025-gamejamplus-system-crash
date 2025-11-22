using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Debug.Log("Exit button pressed. Application quitting...");
        Application.Quit();
    }

    public void StartGameButton()
    {
        Debug.Log("Start Game button pressed. Loading game scene...");
        SceneManager.LoadScene("02_MapScene");
    }
}
