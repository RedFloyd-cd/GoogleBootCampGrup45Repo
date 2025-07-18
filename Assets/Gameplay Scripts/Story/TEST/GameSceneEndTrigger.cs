using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneEndTrigger : MonoBehaviour
{
    public void EndLevel()
    {
        SceneManager.LoadScene("MainStory");
    }
}
