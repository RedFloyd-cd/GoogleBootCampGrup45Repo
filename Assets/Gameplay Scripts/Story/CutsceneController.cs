using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneStep
    {
        public GameObject canvasObject;
        public PlayableDirector director;
        public string nextSceneName;
    }

    public CutsceneStep[] steps;

    private int currentIndex = 0;
    private GameObject previousCanvas = null;

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt("CutsceneIndex", 0);
        PlayCurrentStep();
    }

    void PlayCurrentStep()
    {
        if (currentIndex >= steps.Length)
        {
            Debug.Log("Tüm cutscene adýmlarý tamamlandý.");
            return;
        }

        // Öncekini kapat
        if (previousCanvas != null)
        {
            previousCanvas.SetActive(false);
        }

        // Þimdiki canvas’ý aç
        steps[currentIndex].canvasObject.SetActive(true);
        previousCanvas = steps[currentIndex].canvasObject;

        // Timeline’ý oynat
        steps[currentIndex].director.stopped += OnTimelineStopped;
        steps[currentIndex].director.Play();
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        steps[currentIndex].director.stopped -= OnTimelineStopped;

        PlayerPrefs.SetInt("CutsceneIndex", currentIndex + 1);
        PlayerPrefs.Save();

        string nextScene = steps[currentIndex].nextSceneName;
        currentIndex++;

        SceneManager.LoadScene(nextScene);
    }
}
