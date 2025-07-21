using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject[] levelMaps;
    private int currentSubLevel = 0;

    public GameObject upgradePanel;
    public string nextSceneName;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        EnemyTracker.OnAllEnemiesDead += OnLevelCompleted;
    }

    private void OnDisable()
    {
        EnemyTracker.OnAllEnemiesDead -= OnLevelCompleted;
    }

    private void Start()
    {
        SetActiveLevel(currentSubLevel);
        EnemyTracker.Instance?.InitializeLevel();
    }

    void SetActiveLevel(int index)
    {
        for (int i = 0; i < levelMaps.Length; i++)
            levelMaps[i].SetActive(i == index);
    }

    void OnLevelCompleted()
    {
        currentSubLevel++;

        if (currentSubLevel < levelMaps.Length)
        {
            SetActiveLevel(currentSubLevel);
            EnemyTracker.Instance?.InitializeLevel();
        }
        else
        {
            ShowUpgradeUI();
        }
    }

    void ShowUpgradeUI()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
    }

    public void StartCutscene()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainStory");
    }
}
