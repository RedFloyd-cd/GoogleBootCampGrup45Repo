using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            backgroundMusicSource.volume = 0.5f;
        }
        else
        {
            Destroy(gameObject);
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
        Debug.Log("Scene Loaded: " + scene.name); // For debugging

        switch (scene.name)
        {
            case "Menu":
            case "LoadingScene":
                PlayMusic(menuMusicClip);
                break;

            case "MainStory": // Cutscene
                StopMusic(); // silence during cutscene
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
                StopMusic(); // fallback
                break;
        }
    }


    public void PlayMusic(AudioClip newClip)
    {
        if (backgroundMusicSource.clip == newClip) return; // already playing

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
        float t = 0;

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
        float t = 0;
        backgroundMusicSource.volume = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        backgroundMusicSource.volume = 1f;
    }
}
