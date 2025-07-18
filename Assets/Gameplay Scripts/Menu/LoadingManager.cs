using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainStory");
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (!operation.isDone)
        {
            // Sahne aslýnda yüklense bile biz yavaþ ilerletiyoruz
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (fakeProgress < targetProgress)
            {
                fakeProgress += Time.deltaTime * 0.2f; // yavaþ yavaþ artýyor
            }

            loadingBar.value = fakeProgress;
            loadingText.text = "Loading... " + Mathf.RoundToInt(fakeProgress * 100f) + "%";

            if (fakeProgress >= 1f)
            {
                loadingText.text = "Press any key to continue...";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

}
