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
        Debug.Log("Settings A��ld�");
        // Settings paneli a�ma i�lemi
    }

    public void OpenCredits()
    {
        Debug.Log("Credits");
        // Credits sahnesi varsa y�kle
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("��k�l�yor...");
    }
}
