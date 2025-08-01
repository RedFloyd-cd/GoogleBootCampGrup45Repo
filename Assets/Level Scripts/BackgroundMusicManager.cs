using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;

    public AudioClip menuMusicClip;
    public AudioClip level1MusicClip;
    public AudioClip level2MusicClip;
    public AudioClip level3MusicClip;
    public AudioClip level4MusicClip;
    public AudioClip level5MusicClip;

    public float fadeDuration = 1.5f;

    [Header("UI Controls (Optional)")]
    public Slider musicSlider;
    public Slider masterSlider;
    public Toggle muteToggle;

    private float targetMusicVolume = 0.8f;

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

        // Load volume settings
        targetMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bool isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        backgroundMusicSource.mute = isMuted;
        backgroundMusicSource.volume = isMuted ? 0f : targetMusicVolume;
        AudioListener.volume = isMuted ? 0f : masterVolume;

        // Setup UI if assigned
        if (musicSlider != null)
        {
            musicSlider.value = targetMusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (masterSlider != null)
        {
            masterSlider.value = masterVolume;
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = isMuted;
            muteToggle.onValueChanged.AddListener(ToggleMute);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":
            case "LoadingScene":
                PlayMusic(menuMusicClip);
                break;

            case "MainStory": 
                StopMusic();
                break;

            case "TestREM01Map":
                PlayMusic(level1MusicClip);
                break;
            case "TestREM02Map":
                PlayMusic(level2MusicClip);
                break;
            case "TestREM03Map":
                PlayMusic(level3MusicClip);
                break;
            case "TestREM04Map":
                PlayMusic(level4MusicClip);
                break;
            case "TestREM05Map":
                PlayMusic(level5MusicClip);
                break;

            default:
                StopMusic();
                break;
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (backgroundMusicSource.clip == newClip) return;

        backgroundMusicSource.mute = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        targetMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        StartCoroutine(FadeToNewMusic(newClip));
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeToNewMusic(AudioClip newClip)
    {
        yield return FadeOutCoroutine();

        backgroundMusicSource.clip = newClip;
        backgroundMusicSource.Play();

        yield return FadeInCoroutine();
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = backgroundMusicSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = startVolume;
    }

    private IEnumerator FadeInCoroutine()
    {
        float t = 0f;
        backgroundMusicSource.volume = 0f;

        bool isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        float target = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            backgroundMusicSource.volume = isMuted ? 0f : Mathf.Lerp(0f, target, t / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.volume = isMuted ? 0f : target;
    }

    //  UI 

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        targetMusicVolume = value;

        if (!backgroundMusicSource.mute)
        {
            backgroundMusicSource.volume = value;
        }
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        if (PlayerPrefs.GetInt("IsMuted", 0) == 0)
            AudioListener.volume = value;
    }

    public void ToggleMute(bool mute)
    {
        PlayerPrefs.SetInt("IsMuted", mute ? 1 : 0);

        backgroundMusicSource.mute = mute;
        AudioListener.volume = mute ? 0f : PlayerPrefs.GetFloat("MasterVolume", 1f);

        if (mute)
            backgroundMusicSource.volume = 0f;
        else
            backgroundMusicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }
}
