using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;
    public AudioClip backgroundMusicClip;
    public float fadeDuration = 1.5f; // You can adjust this

    public static BackgroundMusicManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (backgroundMusicSource != null && backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.Play();
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = newClip;
            backgroundMusicSource.Play();
        }
    }

    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutCoroutine());
        Debug.Log("FadeOutMusic called");
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

}
