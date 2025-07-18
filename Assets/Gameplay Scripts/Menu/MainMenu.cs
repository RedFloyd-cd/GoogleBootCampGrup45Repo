using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    public Slider musicSlider;
    public TextMeshProUGUI musicVolumeText;

    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    public Toggle fullscreenToggle;
    public Toggle muteToggle;

    void Start()
    {
        // Load saved settings at startup
        LoadSettings();

        // Bind listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        muteToggle.onValueChanged.AddListener(SetMute);
    }

    // --- Panel Control ---
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void OpenCredits() => creditsPanel.SetActive(true);
    public void CloseCredits() => creditsPanel.SetActive(false);

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Çýkýlýyor...");
    }

    // --- Settings ---
    public void SetVolume(float value)
    {
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
        SaveSettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolumeText.text = Mathf.RoundToInt(value * 100f) + "%";
        SaveSettings();
    }

    public void SetSensitivity(float value)
    {
        sensitivityText.text = Mathf.RoundToInt(value * 100f) + "%";
        SaveSettings();
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        SaveSettings();
    }

    public void SetMute(bool isMuted)
    {
        // Ses sistemin varsa burada devre dýþý býrakabilirsin
        SaveSettings();
    }

    public void ResetDefaults()
    {
        volumeSlider.value = 0.25f;
        musicSlider.value = 0.33f;
        sensitivitySlider.value = 0.10f;
        fullscreenToggle.isOn = true;
        muteToggle.isOn = false;
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("mute", muteToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.25f);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.33f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 0.10f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        muteToggle.isOn = PlayerPrefs.GetInt("mute", 0) == 1;
    }
}
