using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingController : MonoBehaviour
{
    [Header("UI Components")]
    public Slider masterSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        LoadSettings();
    }

    // Called when Master slider changed
    public void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        Debug.Log("Master volume changed to: " + value);
    }

    // Called when Music checkbox clicked
    public void OnMusicToggled(bool enabled)
    {
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        Debug.Log("Music enabled changed to: " + enabled);
        // Kalau kamu punya MusicManager → panggil di sini
        // MusicManager.Instance.SetEnabled(enabled);
    }

    // Called when SFX checkbox clicked
    public void OnSFXToggled(bool enabled)
    {
        PlayerPrefs.SetInt("SFXEnabled", enabled ? 1 : 0);
        Debug.Log("SFX enabled changed to: " + enabled);
        // SFXManager 
        // SFXManager.Instance.SetEnabled(enabled);
    }

    private void LoadSettings()
    {
        // MASTER VOLUME
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterSlider.value = master;
        AudioListener.volume = master;

        // MUSIC
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        musicToggle.isOn = musicEnabled;

        // SFX
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        sfxToggle.isOn = sfxEnabled;
    }
}
