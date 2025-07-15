using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainStory");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Açýldý");
        // Settings paneli açma iþlemi
    }

    public void OpenCredits()
    {
        Debug.Log("Credits");
        // Credits sahnesi varsa yükle
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Çýkýlýyor...");
    }
}
