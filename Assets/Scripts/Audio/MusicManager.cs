using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        bool enabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        if (enabled)
            audioSource.Play();
        else
            audioSource.Stop();
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
    }
}
