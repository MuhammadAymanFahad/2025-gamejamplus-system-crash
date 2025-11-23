using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menuMusicClip;

    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartGameButton()
    {
        MusicManager.Instance.PlayExplorationMusic();
        SceneManager.LoadScene("World");
    }
}
