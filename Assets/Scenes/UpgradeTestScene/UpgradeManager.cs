using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("UI")]
    public Button healthButton;
    public Button damageButton;
    public Button cooldownButton;
    public GameObject upgradePanel;

    [Header("Upgrade Values")]
    public float healthIncreaseAmount = 50f;
    public float damageIncreaseAmount = 10f;
    public float cooldownMultiplier = 0.8f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        healthButton.onClick.AddListener(ApplyHealthUpgrade);
        damageButton.onClick.AddListener(ApplyDamageUpgrade);
        cooldownButton.onClick.AddListener(ApplyCooldownUpgrade);
    }

    void ApplyHealthUpgrade()
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.maxHealth += healthIncreaseAmount;
            Debug.Log($"Yeni maxHealth: {player.maxHealth}");
        }
        ContinueToCutscene();
    }

    void ApplyDamageUpgrade()
    {
        var pistol = Object.FindFirstObjectByType<PistolController>();
        if (pistol != null)
        {
            pistol.bulletDamage += damageIncreaseAmount;
            Debug.Log($"Yeni damage: {pistol.bulletDamage}");
        }
        ContinueToCutscene();
    }

    void ApplyCooldownUpgrade()
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.dashCooldown *= cooldownMultiplier;
            Debug.Log($"Yeni dashCooldown: {player.dashCooldown}");
        }
        ContinueToCutscene();
    }

    void ContinueToCutscene()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f;
        LevelManager.Instance.StartCutscene();
    }
}
