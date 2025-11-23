using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene("02_MapScene");
    }
}
