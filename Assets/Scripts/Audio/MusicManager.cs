using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource mainMenu;
    public AudioSource exploration;

    [Header("Settings")]
    public float crossfadeDuration = 1.5f;

    private AudioSource activeSource;
    private AudioSource idleSource;

    private const string PREF_KEY = "MusicEnabled";

    private bool isEnabled;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Validation
        if (mainMenu == null || exploration == null)
        {
            Debug.LogError("MusicManager requires two AudioSource components assigned.");
        }

        activeSource = mainMenu;
        idleSource = exploration;

        activeSource.loop = true;
        idleSource.loop = true;

        // Load preference
        isEnabled = PlayerPrefs.GetInt(PREF_KEY, 1) == 1;
    }

    private void Start()
    {
        if (!isEnabled)
        {
            activeSource.Stop();
            idleSource.Stop();
        } else
        {
            activeSource.Play();
        }
    }

    // Called from UI Toggle
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        PlayerPrefs.SetInt(PREF_KEY, enabled ? 1 : 0);

        if (!enabled)
        {
            activeSource.Stop();
            idleSource.Stop();
        }
        else
        {
            // if toggle enabled after disabled, simply play active clip again
            if (activeSource.clip != null && !activeSource.isPlaying)
                activeSource.Play();
        }
    }

    // Public API used by scenes
    public void PlayExplorationMusic()
    {
        if (!isEnabled) return;

        activeSource.Stop();
        idleSource.Play();
    }

    public void PlayMainMenuMusic()
    {
        if (!isEnabled) return;

        activeSource.Play();
        idleSource.Stop();
    }

    private IEnumerator CrossfadeRoutine()
    {
        idleSource.volume = 0f;
        idleSource.Play();

        float timer = 0f;
        float startVolume = activeSource.volume;

        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / crossfadeDuration;

            activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
            idleSource.volume = Mathf.Lerp(0f, startVolume, t);

            yield return null;
        }

        // Swap active & idle
        AudioSource temp = activeSource;
        activeSource = idleSource;
        idleSource = temp;

        idleSource.Stop();
    }
}
