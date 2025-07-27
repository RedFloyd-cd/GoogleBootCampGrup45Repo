using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThankYouSceneController : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 2f;

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.canvasRenderer.SetAlpha(1f);
            fadePanel.CrossFadeAlpha(0f, fadeDuration, false);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
